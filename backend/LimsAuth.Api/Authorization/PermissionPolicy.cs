namespace LimsAuth.Api.Authorization;

/// <summary>
/// 权限验证策略
/// </summary>
public enum PermissionPolicy
{
    /// <summary>
    /// 拥有任一权限即可
    /// </summary>
    Any,

    /// <summary>
    /// 必须拥有所有权限
    /// </summary>
    All
}
