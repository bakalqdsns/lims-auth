using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

public interface IBuildingService
{
    Task<List<BuildingDto>> GetListAsync(string? keyword, Guid? campusId);
    Task<BuildingDto?> GetByIdAsync(Guid id);
    Task<BuildingDto> CreateAsync(CreateBuildingRequest request);
    Task<BuildingDto?> UpdateAsync(Guid id, UpdateBuildingRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
    Task<List<BuildingBriefDto>> GetByCampusIdAsync(Guid campusId);
}

public class BuildingService : IBuildingService
{
    private readonly AppDbContext _context;

    public BuildingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BuildingDto>> GetListAsync(string? keyword, Guid? campusId)
    {
        var query = _context.Buildings
            .Include(b => b.Campus)
            .Include(b => b.Manager)
            .Include(b => b.Labs)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(b => b.Name.Contains(keyword) || b.Code.Contains(keyword));
        }

        if (campusId.HasValue)
        {
            query = query.Where(b => b.CampusId == campusId.Value);
        }

        var buildings = await query
            .OrderBy(b => b.Campus.Code)
            .ThenBy(b => b.Code)
            .ToListAsync();

        return buildings.Select(b => new BuildingDto
        {
            Id = b.Id,
            Code = b.Code,
            Name = b.Name,
            CampusId = b.CampusId,
            CampusName = b.Campus.Name,
            Address = b.Address,
            FloorCount = b.FloorCount,
            BuildingArea = b.BuildingArea,
            BuildingType = b.BuildingType,
            BuiltYear = b.BuiltYear,
            ManagerId = b.ManagerId,
            ManagerName = b.Manager?.FullName,
            Description = b.Description,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt,
            LabCount = b.Labs.Count
        }).ToList();
    }

    public async Task<BuildingDto?> GetByIdAsync(Guid id)
    {
        var building = await _context.Buildings
            .Include(b => b.Campus)
            .Include(b => b.Manager)
            .Include(b => b.Labs)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (building == null) return null;

        return new BuildingDto
        {
            Id = building.Id,
            Code = building.Code,
            Name = building.Name,
            CampusId = building.CampusId,
            CampusName = building.Campus.Name,
            Address = building.Address,
            FloorCount = building.FloorCount,
            BuildingArea = building.BuildingArea,
            BuildingType = building.BuildingType,
            BuiltYear = building.BuiltYear,
            ManagerId = building.ManagerId,
            ManagerName = building.Manager?.FullName,
            Description = building.Description,
            IsActive = building.IsActive,
            CreatedAt = building.CreatedAt,
            LabCount = building.Labs.Count
        };
    }

    public async Task<BuildingDto> CreateAsync(CreateBuildingRequest request)
    {
        // 验证校区是否存在
        var campus = await _context.Campuses.FindAsync(request.CampusId);
        if (campus == null)
        {
            throw new ArgumentException("指定的校区不存在");
        }

        var building = new Building
        {
            Code = request.Code,
            Name = request.Name,
            CampusId = request.CampusId,
            Address = request.Address,
            FloorCount = request.FloorCount,
            BuildingArea = request.BuildingArea,
            BuildingType = request.BuildingType,
            BuiltYear = request.BuiltYear,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true
        };

        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();

        return new BuildingDto
        {
            Id = building.Id,
            Code = building.Code,
            Name = building.Name,
            CampusId = building.CampusId,
            CampusName = campus.Name,
            Address = building.Address,
            FloorCount = building.FloorCount,
            BuildingArea = building.BuildingArea,
            BuildingType = building.BuildingType,
            BuiltYear = building.BuiltYear,
            ManagerId = building.ManagerId,
            Description = building.Description,
            IsActive = building.IsActive,
            CreatedAt = building.CreatedAt,
            LabCount = 0
        };
    }

    public async Task<BuildingDto?> UpdateAsync(Guid id, UpdateBuildingRequest request)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null) return null;

        if (request.Name != null) building.Name = request.Name;
        if (request.Address != null) building.Address = request.Address;
        if (request.FloorCount.HasValue) building.FloorCount = request.FloorCount.Value;
        if (request.BuildingArea.HasValue) building.BuildingArea = request.BuildingArea;
        if (request.BuildingType != null) building.BuildingType = request.BuildingType;
        if (request.BuiltYear.HasValue) building.BuiltYear = request.BuiltYear;
        if (request.ManagerId.HasValue) building.ManagerId = request.ManagerId;
        if (request.Description != null) building.Description = request.Description;
        if (request.IsActive.HasValue) building.IsActive = request.IsActive.Value;

        building.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var building = await _context.Buildings
            .Include(b => b.Labs)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (building == null) return false;

        if (building.Labs.Any())
        {
            throw new InvalidOperationException("该楼宇下存在实验室，无法删除");
        }

        _context.Buildings.Remove(building);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null) return false;

        building.IsActive = isActive;
        building.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<BuildingBriefDto>> GetByCampusIdAsync(Guid campusId)
    {
        var buildings = await _context.Buildings
            .Include(b => b.Campus)
            .Where(b => b.CampusId == campusId && b.IsActive)
            .OrderBy(b => b.Code)
            .ToListAsync();

        return buildings.Select(b => new BuildingBriefDto
        {
            Id = b.Id,
            Code = b.Code,
            Name = b.Name,
            CampusName = b.Campus.Name
        }).ToList();
    }
}
