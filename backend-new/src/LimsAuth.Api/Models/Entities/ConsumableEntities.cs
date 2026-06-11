namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 耗材管理实体
// ============================================================

/// <summary>
/// 耗材基础实体 (Mat_Consumable)
/// </summary>
[EntityTypeConfiguration(typeof(MatConsumableConfiguration))]
public class MatConsumable
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SpecModel { get; set; }
    public string Unit { get; set; } = "个";
    public decimal CurrentStock { get; set; }
    public decimal AvailableStock { get; set; }
    public decimal LockedStock { get; set; }
    public decimal MinStockThreshold { get; set; }
    public string? StorageLocation { get; set; }
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public int Status { get; set; } = 1;
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ICollection<MatInboundOrder> InboundOrders { get; set; } = new List<MatInboundOrder>();
    public ICollection<MatOutboundOrder> OutboundOrders { get; set; } = new List<MatOutboundOrder>();
    public ICollection<MatStockLog> StockLogs { get; set; } = new List<MatStockLog>();
}

/// <summary>
/// 耗材入库单实体 (Mat_InboundOrder)
/// </summary>
[EntityTypeConfiguration(typeof(MatInboundOrderConfiguration))]
public class MatInboundOrder
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Supplier { get; set; }
    public DateTime InboundTime { get; set; }
    public Guid HandlerId { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? AuditorId { get; set; }
    public DateTime? AuditTime { get; set; }
    public string? AuditRemark { get; set; }
    public string? Remark { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public MatConsumable Consumable { get; set; } = null!;
    public SysUser Handler { get; set; } = null!;
    public SysUser? Auditor { get; set; }
}

/// <summary>
/// 耗材出库单实体 (Mat_OutboundOrder)
/// </summary>
[EntityTypeConfiguration(typeof(MatOutboundOrderConfiguration))]
public class MatOutboundOrder
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ConsumableId { get; set; }
    public decimal Quantity { get; set; }
    public string? Purpose { get; set; }
    public Guid? TargetRoomId { get; set; }
    public Guid ApplicantId { get; set; }
    public DateTime OutTime { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? AuditorId { get; set; }
    public DateTime? AuditTime { get; set; }
    public string? AuditRemark { get; set; }
    public string? Remark { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public MatConsumable Consumable { get; set; } = null!;
    public LabRoom? TargetRoom { get; set; }
    public SysUser Applicant { get; set; } = null!;
    public SysUser? Auditor { get; set; }
}

/// <summary>
/// 库存变动日志实体 (Mat_StockLog)
/// </summary>
[EntityTypeConfiguration(typeof(MatStockLogConfiguration))]
public class MatStockLog
{
    public Guid Id { get; set; }
    public Guid ConsumableId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public decimal BeforeQuantity { get; set; }
    public decimal ChangeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNo { get; set; }
    public Guid OperatorId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MatConsumable Consumable { get; set; } = null!;
    public SysUser Operator { get; set; } = null!;
}
