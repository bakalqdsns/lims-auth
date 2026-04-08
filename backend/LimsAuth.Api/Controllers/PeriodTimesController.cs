using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/period-times")]
[Authorize]
public class PeriodTimesController : ControllerBase
{
    private readonly IPeriodTimeService _periodTimeService;

    public PeriodTimesController(IPeriodTimeService periodTimeService)
    {
        _periodTimeService = periodTimeService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:period_time:read")]
    public async Task<IActionResult> GetList()
    {
        var periods = await _periodTimeService.GetListAsync();
        return Ok(new { code = 200, data = periods });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:period_time:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var period = await _periodTimeService.GetByIdAsync(id);
        if (period == null)
            return NotFound(new { code = 404, message = "节次不存在" });
        return Ok(new { code = 200, data = period });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:period_time:create")]
    public async Task<IActionResult> Create([FromBody] CreatePeriodTimeRequest request)
    {
        var period = await _periodTimeService.CreateAsync(request);
        return Ok(new { code = 200, data = period, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:period_time:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePeriodTimeRequest request)
    {
        var period = await _periodTimeService.UpdateAsync(id, request);
        if (period == null)
            return NotFound(new { code = 404, message = "节次不存在" });
        return Ok(new { code = 200, data = period, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:period_time:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _periodTimeService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "节次不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:period_time:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _periodTimeService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "节次不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}
