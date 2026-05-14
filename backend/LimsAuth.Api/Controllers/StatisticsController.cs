using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/statistics")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("weekly-summary")]
    public async Task<ActionResult> GetWeeklySummary([FromQuery] ScheduleStatisticsQuery query)
    {
        var list = await _statisticsService.GetWeeklyUsageSummaryAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("lab-usage")]
    public async Task<ActionResult> GetLabUsage([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetLabUsageCountAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-major")]
    public async Task<ActionResult> GetByMajor([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetMajorUsageStatsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-class")]
    public async Task<ActionResult> GetByClass([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetClassUsageStatsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-grade")]
    public async Task<ActionResult> GetByGrade([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetGradeUsageStatsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-course")]
    public async Task<ActionResult> GetByCourse([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetCourseUsageStatsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("reservation")]
    public async Task<ActionResult> GetReservationStats([FromQuery] StatisticsQuery query)
    {
        var list = await _statisticsService.GetReservationStatsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("completion-rate")]
    public async Task<ActionResult> GetCompletionRate([FromQuery] StatisticsQuery query)
    {
        var rate = await _statisticsService.GetRegistrationCompletionRateAsync(query);
        return Ok(new { code = 200, data = rate });
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard([FromQuery] DashboardQuery query)
    {
        try
        {
            var data = await _statisticsService.GetDashboardDataAsync(query);
            return Ok(new { code = 200, data = data });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { code = 500, message = ex.Message, detail = ex.StackTrace });
        }
    }

    [HttpGet("export")]
    public IActionResult Export([FromQuery] StatisticsQuery query, [FromQuery] string type = "lab-usage")
    {
        return Ok(new { code = 200, message = "导出功能待集成Excel库" });
    }
}
