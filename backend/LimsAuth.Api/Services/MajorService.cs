using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IMajorService
{
    Task<List<MajorDto>> GetListAsync(string? keyword = null, string? departmentId = null);
    Task<List<MajorDto>> GetAllAsync();
    Task<MajorDto?> GetByIdAsync(Guid id);
    Task<MajorDto> CreateAsync(CreateMajorRequest request);
    Task<MajorDto?> UpdateAsync(Guid id, UpdateMajorRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
}

public class MajorService : IMajorService
{
    private readonly AppDbContext _dbContext;

    public MajorService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MajorDto>> GetListAsync(string? keyword = null, string? departmentId = null)
    {
        var query = _dbContext.Majors.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(m => m.Name.Contains(keyword) || m.Code.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(departmentId) && Guid.TryParse(departmentId, out var deptId))
        {
            query = query.Where(m => m.DepartmentId == deptId);
        }

        var majors = await query.OrderBy(m => m.Code).ToListAsync();
        
        // 获取部门信息
        var departmentIds = majors.Select(m => m.DepartmentId).Distinct().ToList();
        var departments = await _dbContext.Departments
            .Where(d => departmentIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name);
        
        return majors.Select(m => new MajorDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            EnglishName = m.EnglishName,
            DepartmentId = m.DepartmentId,
            DepartmentName = departments.GetValueOrDefault(m.DepartmentId),
            Duration = m.Duration,
            DegreeType = m.DegreeType,
            Description = m.Description,
            IsActive = m.IsActive,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<List<MajorDto>> GetAllAsync()
    {
        var majors = await _dbContext.Majors.Where(m => m.IsActive).OrderBy(m => m.Name).ToListAsync();
        
        var departmentIds = majors.Select(m => m.DepartmentId).Distinct().ToList();
        var departments = await _dbContext.Departments
            .Where(d => departmentIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name);
        
        return majors.Select(m => new MajorDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            EnglishName = m.EnglishName,
            DepartmentId = m.DepartmentId,
            DepartmentName = departments.GetValueOrDefault(m.DepartmentId),
            Duration = m.Duration,
            DegreeType = m.DegreeType,
            Description = m.Description,
            IsActive = m.IsActive,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<MajorDto?> GetByIdAsync(Guid id)
    {
        var major = await _dbContext.Majors.FindAsync(id);
        if (major == null) return null;
        
        var department = await _dbContext.Departments.FindAsync(major.DepartmentId);
        
        return new MajorDto
        {
            Id = major.Id,
            Code = major.Code,
            Name = major.Name,
            EnglishName = major.EnglishName,
            DepartmentId = major.DepartmentId,
            DepartmentName = department?.Name,
            Duration = major.Duration,
            DegreeType = major.DegreeType,
            Description = major.Description,
            IsActive = major.IsActive,
            CreatedAt = major.CreatedAt
        };
    }

    public async Task<MajorDto> CreateAsync(CreateMajorRequest request)
    {
        var major = new Major
        {
            Code = request.Code,
            Name = request.Name,
            EnglishName = request.EnglishName,
            DepartmentId = request.DepartmentId,
            Duration = request.Duration,
            DegreeType = request.DegreeType,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.Majors.Add(major);
        await _dbContext.SaveChangesAsync();
        
        var department = await _dbContext.Departments.FindAsync(major.DepartmentId);
        
        return new MajorDto
        {
            Id = major.Id,
            Code = major.Code,
            Name = major.Name,
            EnglishName = major.EnglishName,
            DepartmentId = major.DepartmentId,
            DepartmentName = department?.Name,
            Duration = major.Duration,
            DegreeType = major.DegreeType,
            Description = major.Description,
            IsActive = major.IsActive,
            CreatedAt = major.CreatedAt
        };
    }

    public async Task<MajorDto?> UpdateAsync(Guid id, UpdateMajorRequest request)
    {
        var major = await _dbContext.Majors.FindAsync(id);
        if (major == null) return null;

        if (request.Code != null) major.Code = request.Code;
        if (request.Name != null) major.Name = request.Name;
        if (request.EnglishName != null) major.EnglishName = request.EnglishName;
        if (request.DepartmentId.HasValue) major.DepartmentId = request.DepartmentId.Value;
        if (request.Duration.HasValue) major.Duration = request.Duration.Value;
        if (request.DegreeType != null) major.DegreeType = request.DegreeType;
        if (request.Description != null) major.Description = request.Description;
        if (request.IsActive.HasValue) major.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        
        var department = await _dbContext.Departments.FindAsync(major.DepartmentId);
        
        return new MajorDto
        {
            Id = major.Id,
            Code = major.Code,
            Name = major.Name,
            EnglishName = major.EnglishName,
            DepartmentId = major.DepartmentId,
            DepartmentName = department?.Name,
            Duration = major.Duration,
            DegreeType = major.DegreeType,
            Description = major.Description,
            IsActive = major.IsActive,
            CreatedAt = major.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var major = await _dbContext.Majors.FindAsync(id);
        if (major == null) return false;

        _dbContext.Majors.Remove(major);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var major = await _dbContext.Majors.FindAsync(id);
        if (major == null) return false;

        major.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class CreateMajorRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public Guid DepartmentId { get; set; }
    public int Duration { get; set; } = 4;
    public string DegreeType { get; set; } = "Bachelor";
    public string? Description { get; set; }
}

public class UpdateMajorRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? EnglishName { get; set; }
    public Guid? DepartmentId { get; set; }
    public int? Duration { get; set; }
    public string? DegreeType { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class MajorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? EnglishName { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int Duration { get; set; }
    public string DegreeType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
