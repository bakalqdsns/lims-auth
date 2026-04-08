using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/majors")]
[Authorize]
public class MajorsController : ControllerBase
{
    private readonly IMajorService _majorService;

    public MajorsController(IMajorService majorService)
    {
        _majorService = majorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] string? departmentId)
    {
        var majors = await _majorService.GetListAsync(keyword, departmentId);
        return Ok(new { code = 200, data = majors });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var majors = await _majorService.GetAllAsync();
        return Ok(new { code = 200, data = majors });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var major = await _majorService.GetByIdAsync(id);
        if (major == null)
            return NotFound(new { code = 404, message = "专业不存在" });
        return Ok(new { code = 200, data = major });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMajorRequest request)
    {
        var major = await _majorService.CreateAsync(request);
        return Ok(new { code = 200, data = major, message = "创建成功" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMajorRequest request)
    {
        var major = await _majorService.UpdateAsync(id, request);
        if (major == null)
            return NotFound(new { code = 404, message = "专业不存在" });
        return Ok(new { code = 200, data = major, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _majorService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "专业不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:major:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _majorService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "专业不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}
