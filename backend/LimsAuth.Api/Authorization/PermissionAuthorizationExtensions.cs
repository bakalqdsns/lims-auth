using Microsoft.AspNetCore.Authorization;

namespace LimsAuth.Api.Authorization;

/// <summary>
/// 权限授权扩展
/// </summary>
public static class PermissionAuthorizationExtensions
{
    /// <summary>
    /// 添加权限策略
    /// </summary>
    public static AuthorizationPolicyBuilder RequirePermission(
        this AuthorizationPolicyBuilder builder,
        params string[] permissions)
    {
        return builder.AddRequirements(new PermissionRequirement(permissions, PermissionPolicy.All));
    }

    /// <summary>
    /// 添加权限策略(任一)
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAnyPermission(
        this AuthorizationPolicyBuilder builder,
        params string[] permissions)
    {
        return builder.AddRequirements(new PermissionRequirement(permissions, PermissionPolicy.Any));
    }
}
