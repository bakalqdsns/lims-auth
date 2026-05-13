using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

[Table("reservations")]
public class Reservation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("lab_id")]
    public Guid LabId { get; set; }

    [Column("use_date")]
    public DateTime UseDate { get; set; }

    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    [Column("period_numbers")]
    [MaxLength(200)]
    public string? PeriodNumbersJson { get; set; }

    [NotMapped]
    public List<int> PeriodNumbers
    {
        get => string.IsNullOrEmpty(PeriodNumbersJson)
            ? new List<int>()
            : System.Text.Json.JsonSerializer.Deserialize<List<int>>(PeriodNumbersJson) ?? new List<int>();
        set => PeriodNumbersJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

    [Column("week_number")]
    public int WeekNumber { get; set; }

    [Column("expected_duration_hours")]
    public double? ExpectedDurationHours { get; set; }

    [Required]
    [Column("project_name")]
    [MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    [Column("project_category")]
    [MaxLength(50)]
    public string ProjectCategory { get; set; } = string.Empty;

    [Column("remark")]
    [MaxLength(1000)]
    public string? Remark { get; set; }

    [Required]
    [Column("applicant_id")]
    public Guid ApplicantId { get; set; }

    [Required]
    [Column("applicant_name")]
    [MaxLength(100)]
    public string ApplicantName { get; set; } = string.Empty;

    [Required]
    [Column("applicant_phone")]
    [MaxLength(20)]
    public string ApplicantPhone { get; set; } = string.Empty;

    [Column("project_leader_id")]
    public Guid? ProjectLeaderId { get; set; }

    [Column("project_leader_name")]
    [MaxLength(100)]
    public string? ProjectLeaderName { get; set; }

    [Column("project_leader_phone")]
    [MaxLength(20)]
    public string? ProjectLeaderPhone { get; set; }

    [Column("member_grade")]
    [MaxLength(50)]
    public string? MemberGrade { get; set; }

    [Column("member_class_id")]
    public Guid? MemberClassId { get; set; }

    [Column("member_class_name")]
    [MaxLength(100)]
    public string? MemberClassName { get; set; }

    [Column("member_count")]
    public int? MemberCount { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    [Column("approval_comment")]
    [MaxLength(500)]
    public string? ApprovalComment { get; set; }

    [Column("approved_by")]
    public Guid? ApprovedBy { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("is_cancelled")]
    public bool IsCancelled { get; set; }

    [Column("cancel_reason")]
    [MaxLength(500)]
    public string? CancelReason { get; set; }

    [Column("cancelled_by")]
    public Guid? CancelledBy { get; set; }

    [Column("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    public virtual Semester? Semester { get; set; }
    public virtual Lab? Lab { get; set; }
    public virtual User? Applicant { get; set; }
}
