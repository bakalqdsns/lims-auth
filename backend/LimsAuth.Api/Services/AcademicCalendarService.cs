using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IAcademicCalendarService
{
    Task<List<AcademicCalendar>> GetBySemesterAsync(Guid semesterId);
    Task<AcademicCalendar?> GetByIdAsync(Guid id);
    Task<AcademicCalendar?> GetTodayAsync();
    Task<AcademicCalendar?> GetByDateAsync(DateTime date);
    Task<AcademicCalendar?> UpdateAsync(Guid id, UpdateCalendarRequest request);
    Task<WeekInfoDto?> GetWeekInfoAsync(Guid semesterId, int weekNumber);
}

public class AcademicCalendarService : IAcademicCalendarService
{
    private readonly AppDbContext _dbContext;

    public AcademicCalendarService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AcademicCalendar>> GetBySemesterAsync(Guid semesterId)
    {
        return await _dbContext.AcademicCalendars
            .Where(c => c.SemesterId == semesterId)
            .OrderBy(c => c.Date)
            .ToListAsync();
    }

    public async Task<AcademicCalendar?> GetByIdAsync(Guid id)
    {
        return await _dbContext.AcademicCalendars.FindAsync(id);
    }

    public async Task<AcademicCalendar?> GetTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbContext.AcademicCalendars
            .FirstOrDefaultAsync(c => c.Date == today);
    }

    public async Task<AcademicCalendar?> GetByDateAsync(DateTime date)
    {
        var dateOnly = date.Date;
        return await _dbContext.AcademicCalendars
            .FirstOrDefaultAsync(c => c.Date == dateOnly);
    }

    public async Task<AcademicCalendar?> UpdateAsync(Guid id, UpdateCalendarRequest request)
    {
        var calendar = await _dbContext.AcademicCalendars.FindAsync(id);
        if (calendar == null) return null;

        if (request.EventType.HasValue) calendar.EventType = request.EventType.Value;
        if (request.EventName != null) calendar.EventName = request.EventName;
        if (request.IsHoliday.HasValue) calendar.IsHoliday = request.IsHoliday.Value;
        if (request.IsWorkday.HasValue) calendar.IsWorkday = request.IsWorkday.Value;
        if (request.IsTeachingDay.HasValue) calendar.IsTeachingDay = request.IsTeachingDay.Value;
        if (request.HolidayName != null) calendar.HolidayName = request.HolidayName;
        if (request.Description != null) calendar.Description = request.Description;
        if (request.Color != null) calendar.Color = request.Color;

        calendar.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return calendar;
    }

    public async Task<WeekInfoDto?> GetWeekInfoAsync(Guid semesterId, int weekNumber)
    {
        var days = await _dbContext.AcademicCalendars
            .Where(c => c.SemesterId == semesterId && c.WeekNumber == weekNumber)
            .OrderBy(c => c.Date)
            .ToListAsync();

        if (!days.Any()) return null;

        return new WeekInfoDto
        {
            SemesterId = semesterId,
            WeekNumber = weekNumber,
            StartDate = days.First().Date,
            EndDate = days.Last().Date,
            Days = days.Select(d => new CalendarDayDto
            {
                Date = d.Date,
                DayOfWeek = d.DayOfWeek,
                IsHoliday = d.IsHoliday,
                HolidayName = d.HolidayName
            }).ToList()
        };
    }
}

public class UpdateCalendarRequest
{
    public CalendarEventType? EventType { get; set; }
    public string? EventName { get; set; }
    public bool? IsHoliday { get; set; }
    public bool? IsWorkday { get; set; }
    public bool? IsTeachingDay { get; set; }
    public string? HolidayName { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class WeekInfoDto
{
    public Guid SemesterId { get; set; }
    public int WeekNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CalendarDayDto> Days { get; set; } = new();
}

public class CalendarDayDto
{
    public DateTime Date { get; set; }
    public int DayOfWeek { get; set; }
    public bool IsHoliday { get; set; }
    public string? HolidayName { get; set; }
}
