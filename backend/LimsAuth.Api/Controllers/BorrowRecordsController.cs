using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/borrow-records")]
[Authorize]
public class BorrowRecordsController : ControllerBase
{
    private readonly IEquipmentBorrowService _borrowService;

    public BorrowRecordsController(IEquipmentBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    private Guid GetCurrentUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
    }

    private bool IsAdmin()
    {
        var isAdmin =
            User.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == "lab_admin" || c.Value == "super_admin" || c.Value == "admin")) ||
            User.HasClaim(c => c.Type == "role" && (c.Value == "lab_admin" || c.Value == "super_admin" || c.Value == "admin"));
        return isAdmin;
    }

    /// <summary>提交借出申请</summary>
    [HttpPost]
    [Authorize(Policy = "Permission:equipment:borrow")]
    public async Task<IActionResult> CreateBorrowRequest([FromBody] CreateBorrowRequestDto request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { code = 401, message = "用户未登录" });

        try
        {
            var record = await _borrowService.CreateBorrowRequestAsync(request, userId);
            return Ok(new { code = 200, data = record, message = "借出申请已提交" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>我的借还记录</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRecords([FromQuery] string? status)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized(new { code = 401, message = "用户未登录" });

        var list = await _borrowService.GetMyRecordsAsync(userId, status);
        return Ok(new { code = 200, data = list });
    }

    /// <summary>待审批列表（导师/管理员）</summary>
    [HttpGet("pending")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var isAdmin = IsAdmin();
        var list = await _borrowService.GetPendingTeacherApprovalsAsync(isAdmin);
        return Ok(new { code = 200, data = list });
    }

    /// <summary>全部借还记录（管理员）</summary>
    [HttpGet]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> GetAllRecords([FromQuery] string? status, [FromQuery] string? keyword)
    {
        var list = await _borrowService.GetAllRecordsAsync(status, keyword);
        return Ok(new { code = 200, data = list });
    }

    /// <summary>借还记录详情</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var record = await _borrowService.GetByIdAsync(id);
        if (record == null)
            return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, data = record });
    }

    /// <summary>按单号查询（扫码用）</summary>
    [HttpGet("no/{recordNo}")]
    public async Task<IActionResult> GetByRecordNo(string recordNo)
    {
        var record = await _borrowService.GetByRecordNoAsync(recordNo);
        if (record == null)
            return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, data = record });
    }

    /// <summary>导师审批</summary>
    [HttpPost("{id}/supervisor-approve")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> SupervisorApprove(Guid id, [FromBody] BorrowApprovalRequest request)
    {
        var approverId = GetCurrentUserId();
        var (success, message) = await _borrowService.SupervisorApproveAsync(id, approverId, request.Approved, request.Remark);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>管理员审批</summary>
    [HttpPost("{id}/admin-approve")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> AdminApprove(Guid id, [FromBody] BorrowApprovalRequest request)
    {
        var approverId = GetCurrentUserId();
        var (success, message) = await _borrowService.AdminApproveAsync(id, approverId, request.Approved, request.Remark);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>审批归还（管理员批准/驳回归还申请）</summary>
    [HttpPost("{id}/approve-return")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> ApproveReturn(Guid id, [FromBody] ReturnApprovalRequest request)
    {
        var checkerId = GetCurrentUserId();
        var (success, message) = await _borrowService.ApproveReturnAsync(id, checkerId, request.Approved, request.Condition ?? "完好", request.Remarks);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>审批续借（导师/管理员批准/驳回续借申请）</summary>
    [HttpPost("{id}/approve-renew")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> ApproveRenew(Guid id, [FromBody] BorrowApprovalRequest request)
    {
        var approverId = GetCurrentUserId();
        var (success, message) = await _borrowService.ApproveRenewAsync(id, approverId, request.Approved, request.Remark);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>确认借出（扫码核验）</summary>
    [HttpPost("{id}/confirm-borrow")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> ConfirmBorrow(Guid id)
    {
        var operatorId = GetCurrentUserId();
        var (success, message) = await _borrowService.ConfirmBorrowAsync(id, operatorId);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>提交归还申请</summary>
    [HttpPost("{id}/submit-return")]
    public async Task<IActionResult> SubmitReturn(Guid id)
    {
        var userId = GetCurrentUserId();
        var (success, message) = await _borrowService.SubmitReturnAsync(id, userId);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>确认归还（扫码验收）</summary>
    [HttpPost("{id}/confirm-return")]
    [Authorize(Policy = "Permission:equipment:approve")]
    public async Task<IActionResult> ConfirmReturn(Guid id, [FromBody] ReturnConfirmRequest request)
    {
        var checkerId = GetCurrentUserId();
        var (success, message) = await _borrowService.ConfirmReturnAsync(id, checkerId, request.Condition, request.Remarks);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>续借申请</summary>
    [HttpPost("{id}/renew")]
    public async Task<IActionResult> Renew(Guid id, [FromBody] RenewRequest request)
    {
        var userId = GetCurrentUserId();
        var (success, message) = await _borrowService.RenewAsync(id, userId, request.NewReturnDate);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }

    /// <summary>逾期清单</summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> GetOverdueRecords()
    {
        var list = await _borrowService.GetOverdueRecordsAsync();
        return Ok(new { code = 200, data = list });
    }

    /// <summary>即将到期提醒</summary>
    [HttpGet("expiring")]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> GetExpiringRecords([FromQuery] int daysBefore = 1)
    {
        var list = await _borrowService.GetExpiringRecordsAsync(daysBefore);
        return Ok(new { code = 200, data = list });
    }

    /// <summary>借还流水账</summary>
    [HttpGet("flow")]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> GetBorrowFlow([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var list = await _borrowService.GetBorrowFlowAsync(startDate, endDate);
        return Ok(new { code = 200, data = list });
    }

    /// <summary>删除借还记录（仅已归还）</summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:equipment:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (success, message) = await _borrowService.DeleteAsync(id);
        if (!success)
            return BadRequest(new { code = 400, message });
        return Ok(new { code = 200, message });
    }
}

public class BorrowApprovalRequest
{
    public bool Approved { get; set; }
    public string? Remark { get; set; }
}

public class ReturnConfirmRequest
{
    public string Condition { get; set; } = "完好";
    public string? Remarks { get; set; }
}

public class RenewRequest
{
    public DateTime NewReturnDate { get; set; }
}

public class ReturnApprovalRequest
{
    public bool Approved { get; set; }
    public string? Condition { get; set; }
    public string? Remarks { get; set; }
}
