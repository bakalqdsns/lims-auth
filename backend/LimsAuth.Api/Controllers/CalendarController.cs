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
}
