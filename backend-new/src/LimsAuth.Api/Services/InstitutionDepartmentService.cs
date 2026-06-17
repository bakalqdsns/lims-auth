using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IInstitutionService
{
    Task<ApiResponse<List<InstitutionDto>>> GetTreeAsync();
    Task<ApiResponse<InstitutionDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<InstitutionDto>> CreateAsync(CreateInstitutionRequest request);
    Task<ApiResponse<InstitutionDto>> UpdateAsync(Guid id, UpdateInstitutionRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class InstitutionService : IInstitutionService
{
    private readonly AppDbContext _db;
    public InstitutionService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<List<InstitutionDto>>> GetTreeAsync()
    {
        var all = await _db.SysInstitutions.Where(i => i.Status == 1).ToListAsync();
        var roots = all.Where(i => i.ParentId == null).ToList();
        var result = roots.Select(r => MapToDto(r, all)).ToList();
        return ApiResponse<List<InstitutionDto>>.Success(result);
    }

    public async Task<ApiResponse<InstitutionDto>> GetByIdAsync(Guid id)
    {
        var item = await _db.SysInstitutions.FindAsync(id);
        if (item == null) return ApiResponse<InstitutionDto>.Error(404, "机构不存在");
        var all = await _db.SysInstitutions.Where(i => i.Status == 1).ToListAsync();
        return ApiResponse<InstitutionDto>.Success(MapToDto(item, all));
    }

    public async Task<ApiResponse<InstitutionDto>> CreateAsync(CreateInstitutionRequest request)
    {
        var item = new SysInstitution
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            ParentId = request.ParentId,
            InstitutionType = request.InstitutionType,
            Status = request.Status,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        if (request.ParentId.HasValue)
        {
            var parent = await _db.SysInstitutions.FindAsync(request.ParentId);
            item.Level = (parent?.Level ?? 0) + 1;
            item.FullPath = (parent?.FullPath ?? "") + "/" + item.Id;
        }
        else
        {
            item.Level = 1;
            item.FullPath = item.Id.ToString();
        }

        _db.SysInstitutions.Add(item);
        await _db.SaveChangesAsync();
        return ApiResponse<InstitutionDto>.Success(new InstitutionDto { Id = item.Id, Code = item.Code, Name = item.Name }, "机构创建成功");
    }

    public async Task<ApiResponse<InstitutionDto>> UpdateAsync(Guid id, UpdateInstitutionRequest request)
    {
        var item = await _db.SysInstitutions.FindAsync(id);
        if (item == null) return ApiResponse<InstitutionDto>.Error(404, "机构不存在");
        if (!string.IsNullOrEmpty(request.Name)) item.Name = request.Name;
        if (request.ParentId.HasValue) item.ParentId = request.ParentId;
        if (!string.IsNullOrEmpty(request.InstitutionType)) item.InstitutionType = request.InstitutionType;
        if (request.Status.HasValue) item.Status = request.Status.Value;
        if (!string.IsNullOrEmpty(request.Description)) item.Description = request.Description;
        await _db.SaveChangesAsync();
        return ApiResponse<InstitutionDto>.Success(new InstitutionDto { Id = item.Id, Code = item.Code, Name = item.Name }, "机构更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (await _db.SysInstitutions.AnyAsync(i => i.ParentId == id))
            return ApiResponse.Error(400, "该机构下存在子机构，无法删除");
        var item = await _db.SysInstitutions.FindAsync(id);
        if (item != null) _db.SysInstitutions.Remove(item);
        await _db.SaveChangesAsync();
        return ApiResponse.Success("机构删除成功");
    }

    private InstitutionDto MapToDto(SysInstitution item, List<SysInstitution> all)
    {
        var dto = new InstitutionDto
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            ParentId = item.ParentId,
            InstitutionType = item.InstitutionType,
            Level = item.Level,
            FullPath = item.FullPath,
            Status = item.Status,
            Description = item.Description,
            CreatedAt = item.CreatedAt
        };
        var parent = all.FirstOrDefault(i => i.Id == item.ParentId);
        dto.ParentName = parent?.Name;
        dto.Children = all.Where(i => i.ParentId == item.Id).Select(c => MapToDto(c, all)).ToList();
        return dto;
    }
}

public interface IDepartmentService
{
    Task<ApiResponse<List<DepartmentDto>>> GetTreeAsync();
    Task<ApiResponse<DepartmentDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentRequest request);
    Task<ApiResponse<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _db;
    public DepartmentService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<List<DepartmentDto>>> GetTreeAsync()
    {
        try
        {
            var all = await _db.SysDepartments
                .Include(d => d.Institution)
                .Where(d => d.Status == 1)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"[DEPT-TREE] count={all.Count}");

            var roots = all.Where(d => d.ParentId == null).ToList();
            var result = roots.Select(r => MapToDto(r, all)).ToList();
            return ApiResponse<List<DepartmentDto>>.Success(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEPT-TREE-ERROR] {ex.GetType().Name}: {ex.Message}");
            return ApiResponse<List<DepartmentDto>>.Error(500, ex.Message);
        }
    }

    public async Task<ApiResponse<DepartmentDto>> GetByIdAsync(Guid id)
    {
        var item = await _db.SysDepartments.Include(d => d.Institution).FirstOrDefaultAsync(d => d.Id == id);
        if (item == null) return ApiResponse<DepartmentDto>.Error(404, "部门不存在");
        var all = await _db.SysDepartments.Where(d => d.Status == 1).ToListAsync();
        return ApiResponse<DepartmentDto>.Success(MapToDto(item, all));
    }

    public async Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentRequest request)
    {
        var item = new SysDepartment
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            InstitutionId = request.InstitutionId,
            ParentId = request.ParentId,
            DepartmentType = request.DepartmentType,
            ManagerId = request.ManagerId,
            Status = request.Status,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        if (request.ParentId.HasValue)
        {
            var parent = await _db.SysDepartments.FindAsync(request.ParentId);
            item.Level = (parent?.Level ?? 0) + 1;
            item.FullPath = (parent?.FullPath ?? "") + "/" + item.Id;
        }
        else
        {
            item.Level = 1;
            item.FullPath = item.Id.ToString();
        }

        _db.SysDepartments.Add(item);
        await _db.SaveChangesAsync();
        return ApiResponse<DepartmentDto>.Success(new DepartmentDto { Id = item.Id, Code = item.Code, Name = item.Name }, "部门创建成功");
    }

    public async Task<ApiResponse<DepartmentDto>> UpdateAsync(Guid id, UpdateDepartmentRequest request)
    {
        var item = await _db.SysDepartments.FindAsync(id);
        if (item == null) return ApiResponse<DepartmentDto>.Error(404, "部门不存在");
        if (!string.IsNullOrEmpty(request.Name)) item.Name = request.Name;
        if (request.InstitutionId.HasValue) item.InstitutionId = request.InstitutionId;
        if (request.ParentId.HasValue) item.ParentId = request.ParentId;
        if (!string.IsNullOrEmpty(request.DepartmentType)) item.DepartmentType = request.DepartmentType;
        if (request.ManagerId.HasValue) item.ManagerId = request.ManagerId;
        if (request.Status.HasValue) item.Status = request.Status.Value;
        if (!string.IsNullOrEmpty(request.Description)) item.Description = request.Description;
        await _db.SaveChangesAsync();
        return ApiResponse<DepartmentDto>.Success(new DepartmentDto { Id = item.Id, Code = item.Code, Name = item.Name }, "部门更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        if (await _db.SysDepartments.AnyAsync(d => d.ParentId == id))
            return ApiResponse.Error(400, "该部门下存在子部门，无法删除");
        var item = await _db.SysDepartments.FindAsync(id);
        if (item != null) _db.SysDepartments.Remove(item);
        await _db.SaveChangesAsync();
        return ApiResponse.Success("部门删除成功");
    }

    private DepartmentDto MapToDto(SysDepartment item, List<SysDepartment> all)
    {
        var dto = new DepartmentDto
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            InstitutionId = item.InstitutionId,
            InstitutionName = item.Institution?.Name,
            ParentId = item.ParentId,
            DepartmentType = item.DepartmentType,
            Level = item.Level,
            FullPath = item.FullPath,
            ManagerId = item.ManagerId,
            Status = item.Status,
            Description = item.Description,
            CreatedAt = item.CreatedAt
        };
        var parent = all.FirstOrDefault(i => i.Id == item.ParentId);
        dto.ParentName = parent?.Name;
        dto.Children = all.Where(i => i.ParentId == item.Id).Select(c => MapToDto(c, all)).ToList();
        return dto;
    }
}
