using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

[Table("usage_registrations")]
public class UsageRegistration
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("lab_id")]
    public Guid? LabId { get; set; }

    [Required]
    [Column("lab_name")]
    [MaxLength(100)]
    public string LabName { get; set; } = string.Empty;

    [Column("building_name")]
    [MaxLength(100)]
    public string? BuildingName { get; set; }

    [Column("room_number")]
    [MaxLength(50)]
    public string? RoomNumber { get; set; }

    [Column("use_date")]
    public DateTime UseDate { get; set; }

    [Column("week_number")]
    public int WeekNumber { get; set; }

    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    [Column("period_number")]
    public int PeriodNumber { get; set; }

    [Column("source")]
    [MaxLength(50)]
    public ScheduleSource Source { get; set; }

    [Column("schedule_entry_id")]
    public Guid? ScheduleEntryId { get; set; }

    [Column("reservation_id")]
    public Guid? ReservationId { get; set; }

    [Column("teaching_application_id")]
    public Guid? TeachingApplicationId { get; set; }

    [Column("experiment_task_id")]
    public Guid? ExperimentTaskId { get; set; }

    [Column("teaching_task_id")]
    public Guid? TeachingTaskId { get; set; }

    [Column("course_name")]
    [MaxLength(200)]
    public string? CourseName { get; set; }

    [Column("project_name")]
    [MaxLength(200)]
    public string? ProjectName { get; set; }

    [Column("experiment_item_name")]
    [MaxLength(200)]
    public string? ExperimentItemName { get; set; }

    [Column("experiment_item_type")]
    [MaxLength(50)]
    public string? ExperimentItemType { get; set; }

    [Column("planned_hours")]
    public int PlannedHours { get; set; }

    [Column("actual_hours")]
    public double ActualHours { get; set; }

    [Column("class_name")]
    [MaxLength(100)]
    public string? ClassName { get; set; }

    [Column("expected_student_count")]
    public int? ExpectedStudentCount { get; set; }

    [Column("actual_student_count")]
    public int? ActualStudentCount { get; set; }

    [Column("attendance_record")]
    [MaxLength(500)]
    public string? AttendanceRecord { get; set; }

    [Column("teaching_condition")]
    [MaxLength(100)]
    public string? TeachingCondition { get; set; }

    [Column("equipment_condition")]
    [MaxLength(100)]
    public string? EquipmentCondition { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

    [Column("reminded_at")]
    public DateTime? RemindedAt { get; set; }

    [Required]
    [Column("filled_by_id")]
    public Guid FilledById { get; set; }

    [Required]
    [Column("filled_by_name")]
    [MaxLength(100)]
    public string FilledByName { get; set; } = string.Empty;

    [Column("filled_at")]
    public DateTime FilledAt { get; set; } = DateTime.UtcNow;

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
    public virtual User? FilledBy { get; set; }
}
