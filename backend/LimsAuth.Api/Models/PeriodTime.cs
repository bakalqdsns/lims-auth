using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 节次时间实体
/// </summary>
[Table("period_times")]
public class PeriodTime
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 节次编号(如: 1,2,3...)
    /// </summary>
    [Required]
    [Column("period_number")]
    public int PeriodNumber { get; set; }

    /// <summary>
    /// 节次名称(如: 第1-2节)
    /// </summary>
    [Required]
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    [Required]
    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [Required]
    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("description")]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
