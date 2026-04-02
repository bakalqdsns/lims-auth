using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 学期实体
/// </summary>
[Table("semesters")]
public class Semester
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 是否当前学期
    /// </summary>
    [Column("is_current")]
    public bool IsCurrent { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual ICollection<AcademicCalendar> CalendarDays { get; set; } = new List<AcademicCalendar>();
    public virtual ICollection<TeachingTask> TeachingTasks { get; set; } = new List<TeachingTask>();
}

/// <summary>
/// 校历/日历实体
/// </summary>
[Table("academic_calendars")]
public class AcademicCalendar
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// 第几周
    /// </summary>
    [Column("week_number")]
    public int WeekNumber { get; set; }

    /// <summary>
    /// 星期几(1=周一,7=周日)
    /// </summary>
    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// 是否节假日
    /// </summary>
    [Column("is_holiday")]
    public bool IsHoliday { get; set; } = false;

    /// <summary>
    /// 节假日名称
    /// </summary>
    [Column("holiday_name")]
    [MaxLength(100)]
    public string? HolidayName { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    // 导航属性
    public virtual Semester Semester { get; set; } = null!;
}
