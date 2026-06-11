using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 认证相关 DTO
// ============================================================

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public long ExpiresIn { get; set; }
    public UserInfoDto User { get; set; } = new();
}

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? Avatar { get; set; }
    public int Status { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

public class UpdateProfileRequest
{
    public string? RealName { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
}

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

// ============================================================
// 用户管理 DTO
// ============================================================

public class CreateUserRequest
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string RealName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? EmployeeNo { get; set; }
    public int Gender { get; set; } = 2;
    [MaxLength(20)]
    public string? Mobile { get; set; }
    [MaxLength(100)]
    public string? Email { get; set; }
    public Guid? MainInstitutionId { get; set; }
    public Guid? MainDepartmentId { get; set; }
    [MaxLength(20)]
    public string? UserType { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
    public int Status { get; set; } = 1;
}

public class UpdateUserRequest
{
    [MaxLength(100)]
    public string? RealName { get; set; }
    public int? Gender { get; set; }
    [MaxLength(20)]
    public string? Mobile { get; set; }
    [MaxLength(100)]
    public string? Email { get; set; }
    public Guid? MainInstitutionId { get; set; }
    public Guid? MainDepartmentId { get; set; }
    [MaxLength(20)]
    public string? UserType { get; set; }
    public int? Status { get; set; }
}

public class UpdateUserRolesRequest
{
    public List<Guid> RoleIds { get; set; } = new();
}

public class ResetPasswordRequest
{
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? EmployeeNo { get; set; }
    public int Gender { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? MainInstitutionName { get; set; }
    public string? MainDepartmentName { get; set; }
    public string? UserType { get; set; }
    public int Status { get; set; }
    public string? Avatar { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RoleBriefDto> Roles { get; set; } = new();
}

public class UserDetailDto : UserListItemDto
{
    public List<PermissionBriefDto> Permissions { get; set; } = new();
}

// ============================================================
// 角色管理 DTO
// ============================================================

public class CreateRoleRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public int? Status { get; set; }
}

public class UpdateRolePermissionsRequest
{
    public List<Guid> PermissionIds { get; set; } = new();
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int IsSystem { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserCount { get; set; }
}

public class RoleDetailDto : RoleDto
{
    public List<PermissionBriefDto> Permissions { get; set; } = new();
}

public class RoleBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

// ============================================================
// 权限管理 DTO
// ============================================================

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PermissionBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class PermissionModuleDto
{
    public string Module { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}

// ============================================================
// 机构管理 DTO
// ============================================================

public class CreateInstitutionRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    [MaxLength(50)]
    public string? InstitutionType { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateInstitutionRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public Guid? ParentId { get; set; }
    [MaxLength(50)]
    public string? InstitutionType { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class InstitutionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string? InstitutionType { get; set; }
    public int Level { get; set; }
    public string? FullPath { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InstitutionDto> Children { get; set; } = new();
}

public class InstitutionBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

// ============================================================
// 部门管理 DTO
// ============================================================

public class CreateDepartmentRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public Guid? ParentId { get; set; }
    [MaxLength(50)]
    public string? DepartmentType { get; set; }
    public Guid? ManagerId { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateDepartmentRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public Guid? InstitutionId { get; set; }
    public Guid? ParentId { get; set; }
    [MaxLength(50)]
    public string? DepartmentType { get; set; }
    public Guid? ManagerId { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public string? InstitutionName { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string? DepartmentType { get; set; }
    public int Level { get; set; }
    public string? FullPath { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DepartmentDto> Children { get; set; } = new();
}

public class DepartmentBriefDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
