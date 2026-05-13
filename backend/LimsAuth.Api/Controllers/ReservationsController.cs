using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations([FromQuery] ReservationQuery query)
    {
        var list = await _reservationService.GetReservationsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(Guid id)
    {
        var r = await _reservationService.GetReservationByIdAsync(id);
        if (r == null) return NotFound(new { code = 404, message = "预约记录不存在" });
        return Ok(new { code = 200, data = r });
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var phone = User.FindFirstValue("phone") ?? "";
        var createdBy = User.Identity?.Name;
        var r = await _reservationService.CreateReservationAsync(request, userId, userName, phone, createdBy);
        return Ok(new { code = 200, data = r, message = "预约申请提交成功" });
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveReservation(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var approverName = GetUserName();
        var ok = await _reservationService.ApproveReservationAsync(id, request, approverId, approverName);
        if (!ok) return NotFound(new { code = 404, message = "预约记录不存在或已审批" });
        return Ok(new { code = 200, message = "预约审批通过" });
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectReservation(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var ok = await _reservationService.RejectReservationAsync(id, request, approverId);
        if (!ok) return NotFound(new { code = 404, message = "预约记录不存在或已审批" });
        return Ok(new { code = 200, message = "预约已驳回" });
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(Guid id, [FromBody] CancelRequest request)
    {
        var cancelledById = GetUserId();
        var ok = await _reservationService.CancelReservationAsync(id, request, cancelledById);
        if (!ok) return NotFound(new { code = 404, message = "预约记录不存在" });
        return Ok(new { code = 200, message = "预约已取消" });
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetPending([FromQuery] Guid? semesterId)
    {
        var list = await _reservationService.GetPendingReservationsAsync(semesterId);
        return Ok(new { code = 200, data = list });
    }

    private Guid GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
    }

    private string GetUserName()
    {
        return User.FindFirstValue(ClaimTypes.Name) ?? "未知用户";
    }
}
