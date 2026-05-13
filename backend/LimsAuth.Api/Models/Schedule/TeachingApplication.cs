using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

[Table("teaching_applications")]
public class TeachingApplication
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("teaching_task_id")]
    public Guid TeachingTaskId { get; set; }

    [Required]
    [Column("course_name")]
    [MaxLength(200)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [Column("major_id")]
    public Guid MajorId { get; set; }

    [Required]
    [Column("major_name")]
    [MaxLength(100)]
    public string MajorName { get; set; } = string.Empty;

    [Required]
    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Required]
    [Column("class_name")]
    [MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;

    [Column("week_numbers")]
    [MaxLength(200)]
    public string? WeekNumbersJson { get; set; }

    [NotMapped]
    public List<int> WeekNumbers
    {
        get => string.IsNullOrEmpty(WeekNumbersJson)
            ? new List<int>()
            : System.Text.Json.JsonSerializer.Deserialize<List<int>>(WeekNumbersJson) ?? new List<int>();
        set => WeekNumbersJson = System.Text.Json.JsonSerializer.Serialize(value);
    }

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

    [Column("expected_lab_id")]
    public Guid? ExpectedLabId { get; set; }

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Required]
    [Column("applicant_id")]
    public Guid ApplicantId { get; set; }

    [Required]
    [Column("applicant_name")]
    [MaxLength(100)]
    public string ApplicantName { get; set; } = string.Empty;

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

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    public virtual Semester? Semester { get; set; }
    public virtual TeachingTask? TeachingTask { get; set; }
    public virtual Major? Major { get; set; }
    public virtual Class? Class { get; set; }
    public virtual Lab? ExpectedLab { get; set; }
    public virtual User? Applicant { get; set; }
}
