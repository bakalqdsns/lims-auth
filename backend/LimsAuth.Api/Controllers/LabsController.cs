using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/labs")]
[Authorize]
public class LabsController : ControllerBase
{
    private readonly ILabService _labService;

    public LabsController(ILabService labService)
    {
        _labService = labService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] string? departmentId, [FromQuery] string? labType)
    {
        var labs = await _labService.GetListAsync(keyword, departmentId, labType);
        return Ok(new { code = 200, data = labs });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var lab = await _labService.GetByIdAsync(id);
        if (lab == null)
            return NotFound(new { code = 404, message = "实验室不存在" });
        return Ok(new { code = 200, data = lab });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:lab:create")]
    public async Task<IActionResult> Create([FromBody] CreateLabRequest request)
    {
        var lab = await _labService.CreateAsync(request);
        return Ok(new { code = 200, data = lab, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:lab:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLabRequest request)
    {
        var lab = await _labService.UpdateAsync(id, request);
        if (lab == null)
            return NotFound(new { code = 404, message = "实验室不存在" });
        return Ok(new { code = 200, data = lab, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:lab:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _labService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "实验室不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:lab:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _labService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "实验室不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}


