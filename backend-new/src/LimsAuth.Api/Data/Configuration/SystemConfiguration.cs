using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

// ============================================================
// 系统实体配置
// ============================================================

public class SysUserConfiguration : IEntityTypeConfiguration<SysUser>
{
    public void Configure(EntityTypeBuilder<SysUser> b)
    {
        b.ToTable("Sys_User");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("UserID");
        b.Property(x => x.Username).HasColumnName("UserName").HasMaxLength(50).IsRequired();
        b.Property(x => x.Password).HasColumnName("Password").HasMaxLength(255).IsRequired();
        b.Property(x => x.RealName).HasColumnName("RealName").HasMaxLength(100).IsRequired();
        b.Property(x => x.EmployeeNo).HasColumnName("EmployeeNo").HasMaxLength(50);
        b.Property(x => x.IdCard).HasColumnName("IDCard").HasMaxLength(18);
        b.Property(x => x.Gender).HasColumnName("Gender").HasDefaultValue(2);
        b.Property(x => x.Mobile).HasColumnName("Mobile").HasMaxLength(20);
        b.Property(x => x.Email).HasColumnName("Email").HasMaxLength(100);
        b.Property(x => x.MainInstitutionId).HasColumnName("MainInstitutionID");
        b.Property(x => x.MainDepartmentId).HasColumnName("MainDepartmentID");
        b.Property(x => x.UserType).HasColumnName("UserType").HasMaxLength(20);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Avatar).HasColumnName("Avatar").HasMaxLength(500);
        b.Property(x => x.LastLoginTime).HasColumnName("LastLoginTime");
        b.Property(x => x.LastLoginIp).HasColumnName("LastLoginIP").HasMaxLength(50);
        b.Property(x => x.PasswordUpdateTime).HasColumnName("PasswordUpdateTime");
        b.Property(x => x.LoginFailCount).HasColumnName("LoginFailCount").HasDefaultValue(0);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasIndex(x => x.Username).IsUnique();
        b.HasIndex(x => x.EmployeeNo);
        b.HasOne(x => x.MainInstitution).WithMany(x => x.Users).HasForeignKey(x => x.MainInstitutionId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.MainDepartment).WithMany(x => x.Users).HasForeignKey(x => x.MainDepartmentId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class SysRoleConfiguration : IEntityTypeConfiguration<SysRole>
{
    public void Configure(EntityTypeBuilder<SysRole> b)
    {
        b.ToTable("Sys_Role");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("RoleID");
        b.Property(x => x.Code).HasColumnName("RoleCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("RoleName").HasMaxLength(100).IsRequired();
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsSystem).HasColumnName("IsSystem").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class SysPermissionConfiguration : IEntityTypeConfiguration<SysPermission>
{
    public void Configure(EntityTypeBuilder<SysPermission> b)
    {
        b.ToTable("Sys_Permission");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("PermissionID");
        b.Property(x => x.Code).HasColumnName("PermissionCode").HasMaxLength(100).IsRequired();
        b.Property(x => x.Name).HasColumnName("PermissionName").HasMaxLength(100).IsRequired();
        b.Property(x => x.Module).HasColumnName("Module").HasMaxLength(50).IsRequired();
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class SysUserRoleConfiguration : IEntityTypeConfiguration<SysUserRole>
{
    public void Configure(EntityTypeBuilder<SysUserRole> b)
    {
        b.ToTable("Sys_UserRole");
        b.HasKey(x => new { x.UserId, x.RoleId });
        b.Property(x => x.UserId).HasColumnName("UserID");
        b.Property(x => x.RoleId).HasColumnName("RoleID");
        b.Property(x => x.AssignedAt).HasColumnName("AssignedAt");
        b.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SysRolePermissionConfiguration : IEntityTypeConfiguration<SysRolePermission>
{
    public void Configure(EntityTypeBuilder<SysRolePermission> b)
    {
        b.ToTable("Sys_RolePermission");
        b.HasKey(x => new { x.RoleId, x.PermissionId });
        b.Property(x => x.RoleId).HasColumnName("RoleID");
        b.Property(x => x.PermissionId).HasColumnName("PermissionID");
        b.Property(x => x.AssignedAt).HasColumnName("AssignedAt");
        b.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Permission).WithMany(x => x.RolePermissions).HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SysInstitutionConfiguration : IEntityTypeConfiguration<SysInstitution>
{
    public void Configure(EntityTypeBuilder<SysInstitution> b)
    {
        b.ToTable("Sys_Institution");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("InstitutionID");
        b.Property(x => x.Code).HasColumnName("InstitutionCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("InstitutionName").HasMaxLength(100).IsRequired();
        b.Property(x => x.ParentId).HasColumnName("ParentID");
        b.Property(x => x.InstitutionType).HasColumnName("InstitutionType").HasMaxLength(50);
        b.Property(x => x.Level).HasColumnName("Level").HasDefaultValue(1);
        b.Property(x => x.FullPath).HasColumnName("FullPath").HasMaxLength(500);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class SysDepartmentConfiguration : IEntityTypeConfiguration<SysDepartment>
{
    public void Configure(EntityTypeBuilder<SysDepartment> b)
    {
        b.ToTable("Sys_Department");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("DepartmentID");
        b.Property(x => x.Code).HasColumnName("DepartmentCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("DepartmentName").HasMaxLength(100).IsRequired();
        b.Property(x => x.InstitutionId).HasColumnName("InstitutionID");
        b.Property(x => x.ParentId).HasColumnName("ParentID");
        b.Property(x => x.DepartmentType).HasColumnName("DepartmentType").HasMaxLength(50);
        b.Property(x => x.Level).HasColumnName("Level").HasDefaultValue(1);
        b.Property(x => x.FullPath).HasColumnName("FullPath").HasMaxLength(500);
        b.Property(x => x.ManagerId).HasColumnName("ManagerID");
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Institution).WithMany(x => x.Departments).HasForeignKey(x => x.InstitutionId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.Code).IsUnique();
    }
}
