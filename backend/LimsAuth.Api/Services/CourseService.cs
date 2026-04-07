using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? courseType = null);
    Task<CourseDto?> GetByIdAsync(Guid id);
    Task<Course> CreateAsync(CreateCourseRequest request);
    Task<Course?> UpdateAsync(Guid id, UpdateCourseRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _dbContext;

    public CourseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CourseDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? courseType = null)
    {
        var query = _dbContext.Courses
            .Include(c => c.Department)
            .Include(c => c.Manager)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(c => c.Name.Contains(keyword) || c.Code.Contains(keyword) || (c.EnglishName != null && c.EnglishName.Contains(keyword)));
        }

        if (!string.IsNullOrEmpty(departmentId) && Guid.TryParse(departmentId, out var deptId))
        {
            query = query.Where(c => c.DepartmentId == deptId);
        }

        if (!string.IsNullOrEmpty(courseType))
        {
            query = query.Where(c => c.CourseType == courseType);
        }

        var courses = await query.OrderBy(c => c.Code).ToListAsync();

        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            EnglishName = c.EnglishName,
            CourseType = c.CourseType,
            Credits = c.Credits,
            TotalHours = c.TotalHours,
            TheoryHours = c.TheoryHours,
            PracticeHours = c.PracticeHours,
            ExperimentHours = c.ExperimentHours,
            OnlineHours = c.OnlineHours,
            SemesterType = c.SemesterType,
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department?.Name,
            ManagerId = c.ManagerId,
            ManagerName = c.Manager?.FullName ?? c.Manager?.Username,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<CourseDto?> GetByIdAsync(Guid id)
    {
        var course = await _dbContext.Courses
            .Include(c => c.Department)
            .Include(c => c.Manager)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        return new CourseDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            EnglishName = course.EnglishName,
            CourseType = course.CourseType,
            Credits = course.Credits,
            TotalHours = course.TotalHours,
            TheoryHours = course.TheoryHours,
            PracticeHours = course.PracticeHours,
            ExperimentHours = course.ExperimentHours,
            OnlineHours = course.OnlineHours,
            SemesterType = course.SemesterType,
            DepartmentId = course.DepartmentId,
            DepartmentName = course.Department?.Name,
            ManagerId = course.ManagerId,
            ManagerName = course.Manager?.FullName ?? course.Manager?.Username,
            Description = course.Description,
            IsActive = course.IsActive,
            CreatedAt = course.CreatedAt
        };
    }

    public async Task<Course> CreateAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            Code = request.Code,
            Name = request.Name,
            EnglishName = request.EnglishName,
            CourseType = request.CourseType,
            Credits = request.Credits,
            TotalHours = request.TotalHours,
            TheoryHours = request.TheoryHours,
            PracticeHours = request.PracticeHours,
            ExperimentHours = request.ExperimentHours,
            OnlineHours = request.OnlineHours,
            SemesterType = request.SemesterType,
            DepartmentId = request.DepartmentId,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync();
        return course;
    }

    public async Task<Course?> UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        if (course == null) return null;

        if (request.Code != null) course.Code = request.Code;
        if (request.Name != null) course.Name = request.Name;
        if (request.EnglishName != null) course.EnglishName = request.EnglishName;
        if (request.CourseType != null) course.CourseType = request.CourseType;
        if (request.Credits.HasValue) course.Credits = request.Credits.Value;
        if (request.TotalHours.HasValue) course.TotalHours = request.TotalHours.Value;
        if (request.TheoryHours.HasValue) course.TheoryHours = request.TheoryHours.Value;
        if (request.PracticeHours.HasValue) course.PracticeHours = request.PracticeHours.Value;
        if (request.ExperimentHours.HasValue) course.ExperimentHours = request.ExperimentHours.Value;
        if (request.OnlineHours.HasValue) course.OnlineHours = request.OnlineHours.Value;
        if (request.SemesterType.HasValue) course.SemesterType = request.SemesterType.Value;
        if (request.DepartmentId.HasValue) course.DepartmentId = request.DepartmentId.Value;
        if (request.ManagerId.HasValue) course.ManagerId = request.ManagerId.Value;
        if (request.Description != null) course.Description = request.Description;
        if (request.IsActive.HasValue) course.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        return course;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        if (course == null) return false;

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var course = await _dbContext.Courses.FindAsync(id);
        if (course == null) return false;

        course.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class CourseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string CourseType { get; set; } = string.Empty;
    public decimal Credits { get; set; }
    public int TotalHours { get; set; }
    public int TheoryHours { get; set; }
    public int PracticeHours { get; set; }
    public int ExperimentHours { get; set; }
    public int OnlineHours { get; set; }
    public int SemesterType { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCourseRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public string CourseType { get; set; } = "必修";
    public decimal Credits { get; set; }
    public int TotalHours { get; set; }
    public int TheoryHours { get; set; }
    public int PracticeHours { get; set; }
    public int ExperimentHours { get; set; }
    public int OnlineHours { get; set; }
    public int SemesterType { get; set; } = 1;
    public Guid? DepartmentId { get; set; }
    public Guid? ManagerId { get; set; }
    public string? Description { get; set; }
}

public class UpdateCourseRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? EnglishName { get; set; }
    public string? CourseType { get; set; }
    public decimal? Credits { get; set; }
    public int? TotalHours { get; set; }
    public int? TheoryHours { get; set; }
    public int? PracticeHours { get; set; }
    public int? ExperimentHours { get; set; }
    public int? OnlineHours { get; set; }
    public int? SemesterType { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ManagerId { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
