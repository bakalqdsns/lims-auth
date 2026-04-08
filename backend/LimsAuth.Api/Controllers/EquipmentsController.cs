using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/equipments")]
[Authorize]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentsController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] string? labId, [FromQuery] string? category, [FromQuery] string? status)
    {
        var equipments = await _equipmentService.GetListAsync(keyword, labId, category, status);
        return Ok(new { code = 200, data = equipments });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var equipment = await _equipmentService.GetByIdAsync(id);
        if (equipment == null)
            return NotFound(new { code = 404, message = "设备不存在" });
        return Ok(new { code = 200, data = equipment });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:equipment:create")]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentRequest request)
    {
        var equipment = await _equipmentService.CreateAsync(request);
        return Ok(new { code = 200, data = equipment, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:equipment:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEquipmentRequest request)
    {
        var equipment = await _equipmentService.UpdateAsync(id, request);
        if (equipment == null)
            return NotFound(new { code = 404, message = "设备不存在" });
        return Ok(new { code = 200, data = equipment, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:equipment:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _equipmentService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "设备不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:equipment:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _equipmentService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "设备不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }

    [HttpPatch("{id}/equipment-status")]
    [Authorize(Policy = "Permission:equipment:update")]
    public async Task<IActionResult> UpdateEquipmentStatus(Guid id, [FromBody] UpdateEquipmentStatusRequest request)
    {
        var result = await _equipmentService.UpdateStatusAsync(id, request.Status);
        if (!result)
            return NotFound(new { code = 404, message = "设备不存在" });
        return Ok(new { code = 200, message = "设备状态更新成功" });
    }
}

public class UpdateEquipmentStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
