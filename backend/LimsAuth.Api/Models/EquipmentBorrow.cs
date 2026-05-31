using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 设备借还记录
/// </summary>
[Table("equipment_borrow_records")]
public class EquipmentBorrowRecord
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 借还单号
    /// </summary>
    [Required]
    [Column("record_no")]
    [MaxLength(50)]
    public string RecordNo { get; set; } = string.Empty;

    /// <summary>
    /// 设备ID
    /// </summary>
    [Column("equipment_id")]
    public Guid EquipmentId { get; set; }

    /// <summary>
    /// 申请人ID
    /// </summary>
    [Column("applicant_id")]
    public Guid ApplicantId { get; set; }

    /// <summary>
    /// 当前审批人ID（导师/管理员）
    /// </summary>
    [Column("approver_id")]
    public Guid? ApproverId { get; set; }

    /// <summary>
    /// 记录状态
    /// </summary>
    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = BorrowStatus.PendingTeacherApproval;

    /// <summary>
    /// 计划借出时间
    /// </summary>
    [Column("borrow_date")]
    public DateTime BorrowDate { get; set; }

    /// <summary>
    /// 计划归还时间
    /// </summary>
    [Column("expected_return_date")]
    public DateTime ExpectedReturnDate { get; set; }

    /// <summary>
    /// 实际借出时间
    /// </summary>
    [Column("actual_borrow_date")]
    public DateTime? ActualBorrowDate { get; set; }

    /// <summary>
    /// 实际归还时间
    /// </summary>
    [Column("actual_return_date")]
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// 借用用途
    /// </summary>
    [Column("purpose")]
    [MaxLength(500)]
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    /// <summary>
    /// 使用地点
    /// </summary>
    [Column("usage_location")]
    [MaxLength(200)]
    public string? UsageLocation { get; set; }

    /// <summary>
    /// 申请人备注
    /// </summary>
    [Column("remarks")]
    [MaxLength(500)]
    public string? Remarks { get; set; }

    // ===== 导师审批 =====
    /// <summary>
    /// 导师审批状态：同意/拒绝
    /// </summary>
    [Column("supervisor_approval_status")]
    [MaxLength(20)]
    public string? SupervisorApprovalStatus { get; set; }

    /// <summary>
    /// 导师审批备注
    /// </summary>
    [Column("supervisor_approval_remark")]
    [MaxLength(500)]
    public string? SupervisorApprovalRemark { get; set; }

    /// <summary>
    /// 导师审批时间
    /// </summary>
    [Column("supervisor_approval_date")]
    public DateTime? SupervisorApprovalDate { get; set; }

    // ===== 管理员审批 =====
    /// <summary>
    /// 管理员审批状态：同意/拒绝
    /// </summary>
    [Column("admin_approval_status")]
    [MaxLength(20)]
    public string? AdminApprovalStatus { get; set; }

    /// <summary>
    /// 管理员审批备注
    /// </summary>
    [Column("admin_approval_remark")]
    [MaxLength(500)]
    public string? AdminApprovalRemark { get; set; }

    /// <summary>
    /// 管理员审批时间
    /// </summary>
    [Column("admin_approval_date")]
    public DateTime? AdminApprovalDate { get; set; }

    // ===== 归还验收 =====
    /// <summary>
    /// 归还验收状态：完好/损坏/缺件
    /// </summary>
    [Column("return_condition")]
    [MaxLength(20)]
    public string? ReturnCondition { get; set; }

    /// <summary>
    /// 归还备注
    /// </summary>
    [Column("return_remarks")]
    [MaxLength(500)]
    public string? ReturnRemarks { get; set; }

    /// <summary>
    /// 验收人ID
    /// </summary>
    [Column("return_checker_id")]
    public Guid? ReturnCheckerId { get; set; }

    /// <summary>
    /// 验收时间
    /// </summary>
    [Column("return_check_date")]
    public DateTime? ReturnCheckDate { get; set; }

    /// <summary>
    /// 领取人ID（扫码核验时记录）
    /// </summary>
    [Column("recipient_id")]
    public Guid? RecipientId { get; set; }

    /// <summary>
    /// 领取人姓名（冗余字段，扫码时记录）
    /// </summary>
    [Column("recipient_name")]
    [MaxLength(100)]
    public string? RecipientName { get; set; }

    // ===== 续借 =====
    /// <summary>
    /// 是否已续借
    /// </summary>
    [Column("is_renewed")]
    public bool IsRenewed { get; set; } = false;

    /// <summary>
    /// 续借后的计划归还时间
    /// </summary>
    [Column("renewed_return_date")]
    public DateTime? RenewedReturnDate { get; set; }

    /// <summary>
    /// 续借申请状态
    /// </summary>
    [Column("renew_approval_status")]
    [MaxLength(20)]
    public string? RenewApprovalStatus { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>软删除标记</summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    // 导航属性（使用 nullable，避免对 Equipment 表结构有依赖）
    public virtual Equipment? Equipment { get; set; }
    public virtual User Applicant { get; set; } = null!;
    public virtual User? Approver { get; set; }
    public virtual User? ReturnChecker { get; set; }
    public virtual User? Recipient { get; set; }
}

/// <summary>
/// 设备借还状态常量
/// </summary>
public static class BorrowStatus
{
    /// <summary>待老师审批（学生提交后等待导师审批）</summary>
    public const string PendingTeacherApproval = "待老师审批";
    /// <summary>待管理员审批（导师已通过，等待管理员审批）</summary>
    public const string PendingAdminApproval = "待管理员审批";
    /// <summary>待领取（管理员审批通过，等待扫码确认借出）</summary>
    public const string AwaitingPickup = "待领取";
    /// <summary>已借出</summary>
    public const string Borrowed = "已借出";
    /// <summary>待归还（归还扫码后等待管理员验收）</summary>
    public const string AwaitingReturn = "待归还";
    /// <summary>已归还</summary>
    public const string Returned = "已归还";
    /// <summary>已拒绝</summary>
    public const string Rejected = "已拒绝";
    /// <summary>已逾期</summary>
    public const string Overdue = "已逾期";
    /// <summary>续借审批中（导师审批中）</summary>
    public const string RenewPending = "续借审批中";
    /// <summary>管理员审批中（归还时等待管理员审批）</summary>
    public const string ReturnPending = "管理员审批中";
}

/// <summary>
/// 设备分类字典
/// </summary>
[Table("equipment_categories")]
public class EquipmentCategory
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
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("parent_code")]
    [MaxLength(50)]
    public string? ParentCode { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
