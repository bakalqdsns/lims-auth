using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

/// <summary>
/// 权限服务
/// </summary>
public class PermissionService
{
    private readonly AppDbContext _dbContext;

    public PermissionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 获取所有权限
    /// </summary>
    public async Task<List<PermissionDto>> GetAllPermissionsAsync()
    {
        return await _dbContext.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Code)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Module = p.Module,
                Description = p.Description
            })
            .ToListAsync();
    }

    /// <summary>
    /// 按模块分组获取权限
    /// </summary>
    public async Task<List<PermissionModuleDto>> GetPermissionsByModuleAsync()
    {
        var permissions = await GetAllPermissionsAsync();

        var moduleNames = new Dictionary<string, string>
        {
            { "user", "用户管理" },
            { "role", "角色管理" },
            { "permission", "权限管理" },
            { "department", "部门管理" },
            { "equipment", "设备管理" },
            { "lab", "实验室管理" },
            { "course", "课程管理" },
            { "report", "报告管理" },
            { "system", "系统管理" }
        };

        return permissions
            .GroupBy(p => p.Module)
            .Select(g => new PermissionModuleDto
            {
                Module = g.Key,
                ModuleName = moduleNames.GetValueOrDefault(g.Key, g.Key),
                Permissions = g.ToList()
            })
            .ToList();
    }

    /// <summary>
    /// 获取权限模块列表
    /// </summary>
    public List<PermissionModuleDto> GetPermissionModules()
    {
        return new List<PermissionModuleDto>
        {
            new() { Module = "user", ModuleName = "用户管理", Permissions = new() },
            new() { Module = "role", ModuleName = "角色管理", Permissions = new() },
            new() { Module = "permission", ModuleName = "权限管理", Permissions = new() },
            new() { Module = "department", ModuleName = "部门管理", Permissions = new() },
            new() { Module = "equipment", ModuleName = "设备管理", Permissions = new() },
            new() { Module = "lab", ModuleName = "实验室管理", Permissions = new() },
            new() { Module = "course", ModuleName = "课程管理", Permissions = new() },
            new() { Module = "report", ModuleName = "报告管理", Permissions = new() },
            new() { Module = "system", ModuleName = "系统管理", Permissions = new() }
        };
    }

    /// <summary>
    /// 获取用户的权限列表
    /// </summary>
    public async Task<List<PermissionDto>> GetUserPermissionsAsync(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .AsNoTracking()
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Module = p.Module,
                Description = p.Description
            })
            .ToListAsync();
    }
}
