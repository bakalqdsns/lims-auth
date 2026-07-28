using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public async Task<IActionResult> GetList(
        [FromQuery] string? keyword,
        [FromQuery] string? labId,
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var (items, total) = await _equipmentService.GetListAsync(keyword, labId, category, status, page, pageSize);
        return Ok(new { code = 200, data = new { items, total, page, pageSize } });
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
        try
        {
            var equipment = await _equipmentService.CreateAsync(request);
            return Ok(new { code = 200, data = equipment, message = "创建成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, new { code = 500, message = "服务器内部错误: " + inner });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:equipment:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEquipmentRequest request)
    {
        try
        {
            var equipment = await _equipmentService.UpdateAsync(id, request);
            if (equipment == null)
                return NotFound(new { code = 404, message = "设备不存在" });
            return Ok(new { code = 200, data = equipment, message = "更新成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, new { code = 500, message = "服务器内部错误: " + inner });
        }
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

    [HttpGet("statistics")]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _equipmentService.GetStatisticsAsync();
        return Ok(new { code = 200, data = stats });
    }

    [HttpGet("export")]
    [Authorize(Policy = "Permission:equipment:read")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] string? status)
    {
        var bytes = await _equipmentService.ExportToExcelAsync(keyword, category, status);
        var fileName = $"设备台账_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost("import")]
    [Authorize(Policy = "Permission:equipment:create")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { code = 400, message = "请上传文件" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".xls")
            return BadRequest(new { code = 400, message = "仅支持 .xlsx 和 .xls 格式" });

        using var stream = file.OpenReadStream();
        try
        {
            var result = await _equipmentService.ImportExcelAsync(stream, file.FileName);

            if (result.Errors.Count > 0 && result.Success == 0)
                return Ok(new { code = 200, message = $"导入完成：成功 {result.Success} 条，失败 {result.Failed} 条", data = result });

            return Ok(new { code = 200, message = $"导入完成：成功 {result.Success} 条，失败 {result.Failed} 条", data = result });
        }
        catch (Exception ex)
        {
            return Ok(new { code = 200, message = "文件解析失败：" + (ex.InnerException?.Message ?? ex.Message), data = new ImportEquipmentResult() });
        }
    }

    [HttpGet("import-template")]
    public async Task<IActionResult> GetImportTemplate()
    {
        var bytes = await _equipmentService.GenerateImportTemplateAsync();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "设备导入模板.xlsx");
    }
}

public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}

public class UpdateEquipmentStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
