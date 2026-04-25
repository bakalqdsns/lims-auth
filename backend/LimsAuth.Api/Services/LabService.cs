using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface ILabService
{
    Task<List<LabDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? labType = null, Guid? buildingId = null);
    Task<LabDto?> GetByIdAsync(Guid id);
    Task<Lab> CreateAsync(CreateLabRequest request);
    Task<Lab?> UpdateAsync(Guid id, UpdateLabRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
}

public class LabService : ILabService
{
    private readonly AppDbContext _dbContext;

    public LabService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LabDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? labType = null, Guid? buildingId = null)
    {
        var query = _dbContext.Labs
            .Include(l => l.Department)
            .Include(l => l.Building)
            .ThenInclude(b => b!.Campus)
            .Include(l => l.Manager)
            .Include(l => l.Equipments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(l => l.Name.Contains(keyword) || l.Code.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(departmentId) && Guid.TryParse(departmentId, out var deptId))
        {
            query = query.Where(l => l.DepartmentId == deptId);
        }

        if (!string.IsNullOrEmpty(labType))
        {
            query = query.Where(l => l.LabType == labType);
        }

        if (buildingId.HasValue)
        {
            query = query.Where(l => l.BuildingId == buildingId.Value);
        }

        var labs = await query.OrderBy(l => l.Code).ToListAsync();

        return labs.Select(l => new LabDto
        {
            Id = l.Id,
            Code = l.Code,
            Name = l.Name,
            DepartmentId = l.DepartmentId,
            DepartmentName = l.Department?.Name,
            BuildingId = l.BuildingId,
            BuildingName = l.Building?.Name,
            CampusName = l.Building?.Campus?.Name ?? null,
            Floor = l.Floor,
            RoomNumber = l.RoomNumber,
            Location = l.Location,
            Capacity = l.Capacity,
            LabType = l.LabType,
            SafetyLevel = l.SafetyLevel,
            ManagerId = l.ManagerId,
            ManagerName = l.Manager?.FullName ?? l.Manager?.Username,
            Description = l.Description,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt,
            EquipmentCount = l.Equipments.Count
        }).ToList();
    }

    public async Task<LabDto?> GetByIdAsync(Guid id)
    {
        var lab = await _dbContext.Labs
            .Include(l => l.Department)
            .Include(l => l.Building)
            .ThenInclude(b => b!.Campus)
            .Include(l => l.Manager)
            .Include(l => l.Equipments)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lab == null) return null;

        return new LabDto
        {
            Id = lab.Id,
            Code = lab.Code,
            Name = lab.Name,
            DepartmentId = lab.DepartmentId,
            DepartmentName = lab.Department?.Name,
            BuildingId = lab.BuildingId,
            BuildingName = lab.Building?.Name,
            CampusName = lab.Building?.Campus?.Name ?? null,
            Floor = lab.Floor,
            RoomNumber = lab.RoomNumber,
            Location = lab.Location,
            Capacity = lab.Capacity,
            LabType = lab.LabType,
            SafetyLevel = lab.SafetyLevel,
            ManagerId = lab.ManagerId,
            ManagerName = lab.Manager?.FullName ?? lab.Manager?.Username,
            Description = lab.Description,
            IsActive = lab.IsActive,
            CreatedAt = lab.CreatedAt,
            EquipmentCount = lab.Equipments.Count
        };
    }

    public async Task<Lab> CreateAsync(CreateLabRequest request)
    {
        var lab = new Lab
        {
            Code = request.Code,
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            BuildingId = request.BuildingId,
            Floor = request.Floor,
            RoomNumber = request.RoomNumber,
            Location = request.Location,
            Capacity = request.Capacity,
            LabType = request.LabType,
            SafetyLevel = request.SafetyLevel,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.Labs.Add(lab);
        await _dbContext.SaveChangesAsync();
        return lab;
    }

    public async Task<Lab?> UpdateAsync(Guid id, UpdateLabRequest request)
    {
        var lab = await _dbContext.Labs.FindAsync(id);
        if (lab == null) return null;

        if (request.Code != null) lab.Code = request.Code;
        if (request.Name != null) lab.Name = request.Name;
        if (request.DepartmentId.HasValue) lab.DepartmentId = request.DepartmentId.Value;
        if (request.BuildingId.HasValue) lab.BuildingId = request.BuildingId.Value;
        if (request.Floor.HasValue) lab.Floor = request.Floor.Value;
        if (request.RoomNumber != null) lab.RoomNumber = request.RoomNumber;
        if (request.Location != null) lab.Location = request.Location;
        if (request.Capacity.HasValue) lab.Capacity = request.Capacity.Value;
        if (request.LabType != null) lab.LabType = request.LabType;
        if (request.SafetyLevel != null) lab.SafetyLevel = request.SafetyLevel;
        if (request.ManagerId.HasValue) lab.ManagerId = request.ManagerId.Value;
        if (request.Description != null) lab.Description = request.Description;
        if (request.IsActive.HasValue) lab.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        return lab;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var lab = await _dbContext.Labs.FindAsync(id);
        if (lab == null) return false;

        _dbContext.Labs.Remove(lab);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var lab = await _dbContext.Labs.FindAsync(id);
        if (lab == null) return false;

        lab.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class LabDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public string? CampusName { get; set; }
    public int? Floor { get; set; }
    public string? RoomNumber { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public string LabType { get; set; } = string.Empty;
    public string SafetyLevel { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EquipmentCount { get; set; }
}

public class CreateLabRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? BuildingId { get; set; }
    public int? Floor { get; set; }
    public string? RoomNumber { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; } = 0;
    public string LabType { get; set; } = "普通实验室";
    public string SafetyLevel { get; set; } = "一般";
    public Guid? ManagerId { get; set; }
    public string? Description { get; set; }
}

public class UpdateLabRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? BuildingId { get; set; }
    public int? Floor { get; set; }
    public string? RoomNumber { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public string? LabType { get; set; }
    public string? SafetyLevel { get; set; }
    public Guid? ManagerId { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
