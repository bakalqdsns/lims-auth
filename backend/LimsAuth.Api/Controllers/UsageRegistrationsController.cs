using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/usage-registrations")]
[Authorize]
public class UsageRegistrationsController : ControllerBase
{
    private readonly IUsageRegistrationService _service;

    public UsageRegistrationsController(IUsageRegistrationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsageRegistrationDto>>> GetRegistrations([FromQuery] UsageRegistrationQuery query)
    {
        var list = await _service.GetRegistrationsAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsageRegistrationDto>> GetRegistration(Guid id)
    {
        var r = await _service.GetRegistrationByIdAsync(id);
        if (r == null) return NotFound(new { code = 404, message = "使用登记记录不存在" });
        return Ok(new { code = 200, data = r });
    }

    [HttpPost]
    public async Task<ActionResult<UsageRegistrationDto>> CreateRegistration([FromBody] CreateUsageRegistrationRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var createdBy = User.Identity?.Name;
        var r = await _service.CreateRegistrationAsync(request, userId, userName, createdBy);
        return Ok(new { code = 200, data = r, message = "使用登记成功" });
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<UsageRegistrationDto>>> GetPending([FromQuery] Guid? userId, [FromQuery] Guid? semesterId)
    {
        var list = await _service.GetPendingRegistrationsAsync(userId, semesterId);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<UsageRegistrationDto>>> GetOverdue([FromQuery] Guid? semesterId)
    {
        var list = await _service.GetOverdueRegistrationsAsync(semesterId);
        return Ok(new { code = 200, data = list });
    }

    [HttpPut("{id}/remind")]
    public IActionResult Remind(Guid id)
    {
        return Ok(new { code = 200, message = "提醒已发送" });
    }

    [HttpGet("statistics/completion")]
    public async Task<ActionResult<CompletionRate>> GetCompletionRate([FromQuery] StatisticsQuery query)
    {
        var statsService = HttpContext.RequestServices.GetService(typeof(IStatisticsService)) as IStatisticsService;
        if (statsService == null) return Ok(new { code = 200, data = new CompletionRate() });

        var rate = await statsService.GetRegistrationCompletionRateAsync(query);
        return Ok(new { code = 200, data = rate });
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
