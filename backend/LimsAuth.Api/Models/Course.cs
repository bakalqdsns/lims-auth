using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 课程实体
/// </summary>
[Table("courses")]
public class Course
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
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 课程英文名称
    /// </summary>
    [Column("english_name")]
    [MaxLength(200)]
    public string? EnglishName { get; set; }

    /// <summary>
    /// 修读性质(必修/选修/限选)
    /// </summary>
    [Required]
    [Column("course_type")]
    [MaxLength(20)]
    public string CourseType { get; set; } = "必修";

    /// <summary>
    /// 学分
    /// </summary>
    [Column("credits")]
    public decimal Credits { get; set; } = 0;

    /// <summary>
    /// 总学时
    /// </summary>
    [Column("total_hours")]
    public int TotalHours { get; set; } = 0;

    /// <summary>
    /// 讲授学时
    /// </summary>
    [Column("theory_hours")]
    public int TheoryHours { get; set; } = 0;

    /// <summary>
    /// 实践实训学时
    /// </summary>
    [Column("practice_hours")]
    public int PracticeHours { get; set; } = 0;

    /// <summary>
    /// 实验学时
    /// </summary>
    [Column("experiment_hours")]
    public int ExperimentHours { get; set; } = 0;

    /// <summary>
    /// 网络教学学时
    /// </summary>
    [Column("online_hours")]
    public int OnlineHours { get; set; } = 0;

    /// <summary>
    /// 设课学期(1=上学期,2=下学期,3=全学年)
    /// </summary>
    [Column("semester_type")]
    public int SemesterType { get; set; } = 1;

    /// <summary>
    /// 开课部门ID
    /// </summary>
    [Column("department_id")]
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 课程负责人ID
    /// </summary>
    [Column("manager_id")]
    public Guid? ManagerId { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Department? Department { get; set; }
    public virtual User? Manager { get; set; }
    public virtual ICollection<TeachingTask> TeachingTasks { get; set; } = new List<TeachingTask>();
}
