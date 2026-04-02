using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 部门/实验室实体
/// </summary>
[Table("departments")]
public class Department
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
    /// 父部门ID(支持层级结构)
    /// </summary>
    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 负责人ID
    /// </summary>
    [Column("manager_id")]
    public Guid? ManagerId { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Department? Parent { get; set; }
    public virtual ICollection<Department> Children { get; set; } = new List<Department>();
    public virtual User? Manager { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
