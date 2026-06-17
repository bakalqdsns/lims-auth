using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IPermissionService
{
    Task<ApiResponse<List<PermissionDto>>> GetAllAsync();
    Task<ApiResponse<List<PermissionModuleDto>>> GetByModuleAsync();
    Task RegisterPermissionPoliciesAsync(IServiceProvider services);
}

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<List<PermissionDto>>> GetAllAsync()
    {
        var permissions = await _db.SysPermissions.ToListAsync();
        var dtos = permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Module = p.Module,
            Description = p.Description
        }).ToList();
        return ApiResponse<List<PermissionDto>>.Success(dtos);
    }

    public async Task<ApiResponse<List<PermissionModuleDto>>> GetByModuleAsync()
    {
        var permissions = await _db.SysPermissions.ToListAsync();
        var moduleNames = new Dictionary<string, string>
        {
            ["user"] = "用户管理", ["role"] = "角色管理", ["permission"] = "权限管理",
            ["institution"] = "机构管理", ["department"] = "部门管理",
            ["semester"] = "学期管理", ["course"] = "课程管理", ["major"] = "专业管理",
            ["class"] = "班级管理", ["teachingtask"] = "教学任务",
            ["building"] = "楼宇管理", ["room"] = "实验室管理",
            ["schedule"] = "排课管理", ["booking"] = "预约管理", ["usage"] = "使用记录",
            ["asset"] = "设备管理", ["loan"] = "借还管理",
            ["consumable"] = "耗材管理", ["experimentitem"] = "实验项目管理"
        };

        var grouped = permissions.GroupBy(p => p.Module).Select(g => new PermissionModuleDto
        {
            Module = g.Key,
            ModuleName = moduleNames.GetValueOrDefault(g.Key, g.Key),
            Permissions = g.Select(p => new PermissionDto
            {
                Id = p.Id, Code = p.Code, Name = p.Name, Module = p.Module, Description = p.Description
            }).ToList()
        }).ToList();

        return ApiResponse<List<PermissionModuleDto>>.Success(grouped);
    }

    public Task RegisterPermissionPoliciesAsync(IServiceProvider services)
    {
        // Policies are registered statically via PermissionPolicies.AddPermissionAuthorization()
        // which is called in Program.cs before this method is invoked.
        return Task.CompletedTask;
    }
}
