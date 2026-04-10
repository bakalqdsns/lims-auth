using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/buildings")]
[Authorize]
public class BuildingsController : ControllerBase
{
    private readonly IBuildingService _buildingService;

    public BuildingsController(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] Guid? campusId)
    {
        var buildings = await _buildingService.GetListAsync(keyword, campusId);
        return Ok(new { code = 200, data = buildings });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var building = await _buildingService.GetByIdAsync(id);
        if (building == null)
            return NotFound(new { code = 404, message = "楼宇不存在" });
        return Ok(new { code = 200, data = building });
    }

    [HttpGet("by-campus/{campusId}")]
    public async Task<IActionResult> GetByCampusId(Guid campusId)
    {
        var buildings = await _buildingService.GetByCampusIdAsync(campusId);
        return Ok(new { code = 200, data = buildings });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:building:create")]
    public async Task<IActionResult> Create([FromBody] CreateBuildingRequest request)
    {
        try
        {
            var building = await _buildingService.CreateAsync(request);
            return Ok(new { code = 200, data = building, message = "创建成功" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:building:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBuildingRequest request)
    {
        var building = await _buildingService.UpdateAsync(id, request);
        if (building == null)
            return NotFound(new { code = 404, message = "楼宇不存在" });
        return Ok(new { code = 200, data = building, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:building:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _buildingService.DeleteAsync(id);
            if (!result)
                return NotFound(new { code = 404, message = "楼宇不存在" });
            return Ok(new { code = 200, message = "删除成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:building:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _buildingService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "楼宇不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}
