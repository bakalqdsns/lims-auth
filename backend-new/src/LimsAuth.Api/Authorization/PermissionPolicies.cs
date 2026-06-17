using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using LimsAuth.Api.Data;

namespace LimsAuth.Api.Authorization;

public static class PermissionPolicies
{
    public static readonly Dictionary<string, string> PermissionModules = new()
    {
        ["user"] = "用户管理",
        ["role"] = "角色管理",
        ["permission"] = "权限管理",
        ["institution"] = "机构管理",
        ["department"] = "部门管理",
        ["semester"] = "学期管理",
        ["course"] = "课程管理",
        ["major"] = "专业管理",
        ["class"] = "班级管理",
        ["teachingtask"] = "教学任务",
        ["building"] = "楼宇管理",
        ["room"] = "实验室管理",
        ["schedule"] = "排课管理",
        ["booking"] = "预约管理",
        ["usage"] = "使用记录",
        ["experimentitem"] = "实验项目",
        ["asset"] = "设备资产",
        ["loan"] = "借还管理",
        ["consumable"] = "耗材管理"
    };

    private static readonly string[] AllPermissionCodes = SeedData.AllPermissionCodes;

    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var code in AllPermissionCodes)
            {
                var policyName = $"Permission:{code}";
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new PermissionRequirement(code));
                });
            }
        });

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permClaims = context.User.Claims
            .Where(c => c.Type == "permission" && c.Value == requirement.Permission)
            .ToList();

        if (permClaims.Any())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
