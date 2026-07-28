using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 身份认证与权限模块：User / Role / Permission / Department / UserRole / RolePermission
/// </summary>
internal static class IdentityModelConfiguration
{
    public static void ConfigureIdentity(this ModelBuilder modelBuilder)
    {
        // ===== 复合主键 =====
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // ===== 唯一索引 =====
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Code).IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code).IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Code).IsUnique();

        // ===== 外键关系 =====

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Department>()
            .HasOne(d => d.Parent)
            .WithMany(d => d.Children)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // ===== 种子数据 =====

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = RootDeptId, Code = "ROOT", Name = "根部门", Description = "系统根部门", IsActive = true, CreatedAt = SeedDate },
            new Department { Id = LabDeptId, Code = "LAB_CENTER", Name = "实验中心", ParentId = RootDeptId, Description = "学校实验中心", IsActive = true, CreatedAt = SeedDate },
            new Department { Id = CsDeptId, Code = "CS_LAB", Name = "计算机实验室", ParentId = LabDeptId, Description = "计算机专业实验室", IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = SuperAdminRoleId, Code = "super_admin", Name = "超级管理员", Description = "系统超级管理员，拥有所有权限", IsSystem = true, IsActive = true, CreatedAt = SeedDate },
            new Role { Id = LabAdminRoleId, Code = "lab_admin", Name = "实验室管理员", Description = "管理实验室、设备、预约", IsSystem = true, IsActive = true, CreatedAt = SeedDate },
            new Role { Id = TeacherRoleId, Code = "teacher", Name = "教师", Description = "课程管理、学生管理", IsSystem = true, IsActive = true, CreatedAt = SeedDate },
            new Role { Id = StudentRoleId, Code = "student", Name = "学生", Description = "预约设备、提交报告", IsSystem = true, IsActive = true, CreatedAt = SeedDate },
            new Role { Id = AuditorRoleId, Code = "auditor", Name = "审计员", Description = "查看日志、报表", IsSystem = true, IsActive = true, CreatedAt = SeedDate }
        );

        var permissions = new List<Permission>
        {
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Code = "user:create", Name = "创建用户", Module = "user", Description = "创建新用户", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Code = "user:read", Name = "查看用户", Module = "user", Description = "查看用户信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Code = "user:update", Name = "编辑用户", Module = "user", Description = "编辑用户信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Code = "user:delete", Name = "删除用户", Module = "user", Description = "删除用户", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Code = "user:reset_password", Name = "重置密码", Module = "user", Description = "重置用户密码", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Code = "role:create", Name = "创建角色", Module = "role", Description = "创建新角色", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Code = "role:read", Name = "查看角色", Module = "role", Description = "查看角色信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Code = "role:update", Name = "编辑角色", Module = "role", Description = "编辑角色信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Code = "role:delete", Name = "删除角色", Module = "role", Description = "删除角色", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Code = "role:assign", Name = "分配角色", Module = "role", Description = "为用户分配角色", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Code = "permission:read", Name = "查看权限", Module = "permission", Description = "查看权限列表", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Code = "permission:assign", Name = "分配权限", Module = "permission", Description = "为角色分配权限", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Code = "department:create", Name = "创建部门", Module = "department", Description = "创建新部门", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Code = "department:read", Name = "查看部门", Module = "department", Description = "查看部门信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Code = "department:update", Name = "编辑部门", Module = "department", Description = "编辑部门信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Code = "department:delete", Name = "删除部门", Module = "department", Description = "删除部门", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Code = "equipment:create", Name = "创建设备", Module = "equipment", Description = "创建新设备", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Code = "equipment:read", Name = "查看设备", Module = "equipment", Description = "查看设备信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Code = "equipment:update", Name = "编辑设备", Module = "equipment", Description = "编辑设备信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000004"), Code = "equipment:delete", Name = "删除设备", Module = "equipment", Description = "删除设备", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("50000000-0000-0000-0000-000000000005"), Code = "equipment:borrow", Name = "借用设备", Module = "equipment", Description = "借用设备", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Code = "lab:create", Name = "创建实验室", Module = "lab", Description = "创建新实验室", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Code = "lab:read", Name = "查看实验室", Module = "lab", Description = "查看实验室信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Code = "lab:update", Name = "编辑实验室", Module = "lab", Description = "编辑实验室信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Code = "lab:delete", Name = "删除实验室", Module = "lab", Description = "删除实验室", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("61000000-0000-0000-0000-000000000001"), Code = "campus:create", Name = "创建校区", Module = "campus", Description = "创建新校区", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("61000000-0000-0000-0000-000000000002"), Code = "campus:read", Name = "查看校区", Module = "campus", Description = "查看校区信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("61000000-0000-0000-0000-000000000003"), Code = "campus:update", Name = "编辑校区", Module = "campus", Description = "编辑校区信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("61000000-0000-0000-0000-000000000004"), Code = "campus:delete", Name = "删除校区", Module = "campus", Description = "删除校区", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000001"), Code = "building:create", Name = "创建楼宇", Module = "building", Description = "创建新楼宇", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000002"), Code = "building:read", Name = "查看楼宇", Module = "building", Description = "查看楼宇信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000003"), Code = "building:update", Name = "编辑楼宇", Module = "building", Description = "编辑楼宇信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000004"), Code = "building:delete", Name = "删除楼宇", Module = "building", Description = "删除楼宇", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Code = "course:create", Name = "创建课程", Module = "course", Description = "创建新课程", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Code = "course:read", Name = "查看课程", Module = "course", Description = "查看课程信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000003"), Code = "course:update", Name = "编辑课程", Module = "course", Description = "编辑课程信息", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000004"), Code = "course:delete", Name = "删除课程", Module = "course", Description = "删除课程", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("70000000-0000-0000-0000-000000000005"), Code = "course:schedule", Name = "排课", Module = "course", Description = "课程排期", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), Code = "report:create", Name = "创建报告", Module = "report", Description = "创建新报告", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), Code = "report:read", Name = "查看报告", Module = "report", Description = "查看报告", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), Code = "report:approve", Name = "审批报告", Module = "report", Description = "审批报告", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("90000000-0000-0000-0000-000000000001"), Code = "system:config", Name = "系统配置", Module = "system", Description = "系统配置管理", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("90000000-0000-0000-0000-000000000002"), Code = "system:log", Name = "查看日志", Module = "system", Description = "查看系统日志", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"), Code = "schedule:read", Name = "查看排课", Module = "schedule", Description = "查看排课记录", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"), Code = "schedule:create", Name = "创建排课", Module = "schedule", Description = "创建排课记录", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"), Code = "schedule:update", Name = "编辑排课", Module = "schedule", Description = "编辑排课记录", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000004"), Code = "schedule:delete", Name = "删除排课", Module = "schedule", Description = "删除排课记录", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000011"), Code = "reservation:read", Name = "查看预约", Module = "reservation", Description = "查看预约申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000012"), Code = "reservation:create", Name = "创建预约", Module = "reservation", Description = "提交预约申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000013"), Code = "reservation:approve", Name = "审批预约", Module = "reservation", Description = "审批预约申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000014"), Code = "reservation:cancel", Name = "取消预约", Module = "reservation", Description = "取消预约", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000021"), Code = "teaching_application:read", Name = "查看授课申请", Module = "teaching_application", Description = "查看授课申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000022"), Code = "teaching_application:create", Name = "创建授课申请", Module = "teaching_application", Description = "提交授课申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000023"), Code = "teaching_application:approve", Name = "审批授课申请", Module = "teaching_application", Description = "审批授课申请", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000031"), Code = "usage_registration:read", Name = "查看使用登记", Module = "usage_registration", Description = "查看使用登记记录", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000032"), Code = "usage_registration:create", Name = "创建使用登记", Module = "usage_registration", Description = "填写使用登记表", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000041"), Code = "statistics:read", Name = "查看统计", Module = "statistics", Description = "查看统计报表", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000042"), Code = "statistics:export", Name = "导出统计", Module = "statistics", Description = "导出统计报表", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a0000000-0000-0000-0000-000000000043"), Code = "statistics:dashboard", Name = "查看大屏", Module = "statistics", Description = "查看可视化大屏", CreatedAt = SeedDate },
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        // 超级管理员拥有所有权限
        var rolePermissions = permissions
            .Select(p => new RolePermission { RoleId = SuperAdminRoleId, PermissionId = p.Id, AssignedAt = SeedDate })
            .ToList();

        var labAdminPermissions = new[]
        {
            "equipment:create", "equipment:read", "equipment:update", "equipment:delete", "equipment:borrow",
            "lab:create", "lab:read", "lab:update", "lab:delete",
            "campus:create", "campus:read", "campus:update", "campus:delete",
            "building:create", "building:read", "building:update", "building:delete",
            "department:read",
            "schedule:read", "schedule:create", "schedule:update", "schedule:delete",
            "reservation:read", "reservation:approve",
            "teaching_application:read", "teaching_application:approve",
            "usage_registration:read",
            "statistics:read", "statistics:export", "statistics:dashboard"
        };
        rolePermissions.AddRange(permissions.Where(p => labAdminPermissions.Contains(p.Code))
            .Select(p => new RolePermission { RoleId = LabAdminRoleId, PermissionId = p.Id, AssignedAt = SeedDate }));

        var teacherPermissions = new[]
        {
            "course:create", "course:read", "course:update", "course:delete", "course:schedule",
            "report:read", "report:approve",
            "equipment:read", "equipment:borrow",
            "lab:read",
            "campus:read", "building:read",
            "schedule:read",
            "reservation:read", "reservation:create",
            "teaching_application:read", "teaching_application:create",
            "usage_registration:read", "usage_registration:create",
            "statistics:read"
        };
        rolePermissions.AddRange(permissions.Where(p => teacherPermissions.Contains(p.Code))
            .Select(p => new RolePermission { RoleId = TeacherRoleId, PermissionId = p.Id, AssignedAt = SeedDate }));

        var studentPermissions = new[]
        {
            "course:read", "report:create", "report:read",
            "equipment:read", "equipment:borrow",
            "lab:read",
            "campus:read", "building:read"
        };
        rolePermissions.AddRange(permissions.Where(p => studentPermissions.Contains(p.Code))
            .Select(p => new RolePermission { RoleId = StudentRoleId, PermissionId = p.Id, AssignedAt = SeedDate }));

        var auditorPermissions = new[]
        {
            "user:read", "role:read", "permission:read",
            "equipment:read", "lab:read", "course:read", "report:read",
            "campus:read", "building:read", "system:log"
        };
        rolePermissions.AddRange(permissions.Where(p => auditorPermissions.Contains(p.Code))
            .Select(p => new RolePermission { RoleId = AuditorRoleId, PermissionId = p.Id, AssignedAt = SeedDate }));

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

        modelBuilder.Entity<User>().HasData(
            new User { Id = AdminUserId, Username = "admin", PasswordHash = "$2a$11$a5KJRw3O4upDmqpwnebX8O7loyuih3XazDAGmyI.Kc1fU5JWjC2/q", Email = "admin@example.com", Phone = "13800000001", FullName = "系统管理员", DepartmentId = RootDeptId, IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new User { Id = TeacherUserId, Username = "teacher", PasswordHash = "$2a$11$YbtazoV7p.D9yY0emOL.eOuWHuO.ndgnAss35YrclPacrF1HniQpa", Email = "teacher@example.com", Phone = "13800000002", FullName = "张老师", DepartmentId = CsDeptId, IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new User { Id = StudentUserId, Username = "student", PasswordHash = "$2a$11$wFAB.xkQQWWN9EggYXoZEOt2LilHpWZwIDc.6WXfNyiPK.KfBQ9A6", Email = "student@example.com", Phone = "13800000003", FullName = "李同学", DepartmentId = CsDeptId, IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = AdminUserId, RoleId = SuperAdminRoleId, AssignedAt = SeedDate },
            new UserRole { UserId = TeacherUserId, RoleId = TeacherRoleId, AssignedAt = SeedDate },
            new UserRole { UserId = StudentUserId, RoleId = StudentRoleId, AssignedAt = SeedDate }
        );
    }
}
