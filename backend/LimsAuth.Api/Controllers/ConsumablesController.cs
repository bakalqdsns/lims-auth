using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/consumables")]
[Authorize]
public class ConsumablesController : ControllerBase
{
    private readonly IConsumableService _service;

    public ConsumablesController(IConsumableService service)
    {
        _service = service;
    }

    #region 耗材分类

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _service.GetCategoriesAsync();
        return Ok(new { code = 200, data = categories });
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _service.GetCategoryByIdAsync(id);
        if (category == null) return NotFound(new { code = 404, message = "分类不存在" });
        return Ok(new { code = 200, data = category });
    }

    [HttpPost("categories")]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateConsumableCategoryRequest request)
    {
        try
        {
            var category = await _service.CreateCategoryAsync(request);
            return Ok(new { code = 200, data = category, message = "分类创建成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpPut("categories/{id}")]
    [Authorize(Policy = "Permission:consumable:update")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateConsumableCategoryRequest request)
    {
        var category = await _service.UpdateCategoryAsync(id, request);
        if (category == null) return NotFound(new { code = 404, message = "分类不存在" });
        return Ok(new { code = 200, data = category, message = "分类更新成功" });
    }

    [HttpDelete("categories/{id}")]
    [Authorize(Policy = "Permission:consumable:delete")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        try
        {
            var result = await _service.DeleteCategoryAsync(id);
            if (!result) return NotFound(new { code = 404, message = "分类不存在" });
            return Ok(new { code = 200, message = "分类删除成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    #endregion

    #region 耗材基础信息

    [HttpGet]
    public async Task<IActionResult> GetConsumables([FromQuery] ConsumableQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var (items, total) = await _service.GetConsumablesAsync(query);
        return Ok(new { code = 200, data = items, total });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsumableById(Guid id)
    {
        var consumable = await _service.GetConsumableByIdAsync(id);
        if (consumable == null) return NotFound(new { code = 404, message = "耗材不存在" });
        return Ok(new { code = 200, data = consumable });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> CreateConsumable([FromBody] CreateConsumableRequest request)
    {
        try
        {
            var consumable = await _service.CreateConsumableAsync(request);
            return Ok(new { code = 200, data = consumable, message = "耗材创建成功" });
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

    [HttpPost("import")]
    [Authorize(Policy = "Permission:consumable:create")]
    public async Task<IActionResult> ImportConsumables(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { code = 400, message = "请上传文件" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".xlsx" && ext != ".xls")
            return BadRequest(new { code = 400, message = "仅支持 .xlsx 和 .xls 格式" });

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _service.ImportConsumablesAsync(stream);
            return Ok(new { code = 200, message = $"导入完成：成功 {result.Success} 条，失败 {result.Failed} 条", data = result });
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return Ok(new { code = 200, message = "导入失败：" + msg, data = new ImportConsumableResult { Success = 0, Failed = 0, Total = 0, Errors = new List<string> { msg } } });
        }
    }

    [HttpGet("import-template")]
    public async Task<IActionResult> GetConsumableImportTemplate()
    {
        var bytes = await _service.GenerateConsumableImportTemplateAsync();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "耗材导入模板.xlsx");
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:consumable:update")]
    public async Task<IActionResult> UpdateConsumable(Guid id, [FromBody] UpdateConsumableRequest request)
    {
        try
        {
            var consumable = await _service.UpdateConsumableAsync(id, request);
            if (consumable == null) return NotFound(new { code = 404, message = "耗材不存在" });
            return Ok(new { code = 200, data = consumable, message = "耗材更新成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:consumable:delete")]
    public async Task<IActionResult> DeleteConsumable(Guid id)
    {
        var result = await _service.DeleteConsumableAsync(id);
        if (!result) return NotFound(new { code = 404, message = "耗材不存在" });
        return Ok(new { code = 200, message = "耗材删除成功" });
    }

    #endregion

    #region 入库管理

    [HttpGet("in-records")]
    public async Task<IActionResult> GetInRecords([FromQuery] ConsumableInRecordQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var (items, total) = await _service.GetInRecordsAsync(query);
        return Ok(new { code = 200, data = items, total });
    }

    [HttpPost("in-records")]
    [Authorize(Policy = "Permission:consumable:in")]
    public async Task<IActionResult> CreateInRecord([FromBody] CreateConsumableInRecordRequest request)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();
            var record = await _service.CreateInRecordAsync(request, userId, userName);
            return Ok(new { code = 200, data = record, message = "入库申请提交成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { code = 500, message = "服务器内部错误: " + (ex.InnerException?.Message ?? ex.Message) });
        }
    }

    [HttpPost("in-records/batch")]
    [Authorize(Policy = "Permission:consumable:in")]
    public async Task<IActionResult> BatchCreateInRecords([FromBody] BatchCreateConsumableInRecordRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var count = await _service.BatchCreateInRecordsAsync(request, userId, userName);
        return Ok(new { code = 200, message = $"批量入库完成，成功 {count} 条" });
    }

    [HttpPost("in-records/{id}/approve")]
    [Authorize(Policy = "Permission:consumable:approve")]
    public async Task<IActionResult> ApproveInRecord(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var approverName = GetUserName();
        var result = await _service.ApproveInRecordAsync(id, request.Approved, request.Comment, approverId, approverName);
        if (!result) return NotFound(new { code = 404, message = "入库记录不存在或已审批" });
        return Ok(new { code = 200, message = request.Approved ? "入库审批通过" : "入库审批驳回" });
    }

    [HttpDelete("in-records/{id}")]
    [Authorize(Policy = "Permission:consumable:delete")]
    public async Task<IActionResult> DeleteInRecord(Guid id)
    {
        var result = await _service.DeleteInRecordAsync(id);
        if (!result) return NotFound(new { code = 404, message = "入库记录不存在" });
        return Ok(new { code = 200, message = "入库记录删除成功" });
    }

    #endregion

    #region 出库管理

    [HttpGet("out-records")]
    public async Task<IActionResult> GetOutRecords([FromQuery] ConsumableOutRecordQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var (items, total) = await _service.GetOutRecordsAsync(query);
        return Ok(new { code = 200, data = items, total });
    }

    [HttpGet("out-records/my")]
    public async Task<IActionResult> GetMyOutRecords([FromQuery] ConsumableOutRecordQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var userId = GetUserId();
        var (items, total) = await _service.GetMyOutRecordsAsync(userId, query);
        return Ok(new { code = 200, data = items, total });
    }

    [HttpPost("out-records")]
    public async Task<IActionResult> CreateOutRecord([FromBody] CreateConsumableOutRecordRequest request)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();
            var record = await _service.CreateOutRecordAsync(request, userId, userName);
            return Ok(new { code = 200, data = record, message = "领用申请提交成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { code = 500, message = "服务器内部错误: " + (ex.InnerException?.Message ?? ex.Message) });
        }
    }

    [HttpPost("out-records/batch")]
    public async Task<IActionResult> BatchCreateOutRecords([FromBody] BatchCreateConsumableOutRecordRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var count = await _service.BatchCreateOutRecordsAsync(request, userId, userName);
        return Ok(new { code = 200, message = $"批量领用申请提交完成，成功 {count} 条" });
    }

    [HttpPost("out-records/{id}/approve")]
    [Authorize(Policy = "Permission:consumable:approve")]
    public async Task<IActionResult> ApproveOutRecord(Guid id, [FromBody] ApprovalRequest request)
    {
        var approverId = GetUserId();
        var approverName = GetUserName();
        var result = await _service.ApproveOutRecordAsync(id, request.Approved, request.Comment, approverId, approverName);
        if (!result) return NotFound(new { code = 404, message = "领用记录不存在或已审批" });
        return Ok(new { code = 200, message = request.Approved ? "领用审批通过" : "领用审批驳回" });
    }

    [HttpDelete("out-records/{id}")]
    public async Task<IActionResult> DeleteOutRecord(Guid id)
    {
        var userId = GetUserId();
        var record = await _service.GetOutRecordsAsync(new ConsumableOutRecordQuery { Page = 1, PageSize = 1 });
        var result = await _service.DeleteOutRecordAsync(id);
        if (!result) return NotFound(new { code = 404, message = "领用记录不存在" });
        return Ok(new { code = 200, message = "领用记录删除成功" });
    }

    #endregion

    #region 库存管理

    [HttpPost("stock/adjust")]
    [Authorize(Policy = "Permission:consumable:adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] CreateConsumableStockAdjustmentRequest request)
    {
        try
        {
            var userId = GetUserId();
            var userName = GetUserName();
            var adjustment = await _service.AdjustStockAsync(request, userId, userName);
            return Ok(new { code = 200, data = adjustment, message = "库存调整成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpPost("stock/check")]
    [Authorize(Policy = "Permission:consumable:adjust")]
    public async Task<IActionResult> StockCheck([FromBody] StockCheckRequest request)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var count = await _service.StockCheckAsync(request, userId, userName);
        return Ok(new { code = 200, message = $"库存盘点完成，调整 {count} 条" });
    }

    [HttpGet("stock/logs")]
    public async Task<IActionResult> GetStockLogs([FromQuery] ConsumableStockLogQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var (items, total) = await _service.GetStockLogsAsync(query);
        return Ok(new { code = 200, data = items, total });
    }

    [HttpGet("stock/adjustments")]
    public async Task<IActionResult> GetStockAdjustments([FromQuery] ConsumableStockAdjustmentQuery query)
    {
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var (items, total) = await _service.GetStockAdjustmentsAsync(query);
        return Ok(new { code = 200, data = items, total });
    }

    #endregion

    #region 数据统计

    [HttpGet("statistics")]
    [Authorize(Policy = "Permission:consumable:statistics")]
    public async Task<IActionResult> GetStatistics([FromQuery] ConsumableStatisticsQuery? query)
    {
        var stats = await _service.GetStatisticsAsync(query);
        return Ok(new { code = 200, data = stats });
    }

    #endregion

    #region 消息通知

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] ConsumableNotificationQuery query)
    {
        var userId = GetUserId();
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1) query.PageSize = 20;
        if (query.PageSize > 200) query.PageSize = 200;

        var notifications = await _service.GetNotificationsAsync(userId, query);
        return Ok(new { code = 200, data = notifications });
    }

    [HttpGet("notifications/unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetUserId();
        var count = await _service.GetUnreadNotificationCountAsync(userId);
        return Ok(new { code = 200, data = count });
    }

    [HttpPut("notifications/{id}/read")]
    public async Task<IActionResult> MarkNotificationRead(Guid id)
    {
        var userId = GetUserId();
        var result = await _service.MarkNotificationReadAsync(id, userId);
        if (!result) return NotFound(new { code = 404, message = "通知不存在" });
        return Ok(new { code = 200, message = "标记已读成功" });
    }

    #endregion

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
