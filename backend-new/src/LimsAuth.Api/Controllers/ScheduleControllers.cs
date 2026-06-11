using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:schedule:read")]
    public async Task<IActionResult> GetList([FromQuery] ScheduleQueryRequest query)
    {
        var result = await _scheduleService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:schedule:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _scheduleService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:schedule:create")]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var result = await _scheduleService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:schedule:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleRequest request)
    {
        var result = await _scheduleService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:schedule:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _scheduleService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("dashboard")]
    [Authorize(Policy = "Permission:schedule:read")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _scheduleService.GetDashboardAsync();
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ExperimentItemsController : ControllerBase
{
    private readonly IExperimentItemService _experimentItemService;

    public ExperimentItemsController(IExperimentItemService experimentItemService)
    {
        _experimentItemService = experimentItemService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:experimentitem:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _experimentItemService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:experimentitem:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _experimentItemService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:experimentitem:create")]
    public async Task<IActionResult> Create([FromBody] CreateExperimentItemRequest request)
    {
        var result = await _experimentItemService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:experimentitem:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExperimentItemRequest request)
    {
        var result = await _experimentItemService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:experimentitem:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _experimentItemService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/booking-applies")]
[Authorize]
public class BookingAppliesController : ControllerBase
{
    private readonly IBookingApplyService _bookingApplyService;

    public BookingAppliesController(IBookingApplyService bookingApplyService)
    {
        _bookingApplyService = bookingApplyService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:booking:read")]
    public async Task<IActionResult> GetList([FromQuery] BookingApplyQueryRequest query)
    {
        var result = await _bookingApplyService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:booking:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _bookingApplyService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:booking:create")]
    public async Task<IActionResult> Create([FromBody] CreateBookingApplyRequest request)
    {
        var result = await _bookingApplyService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/audit")]
    [Authorize(Policy = "Permission:booking:approve")]
    public async Task<IActionResult> Audit(Guid id, [FromBody] AuditBookingRequest request)
    {
        var result = await _bookingApplyService.AuditAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:booking:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookingApplyService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/usage-registers")]
[Authorize]
public class UsageRegistersController : ControllerBase
{
    private readonly IUsageRegisterService _usageRegisterService;

    public UsageRegistersController(IUsageRegisterService usageRegisterService)
    {
        _usageRegisterService = usageRegisterService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:usage:read")]
    public async Task<IActionResult> GetList([FromQuery] UsageRegisterQueryRequest query)
    {
        var result = await _usageRegisterService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:usage:create")]
    public async Task<IActionResult> Create([FromBody] CreateUsageRegisterRequest request)
    {
        var result = await _usageRegisterService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:usage:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _usageRegisterService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}
