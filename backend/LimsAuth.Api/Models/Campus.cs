using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 校区实体
/// </summary>
[Table("campuses")]
public class Campus
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
    /// 校区地址
    /// </summary>
    [Column("address")]
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// 校区面积(平方米)
    /// </summary>
    [Column("area")]
    public decimal? Area { get; set; }

    /// <summary>
    /// 校区类型：主校区、分校区
    /// </summary>
    [Column("campus_type")]
    [MaxLength(50)]
    public string CampusType { get; set; } = "主校区";

    /// <summary>
    /// 联系电话
    /// </summary>
    [Column("contact_phone")]
    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 负责人ID
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

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual User? Manager { get; set; }
    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
}

/// <summary>
/// 楼宇实体
/// </summary>
[Table("buildings")]
public class Building
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
    /// 所属校区ID
    /// </summary>
    [Column("campus_id")]
    public Guid CampusId { get; set; }

    /// <summary>
    /// 楼宇地址/位置
    /// </summary>
    [Column("address")]
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// 楼层数
    /// </summary>
    [Column("floor_count")]
    public int FloorCount { get; set; } = 1;

    /// <summary>
    /// 建筑面积(平方米)
    /// </summary>
    [Column("building_area")]
    public decimal? BuildingArea { get; set; }

    /// <summary>
    /// 楼宇类型：教学楼、实验楼、办公楼、图书馆等
    /// </summary>
    [Column("building_type")]
    [MaxLength(50)]
    public string BuildingType { get; set; } = "实验楼";

    /// <summary>
    /// 建筑年份
    /// </summary>
    [Column("built_year")]
    public int? BuiltYear { get; set; }

    /// <summary>
    /// 负责人ID
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

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Campus Campus { get; set; } = null!;
    public virtual User? Manager { get; set; }
    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();
}
