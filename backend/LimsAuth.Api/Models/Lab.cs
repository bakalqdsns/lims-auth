using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 实验室实体
/// </summary>
[Table("labs")]
public class Lab
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
    /// 所属部门ID
    /// </summary>
    [Column("department_id")]
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 实验室地点
    /// </summary>
    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// 容纳人数
    /// </summary>
    [Column("capacity")]
    public int Capacity { get; set; } = 0;

    /// <summary>
    /// 实验室类型
    /// </summary>
    [Column("lab_type")]
    [MaxLength(50)]
    public string LabType { get; set; } = "普通实验室";

    /// <summary>
    /// 安全等级
    /// </summary>
    [Column("safety_level")]
    [MaxLength(20)]
    public string SafetyLevel { get; set; } = "一般";

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

    // 导航属性
    public virtual Department? Department { get; set; }
    public virtual User? Manager { get; set; }
    public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}

/// <summary>
/// 设备实体
/// </summary>
[Table("equipments")]
public class Equipment
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
    /// 设备型号
    /// </summary>
    [Column("model")]
    [MaxLength(100)]
    public string? Model { get; set; }

    /// <summary>
    /// 制造商
    /// </summary>
    [Column("manufacturer")]
    [MaxLength(200)]
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    [Column("serial_number")]
    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 所属实验室ID
    /// </summary>
    [Column("lab_id")]
    public Guid? LabId { get; set; }

    /// <summary>
    /// 设备分类
    /// </summary>
    [Column("category")]
    [MaxLength(50)]
    public string Category { get; set; } = "通用设备";

    /// <summary>
    /// 设备状态
    /// </summary>
    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "正常"; // 正常, 维修中, 报废, 借用中

    /// <summary>
    /// 购买日期
    /// </summary>
    [Column("purchase_date")]
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// 保修期(月)
    /// </summary>
    [Column("warranty_months")]
    public int? WarrantyMonths { get; set; }

    /// <summary>
    /// 购买价格
    /// </summary>
    [Column("price")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 存放位置
    /// </summary>
    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// 设备图片URL
    /// </summary>
    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 使用说明
    /// </summary>
    [Column("instructions")]
    public string? Instructions { get; set; }

    /// <summary>
    /// 是否需要预约
    /// </summary>
    [Column("requires_booking")]
    public bool RequiresBooking { get; set; } = false;

    /// <summary>
    /// 最大预约时长(小时)
    /// </summary>
    [Column("max_booking_hours")]
    public int? MaxBookingHours { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    public virtual Lab? Lab { get; set; }
}
