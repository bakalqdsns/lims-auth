using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ConsumablesController : ControllerBase
{
    private readonly IConsumableService _consumableService;

    public ConsumablesController(IConsumableService consumableService)
    {
        _consumableService = consumableService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetList([FromQuery] ConsumableQueryRequest query)
    {
        var result = await _consumableService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _consumableService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> Create([FromBody] CreateConsumableRequest request)
    {
        var result = await _consumableService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:consumable:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConsumableRequest request)
    {
        var result = await _consumableService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:consumable:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _consumableService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("statistics")]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _consumableService.GetStatisticsAsync();
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/consumable-in-records")]
[Authorize]
public class ConsumableInRecordsController : ControllerBase
{
    private readonly IInboundService _inboundService;

    public ConsumableInRecordsController(IInboundService inboundService)
    {
        _inboundService = inboundService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetList([FromQuery] InboundQueryRequest query)
    {
        var result = await _inboundService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> Create([FromBody] CreateInboundRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _inboundService.CreateAsync(request, userId);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "Permission:consumable:approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _inboundService.ApproveAsync(id, request, userId);
        return StatusCode(result.Code, result);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }
}

[ApiController]
[Route("api/v1/consumable-out-records")]
[Authorize]
public class ConsumableOutRecordsController : ControllerBase
{
    private readonly IOutboundService _outboundService;

    public ConsumableOutRecordsController(IOutboundService outboundService)
    {
        _outboundService = outboundService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetList([FromQuery] OutboundQueryRequest query)
    {
        var result = await _outboundService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> Create([FromBody] CreateOutboundRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _outboundService.CreateAsync(request, userId);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "Permission:consumable:approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _outboundService.ApproveAsync(id, request, userId);
        return StatusCode(result.Code, result);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }
}

[ApiController]
[Route("api/v1/consumable-stock-logs")]
[Authorize]
public class ConsumableStockLogsController : ControllerBase
{
    private readonly IStockLogService _stockLogService;

    public ConsumableStockLogsController(IStockLogService stockLogService)
    {
        _stockLogService = stockLogService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:consumable:read")]
    public async Task<IActionResult> GetList([FromQuery] StockLogQueryRequest query)
    {
        var result = await _stockLogService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpPost("adjust")]
    [Authorize(Policy = "Permission:consumable:update")]
    public async Task<IActionResult> AdjustStock([FromBody] CreateStockAdjustmentRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _stockLogService.AdjustStockAsync(request, userId);
        return StatusCode(result.Code, result);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }
}
