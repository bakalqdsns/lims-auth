using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 耗材分类
/// </summary>
[Table("consumable_categories")]
public class ConsumableCategory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual ICollection<Consumable> Consumables { get; set; } = new List<Consumable>();
}

/// <summary>
/// 耗材基础信息
/// </summary>
[Table("consumables")]
public class Consumable
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("code")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("specification")]
    [MaxLength(200)]
    public string? Specification { get; set; }

    [Column("unit")]
    [MaxLength(20)]
    public string Unit { get; set; } = "个";

    [Column("current_stock")]
    public decimal CurrentStock { get; set; } = 0;

    [Column("available_stock")]
    public decimal AvailableStock { get; set; } = 0;

    [Column("locked_stock")]
    public decimal LockedStock { get; set; } = 0;

    [Column("min_stock")]
    public decimal MinStock { get; set; } = 0;

    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    [Column("supplier")]
    [MaxLength(200)]
    public string? Supplier { get; set; }

    [Column("unit_price")]
    public decimal? UnitPrice { get; set; }

    [Column("max_single_request")]
    public decimal MaxSingleRequest { get; set; } = 999999;

    [Column("monthly_limit")]
    public decimal MonthlyLimit { get; set; } = 999999;

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual ConsumableCategory? Category { get; set; }
    public virtual ICollection<ConsumableInRecord> InRecords { get; set; } = new List<ConsumableInRecord>();
    public virtual ICollection<ConsumableOutRecord> OutRecords { get; set; } = new List<ConsumableOutRecord>();
}

/// <summary>
/// 入库记录
/// </summary>
[Table("consumable_in_records")]
public class ConsumableInRecord
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("record_no")]
    [MaxLength(50)]
    public string RecordNo { get; set; } = string.Empty;

    [Column("consumable_id")]
    public Guid ConsumableId { get; set; }

    [Column("quantity")]
    public decimal Quantity { get; set; }

    [Column("unit_price")]
    public decimal? UnitPrice { get; set; }

    [Column("supplier")]
    [MaxLength(200)]
    public string? Supplier { get; set; }

    [Column("in_time")]
    public DateTime InTime { get; set; } = DateTime.UtcNow;

    [Column("handler_id")]
    public Guid HandlerId { get; set; }

    [Column("handler_name")]
    [MaxLength(100)]
    public string? HandlerName { get; set; }

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approver_name")]
    [MaxLength(100)]
    public string? ApproverName { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("approval_remark")]
    [MaxLength(500)]
    public string? ApprovalRemark { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Consumable? Consumable { get; set; }
    public virtual User? Handler { get; set; }
}

/// <summary>
/// 出库记录（领用申请）
/// </summary>
[Table("consumable_out_records")]
public class ConsumableOutRecord
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("record_no")]
    [MaxLength(50)]
    public string RecordNo { get; set; } = string.Empty;

    [Column("consumable_id")]
    public Guid ConsumableId { get; set; }

    [Column("quantity")]
    public decimal Quantity { get; set; }

    [Column("usage_purpose")]
    [MaxLength(500)]
    public string? UsagePurpose { get; set; }

    [Column("usage_lab")]
    [MaxLength(200)]
    public string? UsageLab { get; set; }

    [Column("out_time")]
    public DateTime OutTime { get; set; } = DateTime.UtcNow;

    [Column("applicant_id")]
    public Guid ApplicantId { get; set; }

    [Column("applicant_name")]
    [MaxLength(100)]
    public string? ApplicantName { get; set; }

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approver_name")]
    [MaxLength(100)]
    public string? ApproverName { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("approval_remark")]
    [MaxLength(500)]
    public string? ApprovalRemark { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Consumable? Consumable { get; set; }
    public virtual User? Applicant { get; set; }
}

/// <summary>
/// 库存调整记录
/// </summary>
[Table("consumable_stock_adjustments")]
public class ConsumableStockAdjustment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("consumable_id")]
    public Guid ConsumableId { get; set; }

    [Column("adjustment_type")]
    [MaxLength(20)]
    public string AdjustmentType { get; set; } = string.Empty;

    [Column("before_quantity")]
    public decimal BeforeQuantity { get; set; }

    [Column("adjustment_quantity")]
    public decimal AdjustmentQuantity { get; set; }

    [Column("after_quantity")]
    public decimal AfterQuantity { get; set; }

    [Column("reason")]
    [MaxLength(500)]
    public string? Reason { get; set; }

    [Column("operator_id")]
    public Guid OperatorId { get; set; }

    [Column("operator_name")]
    [MaxLength(100)]
    public string? OperatorName { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Consumable? Consumable { get; set; }
}

/// <summary>
/// 库存变动日志
/// </summary>
[Table("consumable_stock_logs")]
public class ConsumableStockLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("consumable_id")]
    public Guid ConsumableId { get; set; }

    [Column("change_type")]
    [MaxLength(30)]
    public string ChangeType { get; set; } = string.Empty;

    [Column("change_quantity")]
    public decimal ChangeQuantity { get; set; }

    [Column("before_stock")]
    public decimal BeforeStock { get; set; }

    [Column("after_stock")]
    public decimal AfterStock { get; set; }

    [Column("reference_id")]
    public Guid? ReferenceId { get; set; }

    [Column("reference_no")]
    [MaxLength(50)]
    public string? ReferenceNo { get; set; }

    [Column("operator_id")]
    public Guid OperatorId { get; set; }

    [Column("operator_name")]
    [MaxLength(100)]
    public string? OperatorName { get; set; }

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Consumable? Consumable { get; set; }
}

/// <summary>
/// 耗材通知消息
/// </summary>
[Table("consumable_notifications")]
public class ConsumableNotification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("type")]
    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    [Column("title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("content")]
    [MaxLength(1000)]
    public string? Content { get; set; }

    [Column("related_id")]
    public Guid? RelatedId { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual User? User { get; set; }
}
