namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 设备管理实体
// ============================================================

/// <summary>
/// 设备资产台账实体 (Dev_Asset)
/// </summary>
[EntityTypeConfiguration(typeof(DevAssetConfiguration))]
public class DevAsset
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ModelNumber { get; set; }
    public string? Specification { get; set; }
    public string? Category { get; set; }
    public string Unit { get; set; } = "台";
    public DateTime? PurchaseDate { get; set; }
    public string? Brand { get; set; }
    public string? SerialNumber { get; set; }
    public decimal? Price { get; set; }
    public string? FundingSource { get; set; }
    public int? ServiceLife { get; set; }
    public string? Supplier { get; set; }
    public DateTime? WarrantyPeriod { get; set; }
    public string? StorageLocation { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public int IsImportant { get; set; } = 0;
    public string? LabelInfo { get; set; }
    public string? PhotoPath { get; set; }
    public string DeviceStatus { get; set; } = "Available";
    public int TotalQuantity { get; set; } = 1;
    public int AvailableQuantity { get; set; } = 1;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysUser? ResponsibleUser { get; set; }
    public SysDepartment? Department { get; set; }
    public ICollection<DevLoanApply> LoanApplies { get; set; } = new List<DevLoanApply>();
}

/// <summary>
/// 设备借还申请实体 (Dev_LoanApply)
/// </summary>
[EntityTypeConfiguration(typeof(DevLoanApplyConfiguration))]
public class DevLoanApply
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public Guid BorrowerId { get; set; }
    public string BorrowerType { get; set; } = string.Empty;
    public string? LoanPurpose { get; set; }
    public int LoanQuantity { get; set; } = 1;
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualLoanTime { get; set; }
    public DateTime? ActualReturnTime { get; set; }
    public string AuditStatus { get; set; } = "Pending";
    public Guid? AuditorId { get; set; }
    public DateTime? AuditTime { get; set; }
    public string? AuditOpinion { get; set; }
    public Guid? ReturnConfirmUserId { get; set; }
    public string? DeviceCondition { get; set; }
    public string? Remark { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public DevAsset Asset { get; set; } = null!;
    public SysUser Borrower { get; set; } = null!;
    public SysUser? Auditor { get; set; }
    public SysUser? ReturnConfirmUser { get; set; }
}
