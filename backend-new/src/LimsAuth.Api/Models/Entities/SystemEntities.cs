namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 系统基础实体
// ============================================================

/// <summary>
/// 用户实体 (Sys_User)
/// </summary>
[EntityTypeConfiguration(typeof(SysUserConfiguration))]
public class SysUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? EmployeeNo { get; set; }
    public string? IdCard { get; set; }
    public int Gender { get; set; } = 2;
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public Guid? MainInstitutionId { get; set; }
    public Guid? MainDepartmentId { get; set; }
    public string? UserType { get; set; }
    public int Status { get; set; } = 1;
    public string? Avatar { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime? PasswordUpdateTime { get; set; }
    public int LoginFailCount { get; set; } = 0;
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? MainInstitution { get; set; }
    public SysDepartment? MainDepartment { get; set; }
    public ICollection<SysUserRole> UserRoles { get; set; } = new List<SysUserRole>();
    public ICollection<EduClassStudent> ClassStudents { get; set; } = new List<EduClassStudent>();
}

/// <summary>
/// 角色实体 (Sys_Role)
/// </summary>
[EntityTypeConfiguration(typeof(SysRoleConfiguration))]
public class SysRole
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int IsSystem { get; set; } = 0;
    public int Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public ICollection<SysUserRole> UserRoles { get; set; } = new List<SysUserRole>();
    public ICollection<SysRolePermission> RolePermissions { get; set; } = new List<SysRolePermission>();
}

/// <summary>
/// 权限实体 (Sys_Permission)
/// </summary>
[EntityTypeConfiguration(typeof(SysPermissionConfiguration))]
public class SysPermission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SysRolePermission> RolePermissions { get; set; } = new List<SysRolePermission>();
}

/// <summary>
/// 用户角色关联 (Sys_UserRole)
/// </summary>
[EntityTypeConfiguration(typeof(SysUserRoleConfiguration))]
public class SysUserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public SysUser User { get; set; } = null!;
    public SysRole Role { get; set; } = null!;
}

/// <summary>
/// 角色权限关联 (Sys_RolePermission)
/// </summary>
[EntityTypeConfiguration(typeof(SysRolePermissionConfiguration))]
public class SysRolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public SysRole Role { get; set; } = null!;
    public SysPermission Permission { get; set; } = null!;
}

/// <summary>
/// 机构实体 (Sys_Institution)
/// </summary>
[EntityTypeConfiguration(typeof(SysInstitutionConfiguration))]
public class SysInstitution
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? InstitutionType { get; set; }
    public int Level { get; set; } = 1;
    public string? FullPath { get; set; }
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? Parent { get; set; }
    public ICollection<SysInstitution> Children { get; set; } = new List<SysInstitution>();
    public ICollection<SysDepartment> Departments { get; set; } = new List<SysDepartment>();
    public ICollection<SysUser> Users { get; set; } = new List<SysUser>();
    public ICollection<VenBuilding> Buildings { get; set; } = new List<VenBuilding>();
}

/// <summary>
/// 部门实体 (Sys_Department)
/// </summary>
[EntityTypeConfiguration(typeof(SysDepartmentConfiguration))]
public class SysDepartment
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public Guid? ParentId { get; set; }
    public string? DepartmentType { get; set; }
    public int Level { get; set; } = 1;
    public string? FullPath { get; set; }
    public Guid? ManagerId { get; set; }
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? Institution { get; set; }
    public SysDepartment? Parent { get; set; }
    public ICollection<SysDepartment> Children { get; set; } = new List<SysDepartment>();
    public ICollection<SysUser> Users { get; set; } = new List<SysUser>();
    public ICollection<EduMajor> Majors { get; set; } = new List<EduMajor>();
    public ICollection<EduClass> Classes { get; set; } = new List<EduClass>();
}
