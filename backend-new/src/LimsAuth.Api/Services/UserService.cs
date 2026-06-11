using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResponse<UserListItemDto>>> GetListAsync(UserQueryRequest query);
    Task<ApiResponse<UserDetailDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<UserListItemDto>> CreateAsync(CreateUserRequest request);
    Task<ApiResponse<UserListItemDto>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> UpdateRolesAsync(Guid id, UpdateUserRolesRequest request);
    Task<ApiResponse> ResetPasswordAsync(Guid id, ResetPasswordRequest request);
    Task<ApiResponse<List<RoleBriefDto>>> GetRolesAsync(Guid id);
    Task<ApiResponse<List<PermissionBriefDto>>> GetPermissionsAsync(Guid id);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<PagedResponse<UserListItemDto>>> GetListAsync(UserQueryRequest query)
    {
        var q = _db.SysUsers
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.MainDepartment)
            .Where(u => u.IsDeleted == 0)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(u => u.Username.Contains(query.Keyword) || u.RealName.Contains(query.Keyword));
        if (query.DepartmentId.HasValue)
            q = q.Where(u => u.MainDepartmentId == query.DepartmentId);
        if (query.IsActive.HasValue)
            q = q.Where(u => u.Status == (query.IsActive.Value ? 1 : 0));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                EmployeeNo = u.EmployeeNo,
                Gender = u.Gender,
                Mobile = u.Mobile,
                Email = u.Email,
                MainInstitutionName = u.MainInstitution != null ? u.MainInstitution.Name : null,
                MainDepartmentName = u.MainDepartment != null ? u.MainDepartment.Name : null,
                UserType = u.UserType,
                Status = u.Status,
                Avatar = u.Avatar,
                LastLoginTime = u.LastLoginTime,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => new RoleBriefDto { Id = ur.Role.Id, Code = ur.Role.Code, Name = ur.Role.Name }).ToList()
            })
            .ToListAsync();

        return ApiResponse<PagedResponse<UserListItemDto>>.Success(new PagedResponse<UserListItemDto>
        {
            Items = items, Total = total, Page = query.Page, PageSize = query.PageSize
        });
    }

    public async Task<ApiResponse<UserDetailDto>> GetByIdAsync(Guid id)
    {
        var u = await _db.SysUsers
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.MainDepartment)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);

        if (u == null) return ApiResponse<UserDetailDto>.Error(404, "用户不存在");

        var perms = u.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .Select(p => new PermissionBriefDto { Id = p.Id, Code = p.Code, Name = p.Name })
            .ToList();

        return ApiResponse<UserDetailDto>.Success(new UserDetailDto
        {
            Id = u.Id,
            Username = u.Username,
            RealName = u.RealName,
            EmployeeNo = u.EmployeeNo,
            Gender = u.Gender,
            Mobile = u.Mobile,
            Email = u.Email,
            MainDepartmentName = u.MainDepartment?.Name,
            UserType = u.UserType,
            Status = u.Status,
            Avatar = u.Avatar,
            LastLoginTime = u.LastLoginTime,
            CreatedAt = u.CreatedAt,
            Roles = u.UserRoles.Select(ur => new RoleBriefDto { Id = ur.Role.Id, Code = ur.Role.Code, Name = ur.Role.Name }).ToList(),
            Permissions = perms
        });
    }

    public async Task<ApiResponse<UserListItemDto>> CreateAsync(CreateUserRequest request)
    {
        if (await _db.SysUsers.AnyAsync(u => u.Username == request.Username && u.IsDeleted == 0))
            return ApiResponse<UserListItemDto>.Error(400, "用户名已存在");

        var user = new SysUser
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RealName = request.RealName,
            EmployeeNo = request.EmployeeNo,
            Gender = request.Gender,
            Mobile = request.Mobile,
            Email = request.Email,
            MainInstitutionId = request.MainInstitutionId,
            MainDepartmentId = request.MainDepartmentId,
            UserType = request.UserType,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _db.SysUsers.Add(user);

        if (request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                _db.SysUserRoles.Add(new SysUserRole { UserId = user.Id, RoleId = roleId });
            }
        }

        await _db.SaveChangesAsync();

        return ApiResponse<UserListItemDto>.Success(new UserListItemDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        }, "用户创建成功");
    }

    public async Task<ApiResponse<UserListItemDto>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);
        if (user == null) return ApiResponse<UserListItemDto>.Error(404, "用户不存在");

        if (!string.IsNullOrEmpty(request.RealName)) user.RealName = request.RealName;
        if (request.Gender.HasValue) user.Gender = request.Gender.Value;
        if (!string.IsNullOrEmpty(request.Mobile)) user.Mobile = request.Mobile;
        if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
        if (request.MainInstitutionId.HasValue) user.MainInstitutionId = request.MainInstitutionId;
        if (request.MainDepartmentId.HasValue) user.MainDepartmentId = request.MainDepartmentId;
        if (!string.IsNullOrEmpty(request.UserType)) user.UserType = request.UserType;
        if (request.Status.HasValue) user.Status = request.Status.Value;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponse<UserListItemDto>.Success(new UserListItemDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Status = user.Status
        }, "用户更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);
        if (user == null) return ApiResponse.Error(404, "用户不存在");
        user.IsDeleted = 1;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("用户删除成功");
    }

    public async Task<ApiResponse> UpdateRolesAsync(Guid id, UpdateUserRolesRequest request)
    {
        var user = await _db.SysUsers.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);
        if (user == null) return ApiResponse.Error(404, "用户不存在");

        _db.SysUserRoles.RemoveRange(user.UserRoles);
        foreach (var roleId in request.RoleIds)
        {
            _db.SysUserRoles.Add(new SysUserRole { UserId = user.Id, RoleId = roleId });
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success("角色分配成功");
    }

    public async Task<ApiResponse> ResetPasswordAsync(Guid id, ResetPasswordRequest request)
    {
        var user = await _db.SysUsers.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);
        if (user == null) return ApiResponse.Error(404, "用户不存在");
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordUpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("密码重置成功");
    }

    public async Task<ApiResponse<List<RoleBriefDto>>> GetRolesAsync(Guid id)
    {
        var roles = await _db.SysUserRoles
            .Where(ur => ur.UserId == id)
            .Include(ur => ur.Role)
            .Select(ur => new RoleBriefDto { Id = ur.Role.Id, Code = ur.Role.Code, Name = ur.Role.Name })
            .ToListAsync();
        return ApiResponse<List<RoleBriefDto>>.Success(roles);
    }

    public async Task<ApiResponse<List<PermissionBriefDto>>> GetPermissionsAsync(Guid id)
    {
        var perms = await _db.SysUserRoles
            .Where(ur => ur.UserId == id)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .Select(p => new PermissionBriefDto { Id = p.Id, Code = p.Code, Name = p.Name })
            .ToListAsync();
        return ApiResponse<List<PermissionBriefDto>>.Success(perms);
    }
}
