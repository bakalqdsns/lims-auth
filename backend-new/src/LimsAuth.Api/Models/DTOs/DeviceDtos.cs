using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 设备资产 DTO
// ============================================================

public class CreateAssetRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? ModelNumber { get; set; }
    [MaxLength(200)]
    public string? Specification { get; set; }
    [MaxLength(50)]
    public string? Category { get; set; }
    [MaxLength(20)]
    public string Unit { get; set; } = "台";
    public DateTime? PurchaseDate { get; set; }
    [MaxLength(100)]
    public string? Brand { get; set; }
    [MaxLength(100)]
    public string? SerialNumber { get; set; }
    public decimal? Price { get; set; }
    [MaxLength(100)]
    public string? FundingSource { get; set; }
    public int? ServiceLife { get; set; }
    [MaxLength(200)]
    public string? Supplier { get; set; }
    public DateTime? WarrantyPeriod { get; set; }
    [MaxLength(200)]
    public string? StorageLocation { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public int IsImportant { get; set; } = 0;
    [MaxLength(200)]
    public string? LabelInfo { get; set; }
    [MaxLength(1000)]
    public string? PhotoPath { get; set; }
    [MaxLength(50)]
    public string DeviceStatus { get; set; } = "Available";
    public int TotalQuantity { get; set; } = 1;
    public int AvailableQuantity { get; set; } = 1;
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateAssetRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(100)]
    public string? ModelNumber { get; set; }
    [MaxLength(200)]
    public string? Specification { get; set; }
    [MaxLength(50)]
    public string? Category { get; set; }
    [MaxLength(20)]
    public string? Unit { get; set; }
    public DateTime? PurchaseDate { get; set; }
    [MaxLength(100)]
    public string? Brand { get; set; }
    [MaxLength(100)]
    public string? SerialNumber { get; set; }
    public decimal? Price { get; set; }
    [MaxLength(100)]
    public string? FundingSource { get; set; }
    public int? ServiceLife { get; set; }
    [MaxLength(200)]
    public string? Supplier { get; set; }
    public DateTime? WarrantyPeriod { get; set; }
    [MaxLength(200)]
    public string? StorageLocation { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public int? IsImportant { get; set; }
    [MaxLength(200)]
    public string? LabelInfo { get; set; }
    [MaxLength(1000)]
    public string? PhotoPath { get; set; }
    [MaxLength(50)]
    public string? DeviceStatus { get; set; }
    public int? TotalQuantity { get; set; }
    public int? AvailableQuantity { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class AssetDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ModelNumber { get; set; }
    public string? Specification { get; set; }
    public string? Category { get; set; }
    public string Unit { get; set; } = string.Empty;
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
    public string? ResponsibleUserName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int IsImportant { get; set; }
    public string? LabelInfo { get; set; }
    public string? PhotoPath { get; set; }
    public string DeviceStatus { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AssetQueryRequest : PagedQueryRequest
{
    public Guid? DepartmentId { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public string? Category { get; set; }
    public string? DeviceStatus { get; set; }
    public int? IsImportant { get; set; }
}

public class AssetStatisticsDto
{
    public int TotalAssets { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int BorrowedQuantity { get; set; }
    public int MaintenanceQuantity { get; set; }
    public int ScrapQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
}

// ============================================================
// 设备借还 DTO
// ============================================================

public class CreateLoanApplyRequest
{
    [Required]
    public Guid AssetId { get; set; }
    public Guid BorrowerId { get; set; }
    [MaxLength(20)]
    public string BorrowerType { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? LoanPurpose { get; set; }
    public int LoanQuantity { get; set; } = 1;
    public DateTime ExpectedReturnDate { get; set; }
}

public class LoanApplyDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public Guid BorrowerId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string BorrowerType { get; set; } = string.Empty;
    public string? LoanPurpose { get; set; }
    public int LoanQuantity { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualLoanTime { get; set; }
    public DateTime? ActualReturnTime { get; set; }
    public string AuditStatus { get; set; } = string.Empty;
    public Guid? AuditorId { get; set; }
    public string? AuditorName { get; set; }
    public DateTime? AuditTime { get; set; }
    public string? AuditOpinion { get; set; }
    public Guid? ReturnConfirmUserId { get; set; }
    public string? ReturnConfirmUserName { get; set; }
    public string? DeviceCondition { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoanApplyQueryRequest : PagedQueryRequest
{
    public Guid? AssetId { get; set; }
    public Guid? BorrowerId { get; set; }
    public string? AuditStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class AuditLoanRequest
{
    [Required]
    public bool Approved { get; set; }
    [MaxLength(500)]
    public string? Opinion { get; set; }
}

public class ConfirmReturnRequest
{
    [Required]
    [MaxLength(50)]
    public string DeviceCondition { get; set; } = "完好";
    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class RenewLoanRequest
{
    public DateTime NewReturnDate { get; set; }
}
