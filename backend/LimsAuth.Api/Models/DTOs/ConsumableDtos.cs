using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models.DTOs;

// ============================================================
// 耗材分类 DTOs
// ============================================================

public class ConsumableCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConsumableCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Remark { get; set; }
}

public class UpdateConsumableCategoryRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public string? Remark { get; set; }
    public bool? IsActive { get; set; }
}

// ============================================================
// 耗材 DTOs
// ============================================================

public class ConsumableDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Specification { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal AvailableStock { get; set; }
    public decimal LockedStock { get; set; }
    public decimal MinStock { get; set; }
    public string? Location { get; set; }
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal MaxSingleRequest { get; set; }
    public decimal MonthlyLimit { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLowStock { get; set; }
}

public class CreateConsumableRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }

    [MaxLength(200)]
    public string? Specification { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = "个";

    public decimal CurrentStock { get; set; } = 0;
    public decimal MinStock { get; set; } = 0;

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? Supplier { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal MaxSingleRequest { get; set; } = 999999;
    public decimal MonthlyLimit { get; set; } = 999999;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class UpdateConsumableRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }

    [MaxLength(200)]
    public string? Specification { get; set; }

    [MaxLength(20)]
    public string? Unit { get; set; }

    public decimal? MinStock { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? Supplier { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? MaxSingleRequest { get; set; }
    public decimal? MonthlyLimit { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}

public class ConsumableQuery
{
    public string? Keyword { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Supplier { get; set; }
    public bool? IsLowStock { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// 入库记录 DTOs
// ============================================================

public class ConsumableInRecordDto
{
    public Guid Id { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? ConsumableCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount => UnitPrice.HasValue ? UnitPrice.Value * Quantity : null;
    public string? Supplier { get; set; }
    public DateTime InTime { get; set; }
    public Guid HandlerId { get; set; }
    public string? HandlerName { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalRemark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConsumableInRecordRequest
{
    [Required]
    public Guid ConsumableId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    [MaxLength(200)]
    public string? Supplier { get; set; }

    public DateTime? InTime { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class BatchCreateConsumableInRecordRequest
{
    [Required]
    public List<CreateConsumableInRecordRequest> Items { get; set; } = new();
}

public class ConsumableInRecordQuery
{
    public string? Keyword { get; set; }
    public Guid? ConsumableId { get; set; }
    public Guid? HandlerId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ApprovalRequest
{
    public bool Approved { get; set; }
    public string? Comment { get; set; }
}

// ============================================================
// 出库记录 DTOs
// ============================================================

public class ConsumableOutRecordDto
{
    public Guid Id { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? ConsumableCode { get; set; }
    public string? CategoryName { get; set; }
    public decimal Quantity { get; set; }
    public string? UsagePurpose { get; set; }
    public string? UsageLab { get; set; }
    public DateTime OutTime { get; set; }
    public Guid ApplicantId { get; set; }
    public string? ApplicantName { get; set; }
    public string? Remark { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalRemark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConsumableOutRecordRequest
{
    [Required]
    public Guid ConsumableId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [MaxLength(500)]
    public string? UsagePurpose { get; set; }

    [MaxLength(200)]
    public string? UsageLab { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class BatchCreateConsumableOutRecordRequest
{
    [Required]
    public List<CreateConsumableOutRecordRequest> Items { get; set; } = new();
}

public class ConsumableOutRecordQuery
{
    public string? Keyword { get; set; }
    public Guid? ConsumableId { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// 库存调整 DTOs
// ============================================================

public class ConsumableStockAdjustmentDto
{
    public Guid Id { get; set; }
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? ConsumableCode { get; set; }
    public string AdjustmentType { get; set; } = string.Empty;
    public decimal BeforeQuantity { get; set; }
    public decimal AdjustmentQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public string? Reason { get; set; }
    public Guid OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConsumableStockAdjustmentRequest
{
    [Required]
    public Guid ConsumableId { get; set; }

    [Required]
    [MaxLength(20)]
    public string AdjustmentType { get; set; } = string.Empty;

    [Required]
    public decimal AdjustmentQuantity { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}

public class StockCheckRequest
{
    public List<StockCheckItem> Items { get; set; } = new();
}

public class StockCheckItem
{
    public Guid ConsumableId { get; set; }
    public decimal ActualQuantity { get; set; }
}

public class ConsumableStockAdjustmentQuery
{
    public Guid? ConsumableId { get; set; }
    public string? AdjustmentType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// 库存日志 DTOs
// ============================================================

public class ConsumableStockLogDto
{
    public Guid Id { get; set; }
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public decimal ChangeQuantity { get; set; }
    public decimal BeforeStock { get; set; }
    public decimal AfterStock { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConsumableStockLogQuery
{
    public Guid? ConsumableId { get; set; }
    public string? ChangeType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// 通知消息 DTOs
// ============================================================

public class ConsumableNotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public Guid? RelatedId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConsumableNotificationQuery
{
    public bool? IsRead { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// 数据统计 DTOs
// ============================================================

public class ConsumableStatisticsDto
{
    public int TotalTypes { get; set; }
    public int LowStockTypes { get; set; }
    public int OutOfStockTypes { get; set; }
    public int ActiveTypes { get; set; }
    public decimal TotalStockValue { get; set; }
    public int TotalInRecords { get; set; }
    public int TotalOutRecords { get; set; }
    public decimal TotalInAmount { get; set; }
    public decimal TotalOutAmount { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public List<LowStockItem> LowStockItems { get; set; } = new();
    public List<MonthlyConsumption> MonthlyConsumptions { get; set; } = new();
}

public class LowStockItem
{
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinStock { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class MonthlyConsumption
{
    public string Month { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int RecordCount { get; set; }
}

public class ConsumableStatisticsQuery
{
    public Guid? CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ImportConsumableResult
{
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Total { get; set; }
    public List<string> Errors { get; set; } = new();
}
