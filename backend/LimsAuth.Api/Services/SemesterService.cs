using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface ISemesterService
{
    Task<List<Semester>> GetListAsync(string? keyword = null, bool? isCurrent = null);
    Task<Semester?> GetByIdAsync(Guid id);
    Task<Semester> CreateAsync(CreateSemesterRequest request);
    Task<Semester?> UpdateAsync(Guid id, UpdateSemesterRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SetCurrentAsync(Guid id);
    Task<Semester?> GetCurrentAsync();
    Task<List<AcademicCalendar>> GenerateCalendarAsync(Guid semesterId);
}

public class SemesterService : ISemesterService
{
    private readonly AppDbContext _dbContext;

    public SemesterService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Semester>> GetListAsync(string? keyword = null, bool? isCurrent = null)
    {
        var query = _dbContext.Semesters.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.Name.Contains(keyword));
        }

        if (isCurrent.HasValue)
        {
            query = query.Where(s => s.IsCurrent == isCurrent.Value);
        }

        return await query.OrderByDescending(s => s.StartDate).ToListAsync();
    }

    public async Task<Semester?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Semesters.FindAsync(id);
    }

    public async Task<Semester> CreateAsync(CreateSemesterRequest request)
    {
        var semester = new Semester
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = request.IsCurrent ?? false,
            IsActive = true
        };

        // 如果设置为当前学期，取消其他学期的当前状态
        if (semester.IsCurrent)
        {
            var currentSemesters = await _dbContext.Semesters.Where(s => s.IsCurrent).ToListAsync();
            foreach (var s in currentSemesters)
            {
                s.IsCurrent = false;
            }
        }

        _dbContext.Semesters.Add(semester);
        await _dbContext.SaveChangesAsync();
        return semester;
    }

    public async Task<Semester?> UpdateAsync(Guid id, UpdateSemesterRequest request)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return null;

        if (request.Name != null) semester.Name = request.Name;
        if (request.StartDate.HasValue) semester.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) semester.EndDate = request.EndDate.Value;
        if (request.IsCurrent.HasValue) semester.IsCurrent = request.IsCurrent.Value;
        if (request.IsActive.HasValue) semester.IsActive = request.IsActive.Value;

        // 如果设置为当前学期，取消其他学期的当前状态
        if (request.IsCurrent == true)
        {
            var currentSemesters = await _dbContext.Semesters.Where(s => s.IsCurrent && s.Id != id).ToListAsync();
            foreach (var s in currentSemesters)
            {
                s.IsCurrent = false;
            }
        }

        await _dbContext.SaveChangesAsync();
        return semester;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        _dbContext.Semesters.Remove(semester);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetCurrentAsync(Guid id)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        // 取消其他学期的当前状态
        var currentSemesters = await _dbContext.Semesters.Where(s => s.IsCurrent).ToListAsync();
        foreach (var s in currentSemesters)
        {
            s.IsCurrent = false;
        }

        semester.IsCurrent = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Semester?> GetCurrentAsync()
    {
        return await _dbContext.Semesters.FirstOrDefaultAsync(s => s.IsCurrent && s.IsActive);
    }

    public async Task<List<AcademicCalendar>> GenerateCalendarAsync(Guid semesterId)
    {
        var semester = await _dbContext.Semesters.FindAsync(semesterId);
        if (semester == null) throw new Exception("学期不存在");

        // 删除旧的校历
        var oldCalendars = await _dbContext.AcademicCalendars.Where(c => c.SemesterId == semesterId).ToListAsync();
        _dbContext.AcademicCalendars.RemoveRange(oldCalendars);

        var calendars = new List<AcademicCalendar>();
        var currentDate = semester.StartDate.Date;
        var endDate = semester.EndDate.Date;
        int weekNumber = 1;

        while (currentDate <= endDate)
        {
            var dayOfWeek = (int)currentDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // 周日作为第7天

            calendars.Add(new AcademicCalendar
            {
                SemesterId = semesterId,
                Date = currentDate,
                WeekNumber = weekNumber,
                DayOfWeek = dayOfWeek,
                IsHoliday = false
            });

            if (dayOfWeek == 7)
            {
                weekNumber++;
            }

            currentDate = currentDate.AddDays(1);
        }

        _dbContext.AcademicCalendars.AddRange(calendars);
        await _dbContext.SaveChangesAsync();

        return calendars;
    }
}

public class CreateSemesterRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool? IsCurrent { get; set; }
}

public class UpdateSemesterRequest
{
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsCurrent { get; set; }
    public bool? IsActive { get; set; }
}
