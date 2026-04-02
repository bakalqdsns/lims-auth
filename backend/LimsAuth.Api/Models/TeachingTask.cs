using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 教学任务实体
/// </summary>
[Table("teaching_tasks")]
public class TeachingTask
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 学期ID
    /// </summary>
    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    /// <summary>
    /// 课程ID
    /// </summary>
    [Required]
    [Column("course_id")]
    public Guid CourseId { get; set; }

    /// <summary>
    /// 班级ID
    /// </summary>
    [Required]
    [Column("class_id")]
    public Guid ClassId { get; set; }

    /// <summary>
    /// 任务类型(主讲/辅导/实验)
    /// </summary>
    [Column("task_type")]
    [MaxLength(20)]
    public string TaskType { get; set; } = "主讲";

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Semester Semester { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
    public virtual ICollection<TeachingTaskTeacher> Teachers { get; set; } = new List<TeachingTaskTeacher>();
}

/// <summary>
/// 教学任务教师关联
/// </summary>
[Table("teaching_task_teachers")]
public class TeachingTaskTeacher
{
    [Column("teaching_task_id")]
    public Guid TeachingTaskId { get; set; }

    [Column("teacher_id")]
    public Guid TeacherId { get; set; }

    /// <summary>
    /// 是否主讲教师
    /// </summary>
    [Column("is_main_teacher")]
    public bool IsMainTeacher { get; set; } = false;

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual TeachingTask TeachingTask { get; set; } = null!;
    public virtual User Teacher { get; set; } = null!;
}
