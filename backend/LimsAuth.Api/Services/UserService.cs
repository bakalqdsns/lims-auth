using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

/// <summary>
/// 用户服务
/// </summary>
public class UserService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext dbContext, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    public async Task<PagedResponse<UserListItemDto>> GetUsersAsync(UserListQuery query)
    {
        var queryable = _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .AsNoTracking()
            .AsQueryable();

        // 筛选
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.ToLower();
            queryable = queryable.Where(u =>
                u.Username.ToLower().Contains(keyword) ||
                (u.FullName != null && u.FullName.ToLower().Contains(keyword)) ||
                (u.Email != null && u.Email.ToLower().Contains(keyword)));
        }

        if (query.DepartmentId.HasValue)
        {
            queryable = queryable.Where(u => u.DepartmentId == query.DepartmentId);
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(u => u.IsActive == query.IsActive);
        }

        // 排序和分页
        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Roles = u.UserRoles.Select(ur => new RoleBriefDto
                {
                    Id = ur.Role.Id,
                    Code = ur.Role.Code,
                    Name = ur.Role.Name
                }).ToList(),
                Department = u.Department == null ? null : new DepartmentBriefDto
                {
                    Id = u.Department.Id,
                    Code = u.Department.Code,
                    Name = u.Department.Name
                }
            })
            .ToListAsync();

        return new PagedResponse<UserListItemDto>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    public async Task<UserDetailDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(u => u.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        // 获取所有权限(去重)
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .Select(p => new PermissionBriefDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name
            })
            .ToList();

        return new UserDetailDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.UserRoles.Select(ur => new RoleBriefDto
            {
                Id = ur.Role.Id,
                Code = ur.Role.Code,
                Name = ur.Role.Name
            }).ToList(),
            Department = user.Department == null ? null : new DepartmentBriefDto
            {
                Id = user.Department.Id,
                Code = user.Department.Code,
                Name = user.Department.Name
            },
            Permissions = permissions
        };
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<ApiResponse<UserListItemDto>> CreateUserAsync(CreateUserRequest request)
    {
        // 检查用户名
        if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
        {
            return ApiResponse<UserListItemDto>.Error(400, "用户名已存在");
        }

        // 检查邮箱
        if (!string.IsNullOrEmpty(request.Email) && await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
        {
            return ApiResponse<UserListItemDto>.Error(400, "邮箱已被使用");
        }

        // 验证角色
        if (request.RoleIds.Any())
        {
            var validRoleCount = await _dbContext.Roles.CountAsync(r => request.RoleIds.Contains(r.Id) && r.IsActive);
            if (validRoleCount != request.RoleIds.Count)
            {
                return ApiResponse<UserListItemDto>.Error(400, "包含无效的角色");
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Phone = request.Phone,
            FullName = request.FullName,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive
        };

        // 添加角色关联
        foreach (var roleId in request.RoleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("创建用户成功: {Username}", user.Username);

        var result = await GetUserByIdAsync(user.Id);
        return ApiResponse<UserListItemDto>.Success(result!, "用户创建成功");
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<ApiResponse<UserListItemDto>> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return ApiResponse<UserListItemDto>.Error(404, "用户不存在");
        }

        // 检查邮箱
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
            {
                return ApiResponse<UserListItemDto>.Error(400, "邮箱已被使用");
            }
            user.Email = request.Email;
        }

        if (request.Phone != null) user.Phone = request.Phone;
        if (request.FullName != null) user.FullName = request.FullName;
        if (request.DepartmentId.HasValue) user.DepartmentId = request.DepartmentId;
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新用户成功: {Username}", user.Username);

        var result = await GetUserByIdAsync(id);
        return ApiResponse<UserListItemDto>.Success(result!, "用户更新成功");
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.Error(404, "用户不存在");
        }

        // 不允许删除系统内置的超级管理员
        var isSuperAdmin = await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == id && ur.Role.Code == "super_admin");
        if (isSuperAdmin)
        {
            return ApiResponse<bool>.Error(403, "不能删除超级管理员");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("删除用户成功: {Username}", user.Username);
        return ApiResponse<bool>.Success(true, "用户删除成功");
    }

    /// <summary>
    /// 更新用户状态
    /// </summary>
    public async Task<ApiResponse<bool>> UpdateUserStatusAsync(Guid id, bool isActive)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.Error(404, "用户不存在");
        }

        // 不允许禁用超级管理员
        var isSuperAdmin = await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == id && ur.Role.Code == "super_admin");
        if (isSuperAdmin && !isActive)
        {
            return ApiResponse<bool>.Error(403, "不能禁用超级管理员");
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新用户状态成功: {Username}, IsActive={IsActive}", user.Username, isActive);
        return ApiResponse<bool>.Success(true, isActive ? "用户已启用" : "用户已禁用");
    }

    /// <summary>
    /// 更新用户角色
    /// </summary>
    public async Task<ApiResponse<bool>> UpdateUserRolesAsync(Guid id, UpdateUserRolesRequest request)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return ApiResponse<bool>.Error(404, "用户不存在");
        }

        // 验证所有角色
        if (request.RoleIds.Any())
        {
            var validRoleCount = await _dbContext.Roles.CountAsync(r => request.RoleIds.Contains(r.Id) && r.IsActive);
            if (validRoleCount != request.RoleIds.Count)
            {
                return ApiResponse<bool>.Error(400, "包含无效的角色");
            }
        }

        // 清除现有角色
        _dbContext.UserRoles.RemoveRange(user.UserRoles);

        // 添加新角色
        foreach (var roleId in request.RoleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = id,
                RoleId = roleId
            });
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新用户角色成功: {Username}, 角色数={Count}", user.Username, request.RoleIds.Count);
        return ApiResponse<bool>.Success(true, "角色分配成功");
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    public async Task<ApiResponse<bool>> ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.Error(404, "用户不存在");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("重置密码成功: {Username}", user.Username);
        return ApiResponse<bool>.Success(true, "密码重置成功");
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid id, string oldPassword, string newPassword)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.Error(404, "用户不存在");
        }

        // 验证旧密码
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return ApiResponse<bool>.Error(400, "原密码错误");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("修改密码成功: {Username}", user.Username);
        return ApiResponse<bool>.Success(true, "密码修改成功");
    }

    /// <summary>
    /// 获取用户的所有权限编码
    /// </summary>
    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        var permissions = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        return permissions;
    }

    /// <summary>
    /// 获取用户的所有角色编码
    /// </summary>
    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        var roles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Code)
            .ToListAsync();

        return roles;
    }
}
