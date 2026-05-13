using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

[Table("schedule_statistics")]
public class ScheduleStatistics
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("building_id")]
    public Guid? BuildingId { get; set; }

    [Column("lab_id")]
    public Guid? LabId { get; set; }

    [Column("week_number")]
    public int WeekNumber { get; set; }

    [Column("total_slots")]
    public int TotalSlots { get; set; }

    [Column("used_slots")]
    public int UsedSlots { get; set; }

    [Column("reservation_slots")]
    public int ReservationSlots { get; set; }

    [Column("occupancy_rate")]
    public double OccupancyRate { get; set; }

    [Column("total_student_count")]
    public int TotalStudentCount { get; set; }

    [Column("generated_at")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public virtual Semester? Semester { get; set; }
    public virtual Building? Building { get; set; }
    public virtual Lab? Lab { get; set; }
}
