namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 场地管理实体
// ============================================================

/// <summary>
/// 楼宇实体 (Ven_Building)
/// </summary>
[EntityTypeConfiguration(typeof(VenBuildingConfiguration))]
public class VenBuilding
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public string? Address { get; set; }
    public int TotalFloors { get; set; }
    public decimal? Area { get; set; }
    public int? BuildYear { get; set; }
    public string? UseType { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? Institution { get; set; }
    public ICollection<LabRoom> Rooms { get; set; } = new List<LabRoom>();
}

/// <summary>
/// 场地/实验室实体 (Lab_Room)
/// </summary>
[EntityTypeConfiguration(typeof(VenRoomConfiguration))]
public class LabRoom
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? BuildingId { get; set; }
    public int FloorNo { get; set; }
    public string? RoomNumber { get; set; }
    public int SeatCount { get; set; }
    public decimal? Area { get; set; }
    public string? RoomType { get; set; }
    public string? Photo { get; set; }
    public int IsAvailable { get; set; } = 1;
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public VenBuilding? Building { get; set; }
    public ICollection<LabSchedule> Schedules { get; set; } = new List<LabSchedule>();
    public ICollection<LabBookingApply> BookingApplies { get; set; } = new List<LabBookingApply>();
    public ICollection<LabUsageRegister> UsageRegisters { get; set; } = new List<LabUsageRegister>();
}
