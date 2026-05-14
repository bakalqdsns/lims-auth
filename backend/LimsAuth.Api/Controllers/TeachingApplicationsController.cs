using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/teaching-applications")]
[Authorize]
public class TeachingApplicationsController : ControllerBase
{
    private readonly ITeachingApplicationService _service;

    public TeachingApplicationsController(ITeachingApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeachingApplicationDto>>> GetApplications([FromQuery] TeachingApplicationQuery query)
    {
        try
        {
            var list = await _service.GetApplicationsAsync(query);
            return Ok(new { code = 200, data = list });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { code = 500, message = ex.Message, detail = ex.StackTrace });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeachingApplicationDto>> GetApplication(Guid id)
    {
        var app = await _service.GetApplicationByIdAsync(id);
        if (app == null) return NotFound(new { code = 404, message = "授课申请不存在" });
        return Ok(new { code = 200, data = app });
    }

    [HttpPost]
    public async Task<ActionResult<TeachingApplicationDto>> CreateApplication([FromBody] CreateTeachingApplicationRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var createdBy = User.Identity?.Name;
        var app = await _service.CreateApplicationAsync(request, userId, userName, createdBy);
        return Ok(new { code = 200, data = app, message = "授课申请提交成功" });
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveApplication(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var approverName = GetUserName();
        var ok = await _service.ApproveApplicationAsync(id, request, approverId, approverName);
        if (!ok) return NotFound(new { code = 404, message = "授课申请不存在或已审批" });
        return Ok(new { code = 200, message = "授课申请审批通过" });
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectApplication(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var ok = await _service.RejectApplicationAsync(id, request, approverId);
        if (!ok) return NotFound(new { code = 404, message = "授课申请不存在或已审批" });
        return Ok(new { code = 200, message = "授课申请已驳回" });
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelApplication(Guid id)
    {
        var cancelledBy = User.Identity?.Name;
        var ok = await _service.CancelApplicationAsync(id, cancelledBy);
        if (!ok) return NotFound(new { code = 404, message = "授课申请不存在" });
        return Ok(new { code = 200, message = "授课申请已取消" });
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<TeachingApplicationDto>>> GetPending([FromQuery] Guid? semesterId)
    {
        var list = await _service.GetPendingApplicationsAsync(semesterId);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<TeachingApplicationDto>>> GetMyApplications([FromQuery] Guid? semesterId)
    {
        var userId = GetUserId();
        var list = await _service.GetMyApplicationsAsync(userId, semesterId);
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
