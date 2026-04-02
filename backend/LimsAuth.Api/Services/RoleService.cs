using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

/// <summary>
/// 角色服务
/// </summary>
public class RoleService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<RoleService> _logger;

    public RoleService(AppDbContext dbContext, ILogger<RoleService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    public async Task<PagedResponse<RoleDto>> GetRolesAsync(RoleListQuery query)
    {
        var queryable = _dbContext.Roles
            .Include(r => r.UserRoles)
            .AsNoTracking()
            .AsQueryable();

        // 筛选
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.ToLower();
            queryable = queryable.Where(r =>
                r.Code.ToLower().Contains(keyword) ||
                r.Name.ToLower().Contains(keyword));
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(r => r.IsActive == query.IsActive);
        }

        // 排序和分页
        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(r => r.IsSystem)
            .ThenBy(r => r.Code)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                Description = r.Description,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UserCount = r.UserRoles.Count
            })
            .ToListAsync();

        return new PagedResponse<RoleDto>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取所有角色(用于下拉选择)
    /// </summary>
    public async Task<List<RoleBriefDto>> GetAllRolesAsync()
    {
        return await _dbContext.Roles
            .Where(r => r.IsActive)
            .AsNoTracking()
            .OrderBy(r => r.Code)
            .Select(r => new RoleBriefDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    public async Task<RoleDetailDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null) return null;

        return new RoleDetailDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Description = role.Description,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UserCount = role.UserRoles.Count,
            Permissions = role.RolePermissions.Select(rp => new PermissionBriefDto
            {
                Id = rp.Permission.Id,
                Code = rp.Permission.Code,
                Name = rp.Permission.Name
            }).ToList()
        };
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request)
    {
        // 检查编码
        if (await _dbContext.Roles.AnyAsync(r => r.Code == request.Code))
        {
            return ApiResponse<RoleDto>.Error(400, "角色编码已存在");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            IsSystem = false,
            IsActive = true
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("创建角色成功: {Code}", role.Code);

        return ApiResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            Description = role.Description,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UserCount = 0
        }, "角色创建成功");
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _dbContext.Roles.FindAsync(id);
        if (role == null)
        {
            return ApiResponse<RoleDto>.Error(404, "角色不存在");
        }

        // 系统内置角色只能修改描述和状态
        if (role.IsSystem)
        {
            if (request.Description != null)
                role.Description = request.Description;
            if (request.IsActive.HasValue)
                role.IsActive = request.IsActive.Value;
        }
        else
        {
            if (request.Name != null)
                role.Name = request.Name;
            if (request.Description != null)
                role.Description = request.Description;
            if (request.IsActive.HasValue)
                role.IsActive = request.IsActive.Value;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新角色成功: {Code}", role.Code);

        var result = await GetRoleByIdAsync(id);
        return ApiResponse<RoleDto>.Success(result!, "角色更新成功");
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteRoleAsync(Guid id)
    {
        var role = await _dbContext.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
        {
            return ApiResponse<bool>.Error(404, "角色不存在");
        }

        // 不能删除系统内置角色
        if (role.IsSystem)
        {
            return ApiResponse<bool>.Error(403, "不能删除系统内置角色");
        }

        // 检查是否有用户使用该角色
        if (role.UserRoles.Any())
        {
            return ApiResponse<bool>.Error(400, "该角色下存在用户，不能删除");
        }

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("删除角色成功: {Code}", role.Code);
        return ApiResponse<bool>.Success(true, "角色删除成功");
    }

    /// <summary>
    /// 更新角色权限
    /// </summary>
    public async Task<ApiResponse<bool>> UpdateRolePermissionsAsync(Guid id, UpdateRolePermissionsRequest request)
    {
        var role = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
        {
            return ApiResponse<bool>.Error(404, "角色不存在");
        }

        // 验证所有权限
        if (request.PermissionIds.Any())
        {
            var validPermissionCount = await _dbContext.Permissions.CountAsync(p => request.PermissionIds.Contains(p.Id));
            if (validPermissionCount != request.PermissionIds.Count)
            {
                return ApiResponse<bool>.Error(400, "包含无效的权限");
            }
        }

        // 清除现有权限
        _dbContext.RolePermissions.RemoveRange(role.RolePermissions);

        // 添加新权限
        foreach (var permissionId in request.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = id,
                PermissionId = permissionId
            });
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新角色权限成功: {Code}, 权限数={Count}", role.Code, request.PermissionIds.Count);
        return ApiResponse<bool>.Success(true, "权限分配成功");
    }

    /// <summary>
    /// 获取角色下的用户
    /// </summary>
    public async Task<List<UserListItemDto>> GetRoleUsersAsync(Guid roleId)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Include(ur => ur.User)
            .ThenInclude(u => u!.Department)
            .Include(ur => ur.User)
            .ThenInclude(u => u!.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .Select(ur => new UserListItemDto
            {
                Id = ur.User.Id,
                Username = ur.User.Username,
                Email = ur.User.Email,
                Phone = ur.User.Phone,
                FullName = ur.User.FullName,
                IsActive = ur.User.IsActive,
                CreatedAt = ur.User.CreatedAt,
                LastLoginAt = ur.User.LastLoginAt,
                Roles = ur.User.UserRoles.Select(uur => new RoleBriefDto
                {
                    Id = uur.Role.Id,
                    Code = uur.Role.Code,
                    Name = uur.Role.Name
                }).ToList(),
                Department = ur.User.Department == null ? null : new DepartmentBriefDto
                {
                    Id = ur.User.Department.Id,
                    Code = ur.User.Department.Code,
                    Name = ur.User.Department.Name
                }
            })
            .ToListAsync();
    }
}
