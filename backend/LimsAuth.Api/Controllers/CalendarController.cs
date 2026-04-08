using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;

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
}
