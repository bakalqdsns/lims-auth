using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:asset:read")]
    public async Task<IActionResult> GetList([FromQuery] AssetQueryRequest query)
    {
        var result = await _assetService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:asset:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _assetService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:asset:create")]
    public async Task<IActionResult> Create([FromBody] CreateAssetRequest request)
    {
        var result = await _assetService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:asset:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetRequest request)
    {
        var result = await _assetService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:asset:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _assetService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("statistics")]
    [Authorize(Policy = "Permission:asset:read")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _assetService.GetStatisticsAsync();
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/loan-applies")]
[Authorize]
public class LoanAppliesController : ControllerBase
{
    private readonly ILoanApplyService _loanApplyService;

    public LoanAppliesController(ILoanApplyService loanApplyService)
    {
        _loanApplyService = loanApplyService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:loan:read")]
    public async Task<IActionResult> GetList([FromQuery] LoanApplyQueryRequest query)
    {
        var result = await _loanApplyService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:loan:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _loanApplyService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:loan:create")]
    public async Task<IActionResult> Create([FromBody] CreateLoanApplyRequest request)
    {
        var result = await _loanApplyService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/audit")]
    [Authorize(Policy = "Permission:loan:approve")]
    public async Task<IActionResult> Audit(Guid id, [FromBody] AuditLoanRequest request)
    {
        var result = await _loanApplyService.AuditAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/confirm-return")]
    [Authorize(Policy = "Permission:loan:approve")]
    public async Task<IActionResult> ConfirmReturn(Guid id, [FromBody] ConfirmReturnRequest request)
    {
        var result = await _loanApplyService.ConfirmReturnAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/renew")]
    [Authorize(Policy = "Permission:loan:create")]
    public async Task<IActionResult> Renew(Guid id, [FromBody] RenewLoanRequest request)
    {
        var result = await _loanApplyService.RenewAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
