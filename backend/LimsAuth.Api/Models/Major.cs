using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 专业实体
/// </summary>
[Table("majors")]
public class Major
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

    [Column("english_name")]
    [MaxLength(100)]
    public string? EnglishName { get; set; }

    /// <summary>
    /// 所属部门ID
    /// </summary>
    [Column("department_id")]
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// 学制(年)
    /// </summary>
    [Column("duration")]
    public int Duration { get; set; } = 4;

    /// <summary>
    /// 学位类型(Bachelor/Master/Doctor/Associate)
    /// </summary>
    [Column("degree_type")]
    [MaxLength(20)]
    public string DegreeType { get; set; } = "Bachelor";

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Department Department { get; set; } = null!;
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}

/// <summary>
/// 班级实体
/// </summary>
[Table("classes")]
public class Class
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

    /// <summary>
    /// 年级(如: 2024)
    /// </summary>
    [Required]
    [Column("grade")]
    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// 专业ID
    /// </summary>
    [Column("major_id")]
    public Guid MajorId { get; set; }

    /// <summary>
    /// 所属部门ID
    /// </summary>
    [Column("department_id")]
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// 班主任ID(用户ID)
    /// </summary>
    [Column("head_teacher_id")]
    public Guid? HeadTeacherId { get; set; }

    /// <summary>
    /// 班级管理员学生ID
    /// </summary>
    [Column("admin_student_id")]
    public Guid? AdminStudentId { get; set; }

    /// <summary>
    /// 学生人数
    /// </summary>
    [Column("student_count")]
    public int StudentCount { get; set; } = 0;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Major Major { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
    public virtual User? HeadTeacher { get; set; }
    public virtual User? AdminStudent { get; set; }
    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
    public virtual ICollection<TeachingTask> TeachingTasks { get; set; } = new List<TeachingTask>();
}

/// <summary>
/// 班级学生关联
/// </summary>
[Table("class_students")]
public class ClassStudent
{
    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Class Class { get; set; } = null!;
    public virtual User Student { get; set; } = null!;
}
