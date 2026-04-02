using Microsoft.AspNetCore.Authorization;

namespace LimsAuth.Api.Authorization;

/// <summary>
/// 权限要求
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }
    public PermissionPolicy Policy { get; }

    public PermissionRequirement(string[] permissions, PermissionPolicy policy = PermissionPolicy.All)
    {
        Permissions = permissions;
        Policy = policy;
    }
}
