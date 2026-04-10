using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

public interface ICampusService
{
    Task<List<CampusDto>> GetListAsync(string? keyword);
    Task<CampusDto?> GetByIdAsync(Guid id);
    Task<CampusDto> CreateAsync(CreateCampusRequest request);
    Task<CampusDto?> UpdateAsync(Guid id, UpdateCampusRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
}

public class CampusService : ICampusService
{
    private readonly AppDbContext _context;

    public CampusService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CampusDto>> GetListAsync(string? keyword)
    {
        var query = _context.Campuses
            .Include(c => c.Manager)
            .Include(c => c.Buildings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.Name.Contains(keyword) || c.Code.Contains(keyword));
        }

        var campuses = await query
            .OrderBy(c => c.Code)
            .ToListAsync();

        return campuses.Select(c => new CampusDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Address = c.Address,
            Area = c.Area,
            CampusType = c.CampusType,
            ContactPhone = c.ContactPhone,
            ManagerId = c.ManagerId,
            ManagerName = c.Manager?.FullName,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            BuildingCount = c.Buildings.Count
        }).ToList();
    }

    public async Task<CampusDto?> GetByIdAsync(Guid id)
    {
        var campus = await _context.Campuses
            .Include(c => c.Manager)
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campus == null) return null;

        return new CampusDto
        {
            Id = campus.Id,
            Code = campus.Code,
            Name = campus.Name,
            Address = campus.Address,
            Area = campus.Area,
            CampusType = campus.CampusType,
            ContactPhone = campus.ContactPhone,
            ManagerId = campus.ManagerId,
            ManagerName = campus.Manager?.FullName,
            Description = campus.Description,
            IsActive = campus.IsActive,
            CreatedAt = campus.CreatedAt,
            BuildingCount = campus.Buildings.Count
        };
    }

    public async Task<CampusDto> CreateAsync(CreateCampusRequest request)
    {
        var campus = new Campus
        {
            Code = request.Code,
            Name = request.Name,
            Address = request.Address,
            Area = request.Area,
            CampusType = request.CampusType,
            ContactPhone = request.ContactPhone,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true
        };

        _context.Campuses.Add(campus);
        await _context.SaveChangesAsync();

        return new CampusDto
        {
            Id = campus.Id,
            Code = campus.Code,
            Name = campus.Name,
            Address = campus.Address,
            Area = campus.Area,
            CampusType = campus.CampusType,
            ContactPhone = campus.ContactPhone,
            ManagerId = campus.ManagerId,
            Description = campus.Description,
            IsActive = campus.IsActive,
            CreatedAt = campus.CreatedAt,
            BuildingCount = 0
        };
    }

    public async Task<CampusDto?> UpdateAsync(Guid id, UpdateCampusRequest request)
    {
        var campus = await _context.Campuses.FindAsync(id);
        if (campus == null) return null;

        if (request.Name != null) campus.Name = request.Name;
        if (request.Address != null) campus.Address = request.Address;
        if (request.Area.HasValue) campus.Area = request.Area;
        if (request.CampusType != null) campus.CampusType = request.CampusType;
        if (request.ContactPhone != null) campus.ContactPhone = request.ContactPhone;
        if (request.ManagerId.HasValue) campus.ManagerId = request.ManagerId;
        if (request.Description != null) campus.Description = request.Description;
        if (request.IsActive.HasValue) campus.IsActive = request.IsActive.Value;

        campus.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var campus = await _context.Campuses
            .Include(c => c.Buildings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campus == null) return false;

        if (campus.Buildings.Any())
        {
            throw new InvalidOperationException("该校区下存在楼宇，无法删除");
        }

        _context.Campuses.Remove(campus);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var campus = await _context.Campuses.FindAsync(id);
        if (campus == null) return false;

        campus.IsActive = isActive;
        campus.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
