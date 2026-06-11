namespace LimsAuth.Api.Models;

// ============================================================
// 通用查询参数基类
// ============================================================

public class PagedQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
}

public class UserQueryRequest : PagedQueryRequest
{
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}

public class RoleQueryRequest : PagedQueryRequest
{
    public bool? IsActive { get; set; }
}
