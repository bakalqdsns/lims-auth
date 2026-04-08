using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/calendar")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly IAcademicCalendarService _calendarService;

    public CalendarController(IAcademicCalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    // 转换为DTO，避免循环引用
    private static CalendarDto ToDto(AcademicCalendar c) => new()
    {
        Id = c.Id,
        SemesterId = c.SemesterId,
        Date = c.Date,
        WeekNumber = c.WeekNumber,
        DayOfWeek = c.DayOfWeek,
        EventType = c.EventType.ToString(),
        EventName = c.EventName,
        EventPriority = c.EventPriority.ToString(),
        IsHoliday = c.IsHoliday,
        IsWorkday = c.IsWorkday,
        IsTeachingDay = c.IsTeachingDay,
        IsExamDay = c.IsExamDay,
        IsAdjusted = c.IsAdjusted,
        HolidayName = c.HolidayName,
        HolidayType = c.HolidayType,
        Description = c.Description,
        Color = c.Color,
        Icon = c.Icon,
        AffectsCourseSelection = c.AffectsCourseSelection,
        AffectsScheduling = c.AffectsScheduling,
        AffectsGradeEntry = c.AffectsGradeEntry,
        AffectsRegistration = c.AffectsRegistration,
        AutoTriggerAction = c.AutoTriggerAction,
        TriggeredAt = c.TriggeredAt,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    [HttpGet]
    public async Task<IActionResult> GetBySemester([FromQuery] Guid semesterId)
    {
        var calendars = await _calendarService.GetBySemesterAsync(semesterId);
        var dtos = calendars.Select(ToDto).ToList();
        return Ok(new { code = 200, data = dtos });
    }

    [HttpGet("today")]
    [Authorize(Policy = "Permission:calendar:read")]
    public async Task<IActionResult> GetToday()
    {
        var calendar = await _calendarService.GetTodayAsync();
        if (calendar == null)
            return NotFound(new { code = 404, message = "今天不在校历中" });
        return Ok(new { code = 200, data = ToDto(calendar) });
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        var calendar = await _calendarService.GetByDateAsync(date);
        if (calendar == null)
            return NotFound(new { code = 404, message = "该日期不在校历中" });
        return Ok(new { code = 200, data = ToDto(calendar) });
    }

    [HttpGet("week-info")]
    public async Task<IActionResult> GetWeekInfo([FromQuery] Guid semesterId, [FromQuery] int weekNumber)
    {
        var weekInfo = await _calendarService.GetWeekInfoAsync(semesterId, weekNumber);
        if (weekInfo == null)
            return NotFound(new { code = 404, message = "周次信息不存在" });
        return Ok(new { code = 200, data = weekInfo });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCalendarRequest request)
    {
        var calendar = await _calendarService.UpdateAsync(id, request);
        if (calendar == null)
            return NotFound(new { code = 404, message = "日历不存在" });
        return Ok(new { code = 200, data = ToDto(calendar), message = "更新成功" });
    }

    [HttpGet("event-types")]
    [Authorize(Policy = "Permission:calendar:read")]
    public IActionResult GetEventTypes()
    {
        var eventTypes = new[]
        {
            new { code = "Teaching", name = "教学日", color = "#67C23A" },
            new { code = "Exam", name = "考试", color = "#E6A23C" },
            new { code = "Holiday", name = "节假日", color = "#F56C6C" },
            new { code = "Registration", name = "注册", color = "#409EFF" },
            new { code = "CourseSelection", name = "选课", color = "#409EFF" },
            new { code = "GradeEntry", name = "成绩录入", color = "#409EFF" },
            new { code = "Sports", name = "运动会", color = "#67C23A" },
            new { code = "Activity", name = "校园活动", color = "#909399" },
            new { code = "Maintenance", name = "系统维护", color = "#909399" },
            new { code = "Custom", name = "自定义", color = "#909399" }
        };
        return Ok(new { code = 200, data = eventTypes });
    }

    [HttpGet("check-permission")]
    [Authorize]
    public async Task<IActionResult> CheckBusinessPermission([FromQuery] Guid semesterId, [FromQuery] string businessType)
    {
        var today = DateTime.UtcNow.Date;
        var calendar = await _calendarService.GetByDateAsync(today);
        
        if (calendar == null)
        {
            return Ok(new { code = 200, data = new { isAllowed = false, message = "当前日期不在校历范围内", reason = "OUT_OF_RANGE" } });
        }

        bool isAllowed = true;
        string message = "允许操作";
        string reason = "OK";

        switch (businessType.ToLower())
        {
            case "course_selection":
                if (calendar.AffectsCourseSelection && !calendar.IsWorkday)
                {
                    isAllowed = false;
                    message = "当前时间不在选课时间段内";
                    reason = "COURSE_SELECTION_CLOSED";
                }
                break;
            case "registration":
                if (calendar.AffectsRegistration && !calendar.IsWorkday)
                {
                    isAllowed = false;
                    message = "当前时间不在注册时间段内";
                    reason = "REGISTRATION_CLOSED";
                }
                break;
            case "grade_entry":
                if (calendar.AffectsGradeEntry && !calendar.IsWorkday)
                {
                    isAllowed = false;
                    message = "当前时间不在成绩录入时间段内";
                    reason = "GRADE_ENTRY_CLOSED";
                }
                break;
            case "scheduling":
                if (calendar.AffectsScheduling && !calendar.IsWorkday)
                {
                    isAllowed = false;
                    message = "当前时间不在排课时间段内";
                    reason = "SCHEDULING_CLOSED";
                }
                break;
        }

        return Ok(new { code = 200, data = new { isAllowed, message, reason, currentDate = today, weekNumber = calendar.WeekNumber, dayOfWeek = calendar.DayOfWeek } });
    }

    [HttpGet("holidays")]
    [Authorize(Policy = "Permission:calendar:read")]
    public async Task<IActionResult> GetHolidays([FromQuery] Guid semesterId)
    {
        var calendars = await _calendarService.GetBySemesterAsync(semesterId);
        var holidays = calendars
            .Where(c => c.IsHoliday)
            .Select(c => new HolidayDto
            {
                Id = c.Id,
                Date = c.Date,
                Name = c.HolidayName,
                Type = c.HolidayType,
                IsWorkday = c.IsWorkday,
                Description = c.Description
            })
            .OrderBy(c => c.Date)
            .ToList();
        return Ok(new { code = 200, data = holidays });
    }

    [HttpPost("holidays")]
    [Authorize(Policy = "Permission:calendar:update")]
    public async Task<IActionResult> AddHoliday([FromBody] AddHolidayRequest request)
    {
        var calendar = await _calendarService.GetByDateAsync(request.Date);
        if (calendar == null)
            return NotFound(new { code = 404, message = "该日期不在校历范围内" });

        calendar.IsHoliday = true;
        calendar.HolidayName = request.Name;
        calendar.HolidayType = request.Type;
        calendar.IsWorkday = request.IsWorkday;
        calendar.IsTeachingDay = false;
        calendar.EventType = CalendarEventType.Holiday;
        calendar.Description = request.Description;
        calendar.Color = "#F56C6C";

        await _calendarService.UpdateAsync(calendar.Id, new UpdateCalendarRequest
        {
            IsHoliday = true,
            HolidayName = request.Name,
            Description = request.Description
        });

        return Ok(new { code = 200, data = ToDto(calendar), message = "节假日添加成功" });
    }

    [HttpPost("adjust-workday")]
    [Authorize(Policy = "Permission:calendar:update")]
    public async Task<IActionResult> AdjustWorkday([FromBody] AdjustWorkdayRequest request)
    {
        var calendar = await _calendarService.GetByDateAsync(request.Date);
        if (calendar == null)
            return NotFound(new { code = 404, message = "该日期不在校历范围内" });

        calendar.IsWorkday = request.IsWorkday;
        calendar.IsTeachingDay = request.IsWorkday && calendar.DayOfWeek <= 5;
        calendar.IsAdjusted = true;
        calendar.AdjustedFrom = request.AdjustedFrom;
        calendar.Description = request.Description ?? $"调休调整：{(request.IsWorkday ? "设为工作日" : "设为休息日")}";

        await _calendarService.UpdateAsync(calendar.Id, new UpdateCalendarRequest
        {
            IsWorkday = request.IsWorkday,
            Description = calendar.Description
        });

        return Ok(new { code = 200, data = ToDto(calendar), message = "调休设置成功" });
    }

    [HttpGet("by-event-type")]
    [Authorize(Policy = "Permission:calendar:read")]
    public async Task<IActionResult> GetByEventType([FromQuery] Guid semesterId, [FromQuery] CalendarEventType eventType)
    {
        var calendars = await _calendarService.GetBySemesterAsync(semesterId);
        var filtered = calendars.Where(c => c.EventType == eventType).Select(ToDto).ToList();
        return Ok(new { code = 200, data = filtered });
    }
}

public class CalendarDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public DateTime Date { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? EventName { get; set; }
    public string EventPriority { get; set; } = string.Empty;
    public bool IsHoliday { get; set; }
    public bool IsWorkday { get; set; }
    public bool IsTeachingDay { get; set; }
    public bool IsExamDay { get; set; }
    public bool IsAdjusted { get; set; }
    public string? HolidayName { get; set; }
    public string? HolidayType { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool AffectsCourseSelection { get; set; }
    public bool AffectsScheduling { get; set; }
    public bool AffectsGradeEntry { get; set; }
    public bool AffectsRegistration { get; set; }
    public string? AutoTriggerAction { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class HolidayDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public bool IsWorkday { get; set; }
    public string? Description { get; set; }
}

public class AddHolidayRequest
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "法定假日";
    public bool IsWorkday { get; set; } = false;
    public string? Description { get; set; }
}

public class AdjustWorkdayRequest
{
    public DateTime Date { get; set; }
    public bool IsWorkday { get; set; }
    public DateTime? AdjustedFrom { get; set; }
    public string? Description { get; set; }
}