using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 楼宇管理 DTO
// ============================================================

public class CreateBuildingRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    [MaxLength(500)]
    public string? Address { get; set; }
    public int TotalFloors { get; set; }
    public decimal? Area { get; set; }
    public int? BuildYear { get; set; }
    [MaxLength(50)]
    public string? UseType { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateBuildingRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(200)]
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    [MaxLength(500)]
    public string? Address { get; set; }
    public int? TotalFloors { get; set; }
    public decimal? Area { get; set; }
    public int? BuildYear { get; set; }
    [MaxLength(50)]
    public string? UseType { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class BuildingDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public string? InstitutionName { get; set; }
    public string? Address { get; set; }
    public int TotalFloors { get; set; }
    public decimal? Area { get; set; }
    public int? BuildYear { get; set; }
    public string? UseType { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RoomCount { get; set; }
}

public class BuildingBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

// ============================================================
// 实验室/场地管理 DTO
// ============================================================

public class CreateRoomRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public Guid? BuildingId { get; set; }
    public int FloorNo { get; set; }
    [MaxLength(50)]
    public string? RoomNumber { get; set; }
    public int SeatCount { get; set; }
    public decimal? Area { get; set; }
    [MaxLength(50)]
    public string? RoomType { get; set; }
    [MaxLength(1000)]
    public string? Photo { get; set; }
    public int IsAvailable { get; set; } = 1;
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateRoomRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    public Guid? BuildingId { get; set; }
    public int? FloorNo { get; set; }
    [MaxLength(50)]
    public string? RoomNumber { get; set; }
    public int? SeatCount { get; set; }
    public decimal? Area { get; set; }
    [MaxLength(50)]
    public string? RoomType { get; set; }
    [MaxLength(1000)]
    public string? Photo { get; set; }
    public int? IsAvailable { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class RoomDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public int FloorNo { get; set; }
    public string? RoomNumber { get; set; }
    public int SeatCount { get; set; }
    public decimal? Area { get; set; }
    public string? RoomType { get; set; }
    public string? Photo { get; set; }
    public int IsAvailable { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RoomBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public int SeatCount { get; set; }
}
