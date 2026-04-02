using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LimsAuth.Api.Authorization;

/// <summary>
/// 权限处理器
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 获取用户权限声明
        var userPermissions = context.User.FindAll("permission")
            .Select(c => c.Value)
            .ToList();

        // 超级管理员拥有所有权限
        var isSuperAdmin = context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "super_admin");
        if (isSuperAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 验证权限
        bool hasPermission;
        if (requirement.Policy == PermissionPolicy.All)
        {
            hasPermission = requirement.Permissions.All(p => userPermissions.Contains(p));
        }
        else
        {
            hasPermission = requirement.Permissions.Any(p => userPermissions.Contains(p));
        }

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
