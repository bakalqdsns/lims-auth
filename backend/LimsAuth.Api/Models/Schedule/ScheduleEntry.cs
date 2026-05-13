using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

[Table("schedule_entries")]
public class ScheduleEntry
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("lab_id")]
    public Guid? LabId { get; set; }

    [Column("week_number")]
    public int WeekNumber { get; set; }

    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    [Column("period_number")]
    public int PeriodNumber { get; set; }

    [Column("source")]
    [MaxLength(50)]
    public ScheduleSource Source { get; set; } = ScheduleSource.CentralScheduling;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

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

    [Column("course_id")]
    public Guid? CourseId { get; set; }

    [Column("teacher_id")]
    public Guid? TeacherId { get; set; }

    [Column("teacher_name")]
    [MaxLength(100)]
    public string? TeacherName { get; set; }

    [Column("class_id")]
    public Guid? ClassId { get; set; }

    [Column("class_name")]
    [MaxLength(100)]
    public string? ClassName { get; set; }

    [Column("major_id")]
    public Guid? MajorId { get; set; }

    [Column("major_name")]
    [MaxLength(100)]
    public string? MajorName { get; set; }

    [Column("student_count")]
    public int? StudentCount { get; set; }

    [Column("building_name")]
    [MaxLength(100)]
    public string? BuildingName { get; set; }

    [Column("room_number")]
    [MaxLength(50)]
    public string? RoomNumber { get; set; }

    [Column("remark")]
    [MaxLength(500)]
    public string? Remark { get; set; }

    [Column("has_conflict")]
    public bool HasConflict { get; set; }

    [Column("conflict_info")]
    [MaxLength(500)]
    public string? ConflictInfo { get; set; }

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
}
