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

    [HttpGet]
    public async Task<IActionResult> GetBySemester([FromQuery] Guid semesterId)
    {
        var calendars = await _calendarService.GetBySemesterAsync(semesterId);
        return Ok(new { code = 200, data = calendars });
    }

    [HttpGet("today")]
    [Authorize(Policy = "Permission:calendar:read")]
    public async Task<IActionResult> GetToday()
    {
        var calendar = await _calendarService.GetTodayAsync();
        if (calendar == null)
            return NotFound(new { code = 404, message = "今天不在校历中" });
        return Ok(new { code = 200, data = calendar });
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        var calendar = await _calendarService.GetByDateAsync(date);
        if (calendar == null)
            return NotFound(new { code = 404, message = "该日期不在校历中" });
        return Ok(new { code = 200, data = calendar });
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
        return Ok(new { code = 200, data = calendar, message = "更新成功" });
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
            .Select(c => new
            {
                c.Id,
                c.Date,
                c.HolidayName,
                c.HolidayType,
                c.IsWorkday,
                c.Description
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

        return Ok(new { code = 200, data = calendar, message = "节假日添加成功" });
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

        return Ok(new { code = 200, data = calendar, message = "调休设置成功" });
    }

    [HttpGet("by-event-type")]
    [Authorize(Policy = "Permission:calendar:read")]
    public async Task<IActionResult> GetByEventType([FromQuery] Guid semesterId, [FromQuery] CalendarEventType eventType)
    {
        var calendars = await _calendarService.GetBySemesterAsync(semesterId);
        var filtered = calendars.Where(c => c.EventType == eventType).ToList();
        return Ok(new { code = 200, data = filtered });
    }
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
