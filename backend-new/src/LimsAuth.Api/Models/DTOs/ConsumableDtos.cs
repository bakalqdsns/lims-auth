using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 耗材基础 DTO
// ============================================================

public class CreateConsumableRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? SpecModel { get; set; }
    [MaxLength(20)]
    public string Unit { get; set; } = "个";
    public decimal CurrentStock { get; set; } = 0;
    public decimal MinStockThreshold { get; set; } = 0;
    [MaxLength(200)]
    public string? StorageLocation { get; set; }
    [MaxLength(200)]
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public int Status { get; set; } = 1;
}

public class UpdateConsumableRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(50)]
    public string? Category { get; set; }
    [MaxLength(200)]
    public string? SpecModel { get; set; }
    [MaxLength(20)]
    public string? Unit { get; set; }
    public decimal? MinStockThreshold { get; set; }
    public decimal? CurrentStock { get; set; }
    [MaxLength(200)]
    public string? StorageLocation { get; set; }
    [MaxLength(200)]
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? Status { get; set; }
}

public class ConsumableDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SpecModel { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal AvailableStock { get; set; }
    public decimal LockedStock { get; set; }
    public decimal MinStockThreshold { get; set; }
    public string? StorageLocation { get; set; }
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public int Status { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConsumableQueryRequest : PagedQueryRequest
{
    public string? Category { get; set; }
    public string? Supplier { get; set; }
    public bool? IsLowStock { get; set; }
}

public class ConsumableStatisticsDto
{
    public int TotalTypes { get; set; }
    public int LowStockTypes { get; set; }
    public int OutOfStockTypes { get; set; }
    public decimal TotalStockValue { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
}

public class LowStockItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinStockThreshold { get; set; }
    public string Unit { get; set; } = string.Empty;
}

// ============================================================
// 入库单 DTO
// ============================================================

public class CreateInboundRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    public Guid ConsumableId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    [MaxLength(200)]
    public string? Supplier { get; set; }
    public DateTime? InboundTime { get; set; }
    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class InboundDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? ConsumableCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Supplier { get; set; }
    public DateTime InboundTime { get; set; }
    public Guid HandlerId { get; set; }
    public string HandlerName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? AuditorId { get; set; }
    public string? AuditorName { get; set; }
    public DateTime? AuditTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InboundQueryRequest : PagedQueryRequest
{
    public Guid? ConsumableId { get; set; }
    public Guid? HandlerId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ============================================================
// 出库单 DTO
// ============================================================

public class CreateOutboundRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    public Guid ConsumableId { get; set; }
    public decimal Quantity { get; set; }
    [MaxLength(1000)]
    public string? Purpose { get; set; }
    public Guid? TargetRoomId { get; set; }
    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class OutboundDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public string ConsumableName { get; set; } = string.Empty;
    public string? ConsumableCode { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Purpose { get; set; }
    public Guid? TargetRoomId { get; set; }
    public string? TargetRoomName { get; set; }
    public DateTime OutTime { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? AuditorId { get; set; }
    public string? AuditorName { get; set; }
    public DateTime? AuditTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OutboundQueryRequest : PagedQueryRequest
{
    public Guid? ConsumableId { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ============================================================
// 库存调整 DTO
// ============================================================

public class CreateStockAdjustmentRequest
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

public class StockAdjustmentDto
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
    public string OperatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class StockAdjustmentQueryRequest : PagedQueryRequest
{
    public Guid? ConsumableId { get; set; }
    public string? AdjustmentType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ============================================================
// 库存日志 DTO
// ============================================================

public class StockLogDto
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
    public string OperatorName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StockLogQueryRequest : PagedQueryRequest
{
    public Guid? ConsumableId { get; set; }
    public string? ChangeType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ============================================================
// 通用审核 DTO
// ============================================================

public class ApprovalRequest
{
    [Required]
    public bool Approved { get; set; }
    [MaxLength(500)]
    public string? Comment { get; set; }
}
