using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/campuses")]
[Authorize]
public class CampusesController : ControllerBase
{
    private readonly ICampusService _campusService;

    public CampusesController(ICampusService campusService)
    {
        _campusService = campusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword)
    {
        var campuses = await _campusService.GetListAsync(keyword);
        return Ok(new { code = 200, data = campuses });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var campus = await _campusService.GetByIdAsync(id);
        if (campus == null)
            return NotFound(new { code = 404, message = "校区不存在" });
        return Ok(new { code = 200, data = campus });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:campus:create")]
    public async Task<IActionResult> Create([FromBody] CreateCampusRequest request)
    {
        var campus = await _campusService.CreateAsync(request);
        return Ok(new { code = 200, data = campus, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:campus:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCampusRequest request)
    {
        var campus = await _campusService.UpdateAsync(id, request);
        if (campus == null)
            return NotFound(new { code = 404, message = "校区不存在" });
        return Ok(new { code = 200, data = campus, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:campus:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _campusService.DeleteAsync(id);
            if (!result)
                return NotFound(new { code = 404, message = "校区不存在" });
            return Ok(new { code = 200, message = "删除成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:campus:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _campusService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "校区不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}
