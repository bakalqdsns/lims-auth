using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IRoleService
{
    Task<ApiResponse<PagedResponse<RoleDto>>> GetListAsync(RoleQueryRequest query);
    Task<ApiResponse<RoleDetailDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleRequest request);
    Task<ApiResponse<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request);
}

public class RoleService : IRoleService
{
    private readonly AppDbContext _db;

    public RoleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<PagedResponse<RoleDto>>> GetListAsync(RoleQueryRequest query)
    {
        var q = _db.SysRoles.Include(r => r.UserRoles).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(r => r.Name.Contains(query.Keyword) || r.Code.Contains(query.Keyword));
        if (query.IsActive.HasValue)
            q = q.Where(r => r.Status == (query.IsActive.Value ? 1 : 0));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                Description = r.Description,
                IsSystem = r.IsSystem,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UserCount = r.UserRoles.Count
            })
            .ToListAsync();

        return ApiResponse<PagedResponse<RoleDto>>.Success(new PagedResponse<RoleDto>
        {
            Items = items, Total = total, Page = query.Page, PageSize = query.PageSize
        });
    }

    public async Task<ApiResponse<RoleDetailDto>> GetByIdAsync(Guid id)
    {
        var r = await _db.SysRoles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (r == null) return ApiResponse<RoleDetailDto>.Error(404, "角色不存在");

        return ApiResponse<RoleDetailDto>.Success(new RoleDetailDto
        {
            Id = r.Id,
            Code = r.Code,
            Name = r.Name,
            Description = r.Description,
            IsSystem = r.IsSystem,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            UserCount = r.UserRoles.Count,
            Permissions = r.RolePermissions.Select(rp => new PermissionBriefDto
            {
                Id = rp.Permission.Id,
                Code = rp.Permission.Code,
                Name = rp.Permission.Name
            }).ToList()
        });
    }

    public async Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleRequest request)
    {
        if (await _db.SysRoles.AnyAsync(r => r.Code == request.Code))
            return ApiResponse<RoleDto>.Error(400, "角色代码已存在");

        var role = new SysRole
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Status = 1,
            CreatedAt = DateTime.UtcNow
        };

        _db.SysRoles.Add(role);
        await _db.SaveChangesAsync();

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Status = role.Status,
            CreatedAt = role.CreatedAt
        }, "角色创建成功");
    }

    public async Task<ApiResponse<RoleDto>> UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _db.SysRoles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return ApiResponse<RoleDto>.Error(404, "角色不存在");

        if (!string.IsNullOrEmpty(request.Name)) role.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description)) role.Description = request.Description;
        if (request.Status.HasValue) role.Status = request.Status.Value;

        await _db.SaveChangesAsync();

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Status = role.Status
        }, "角色更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var role = await _db.SysRoles.Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return ApiResponse.Error(404, "角色不存在");
        if (role.IsSystem == 1) return ApiResponse.Error(400, "系统内置角色无法删除");
        if (role.UserRoles.Any()) return ApiResponse.Error(400, "该角色已分配给用户，无法删除");

        _db.SysRoles.Remove(role);
        await _db.SaveChangesAsync();
        return ApiResponse.Success("角色删除成功");
    }

    public async Task<ApiResponse> UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request)
    {
        var role = await _db.SysRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return ApiResponse.Error(404, "角色不存在");

        _db.SysRolePermissions.RemoveRange(role.RolePermissions);
        foreach (var permId in request.PermissionIds)
        {
            _db.SysRolePermissions.Add(new SysRolePermission { RoleId = id, PermissionId = permId });
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success("权限分配成功");
    }
}
