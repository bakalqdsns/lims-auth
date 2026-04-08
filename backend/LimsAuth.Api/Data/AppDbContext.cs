using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Department> Departments => Set<Department>();

    // 教学管理相关
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<AcademicCalendar> AcademicCalendars => Set<AcademicCalendar>();
    public DbSet<CalendarTemplate> CalendarTemplates => Set<CalendarTemplate>();
    public DbSet<SemesterLog> SemesterLogs => Set<SemesterLog>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
    public DbSet<TeachingTask> TeachingTasks => Set<TeachingTask>();
    public DbSet<TeachingTaskTeacher> TeachingTaskTeachers => Set<TeachingTaskTeacher>();
    public DbSet<PeriodTime> PeriodTimes => Set<PeriodTime>();

    // 实验室设备管理
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<Equipment> Equipments => Set<Equipment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置复合主键
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // 配置唯一索引
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Code)
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Code)
            .IsUnique();

        // 配置外键关系
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

        // 教学管理 - 复合主键
        modelBuilder.Entity<ClassStudent>()
            .HasKey(cs => new { cs.ClassId, cs.StudentId });

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasKey(ttt => new { ttt.TeachingTaskId, ttt.TeacherId });

        // 教学管理 - 外键关系
        modelBuilder.Entity<AcademicCalendar>()
            .HasOne(ac => ac.Semester)
            .WithMany(s => s.CalendarDays)
            .HasForeignKey(ac => ac.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Manager)
            .WithMany()
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Major>()
            .HasOne(m => m.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.Major)
            .WithMany(m => m.Classes)
            .HasForeignKey(c => c.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.HeadTeacher)
            .WithMany()
            .HasForeignKey(c => c.HeadTeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.AdminStudent)
            .WithMany()
            .HasForeignKey(c => c.AdminStudentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ClassStudent>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.ClassStudents)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClassStudent>()
            .HasOne(cs => cs.Student)
            .WithMany()
            .HasForeignKey(cs => cs.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Semester)
            .WithMany(s => s.TeachingTasks)
            .HasForeignKey(tt => tt.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Course)
            .WithMany(c => c.TeachingTasks)
            .HasForeignKey(tt => tt.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Class)
            .WithMany(c => c.TeachingTasks)
            .HasForeignKey(tt => tt.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasOne(ttt => ttt.TeachingTask)
            .WithMany(tt => tt.Teachers)
            .HasForeignKey(ttt => ttt.TeachingTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasOne(ttt => ttt.Teacher)
            .WithMany()
            .HasForeignKey(ttt => ttt.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // 学期管理 - 外键关系
        modelBuilder.Entity<Semester>()
            .HasOne(s => s.ParentSemester)
            .WithMany(s => s.ChildSemesters)
            .HasForeignKey(s => s.ParentSemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // 唯一索引
        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => new { s.IsCurrent, s.IsActive });

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.Date })
            .IsUnique();

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.WeekNumber });

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => ac.EventType);

        modelBuilder.Entity<CalendarTemplate>()
            .HasIndex(ct => ct.IsDefault);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Major>()
            .HasIndex(m => m.Code)
            .IsUnique();

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<PeriodTime>()
            .HasIndex(pt => pt.PeriodNumber)
            .IsUnique();

        // 实验室设备管理 - 外键关系
        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Department)
            .WithMany()
            .HasForeignKey(l => l.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Manager)
            .WithMany()
            .HasForeignKey(l => l.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Lab)
            .WithMany(l => l.Equipments)
            .HasForeignKey(e => e.LabId)
            .OnDelete(DeleteBehavior.SetNull);

        // 唯一索引
        modelBuilder.Entity<Lab>()
            .HasIndex(l => l.Code)
            .IsUnique();

        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Code)
            .IsUnique();

        // SQLite seed data
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0);

        // 种子数据 - 部门
        var rootDeptId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var labDeptId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var csDeptId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Department>().HasData(
            new Department
            {
                Id = rootDeptId,
                Code = "ROOT",
                Name = "根部门",
                Description = "系统根部门",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Department
            {
                Id = labDeptId,
                Code = "LAB_CENTER",
                Name = "实验中心",
                ParentId = rootDeptId,
                Description = "学校实验中心",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Department
            {
                Id = csDeptId,
                Code = "CS_LAB",
                Name = "计算机实验室",
                ParentId = labDeptId,
                Description = "计算机专业实验室",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 角色
        var superAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var labAdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var teacherRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var studentRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var auditorRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = superAdminRoleId,
                Code = "super_admin",
                Name = "超级管理员",
                Description = "系统超级管理员，拥有所有权限",
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedDate
            },
            new Role
            {
                Id = labAdminRoleId,
                Code = "lab_admin",
                Name = "实验室管理员",
                Description = "管理实验室、设备、预约",
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedDate
            },
            new Role
            {
                Id = teacherRoleId,
                Code = "teacher",
                Name = "教师",
                Description = "课程管理、学生管理",
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedDate
            },
            new Role
            {
                Id = studentRoleId,
                Code = "student",
                Name = "学生",
                Description = "预约设备、提交报告",
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedDate
            },
            new Role
            {
                Id = auditorRoleId,
                Code = "auditor",
                Name = "审计员",
                Description = "查看日志、报表",
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 权限
        var permissions = new List<Permission>
        {
            // 用户管理权限
            new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Code = "user:create", Name = "创建用户", Module = "user", Description = "创建新用户", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Code = "user:read", Name = "查看用户", Module = "user", Description = "查看用户信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Code = "user:update", Name = "编辑用户", Module = "user", Description = "编辑用户信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Code = "user:delete", Name = "删除用户", Module = "user", Description = "删除用户", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Code = "user:reset_password", Name = "重置密码", Module = "user", Description = "重置用户密码", CreatedAt = seedDate },

            // 角色管理权限
            new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Code = "role:create", Name = "创建角色", Module = "role", Description = "创建新角色", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Code = "role:read", Name = "查看角色", Module = "role", Description = "查看角色信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Code = "role:update", Name = "编辑角色", Module = "role", Description = "编辑角色信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Code = "role:delete", Name = "删除角色", Module = "role", Description = "删除角色", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Code = "role:assign", Name = "分配角色", Module = "role", Description = "为用户分配角色", CreatedAt = seedDate },

            // 权限管理权限
            new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Code = "permission:read", Name = "查看权限", Module = "permission", Description = "查看权限列表", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Code = "permission:assign", Name = "分配权限", Module = "permission", Description = "为角色分配权限", CreatedAt = seedDate },

            // 部门管理权限
            new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Code = "department:create", Name = "创建部门", Module = "department", Description = "创建新部门", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Code = "department:read", Name = "查看部门", Module = "department", Description = "查看部门信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Code = "department:update", Name = "编辑部门", Module = "department", Description = "编辑部门信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Code = "department:delete", Name = "删除部门", Module = "department", Description = "删除部门", CreatedAt = seedDate },

            // 设备管理权限
            new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Code = "equipment:create", Name = "创建设备", Module = "equipment", Description = "创建新设备", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Code = "equipment:read", Name = "查看设备", Module = "equipment", Description = "查看设备信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Code = "equipment:update", Name = "编辑设备", Module = "equipment", Description = "编辑设备信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000004"), Code = "equipment:delete", Name = "删除设备", Module = "equipment", Description = "删除设备", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000005"), Code = "equipment:borrow", Name = "借用设备", Module = "equipment", Description = "借用设备", CreatedAt = seedDate },

            // 实验室管理权限
            new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Code = "lab:create", Name = "创建实验室", Module = "lab", Description = "创建新实验室", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Code = "lab:read", Name = "查看实验室", Module = "lab", Description = "查看实验室信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Code = "lab:update", Name = "编辑实验室", Module = "lab", Description = "编辑实验室信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Code = "lab:delete", Name = "删除实验室", Module = "lab", Description = "删除实验室", CreatedAt = seedDate },

            // 课程管理权限
            new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Code = "course:create", Name = "创建课程", Module = "course", Description = "创建新课程", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Code = "course:read", Name = "查看课程", Module = "course", Description = "查看课程信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000003"), Code = "course:update", Name = "编辑课程", Module = "course", Description = "编辑课程信息", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000004"), Code = "course:delete", Name = "删除课程", Module = "course", Description = "删除课程", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000005"), Code = "course:schedule", Name = "排课", Module = "course", Description = "课程排期", CreatedAt = seedDate },

            // 报告管理权限
            new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), Code = "report:create", Name = "创建报告", Module = "report", Description = "创建新报告", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), Code = "report:read", Name = "查看报告", Module = "report", Description = "查看报告", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), Code = "report:approve", Name = "审批报告", Module = "report", Description = "审批报告", CreatedAt = seedDate },

            // 系统管理权限
            new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000001"), Code = "system:config", Name = "系统配置", Module = "system", Description = "系统配置管理", CreatedAt = seedDate },
            new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000002"), Code = "system:log", Name = "查看日志", Module = "system", Description = "查看系统日志", CreatedAt = seedDate },
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        // 种子数据 - 角色权限关联 (超级管理员拥有所有权限)
        var rolePermissions = permissions.Select(p => new RolePermission
        {
            RoleId = superAdminRoleId,
            PermissionId = p.Id,
            AssignedAt = seedDate
        }).ToList();

        // 实验室管理员权限
        var labAdminPermissions = new[]
        {
            "equipment:create", "equipment:read", "equipment:update", "equipment:delete", "equipment:borrow",
            "lab:create", "lab:read", "lab:update", "lab:delete",
            "department:read"
        };
        rolePermissions.AddRange(permissions
            .Where(p => labAdminPermissions.Contains(p.Code))
            .Select(p => new RolePermission
            {
                RoleId = labAdminRoleId,
                PermissionId = p.Id,
                AssignedAt = seedDate
            }));

        // 教师权限
        var teacherPermissions = new[]
        {
            "course:create", "course:read", "course:update", "course:delete", "course:schedule",
            "report:read", "report:approve",
            "equipment:read", "equipment:borrow",
            "lab:read"
        };
        rolePermissions.AddRange(permissions
            .Where(p => teacherPermissions.Contains(p.Code))
            .Select(p => new RolePermission
            {
                RoleId = teacherRoleId,
                PermissionId = p.Id,
                AssignedAt = seedDate
            }));

        // 学生权限
        var studentPermissions = new[]
        {
            "course:read",
            "report:create", "report:read",
            "equipment:read", "equipment:borrow",
            "lab:read"
        };
        rolePermissions.AddRange(permissions
            .Where(p => studentPermissions.Contains(p.Code))
            .Select(p => new RolePermission
            {
                RoleId = studentRoleId,
                PermissionId = p.Id,
                AssignedAt = seedDate
            }));

        // 审计员权限
        var auditorPermissions = new[]
        {
            "user:read", "role:read", "permission:read",
            "equipment:read", "lab:read", "course:read", "report:read",
            "system:log"
        };
        rolePermissions.AddRange(permissions
            .Where(p => auditorPermissions.Contains(p.Code))
            .Select(p => new RolePermission
            {
                RoleId = auditorRoleId,
                PermissionId = p.Id,
                AssignedAt = seedDate
            }));

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

        // 种子数据 - 用户
        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var teacherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var studentUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Username = "admin",
                PasswordHash = "$2a$11$a5KJRw3O4upDmqpwnebX8O7loyuih3XazDAGmyI.Kc1fU5JWjC2/q",
                Email = "admin@example.com",
                Phone = "13800000001",
                FullName = "系统管理员",
                DepartmentId = rootDeptId,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new User
            {
                Id = teacherUserId,
                Username = "teacher",
                PasswordHash = "$2a$11$YbtazoV7p.D9yY0emOL.eOuWHuO.ndgnAss35YrclPacrF1HniQpa",
                Email = "teacher@example.com",
                Phone = "13800000002",
                FullName = "张老师",
                DepartmentId = csDeptId,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new User
            {
                Id = studentUserId,
                Username = "student",
                PasswordHash = "$2a$11$wFAB.xkQQWWN9EggYXoZEOt2LilHpWZwIDc.6WXfNyiPK.KfBQ9A6",
                Email = "student@example.com",
                Phone = "13800000003",
                FullName = "李同学",
                DepartmentId = csDeptId,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // 种子数据 - 用户角色关联
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = adminUserId, RoleId = superAdminRoleId, AssignedAt = seedDate },
            new UserRole { UserId = teacherUserId, RoleId = teacherRoleId, AssignedAt = seedDate },
            new UserRole { UserId = studentUserId, RoleId = studentRoleId, AssignedAt = seedDate }
        );

        // ========== 教学管理种子数据 ==========

        // 种子数据 - 学期（完整版）
        var semesterId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        modelBuilder.Entity<Semester>().HasData(
            new Semester
            {
                Id = semesterId,
                Name = "2024-2025学年第一学期",
                Code = "2024-2025-1",
                AcademicYear = "2024-2025",
                SemesterType = SemesterType.Regular,
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 19),
                TeachingStartDate = new DateTime(2024, 9, 2),
                TeachingEndDate = new DateTime(2025, 1, 10),
                TotalWeeks = 20,
                TeachingWeeks = 18,
                // 选课时间
                CourseSelectionStart = new DateTime(2024, 8, 20),
                CourseSelectionEnd = new DateTime(2024, 9, 5),
                CourseSelectionEndWithdraw = new DateTime(2024, 9, 15),
                // 排课时间
                SchedulingStart = new DateTime(2024, 7, 1),
                SchedulingEnd = new DateTime(2024, 8, 15),
                SchedulePublishTime = new DateTime(2024, 8, 25),
                // 考试时间
                ExamWeekStart = new DateTime(2025, 1, 6),
                ExamWeekEnd = new DateTime(2025, 1, 17),
                GradeEntryStart = new DateTime(2025, 1, 6),
                GradeEntryEnd = new DateTime(2025, 1, 24),
                GradePublishTime = new DateTime(2025, 1, 26),
                // 注册缴费
                RegistrationStart = new DateTime(2024, 8, 25),
                RegistrationEnd = new DateTime(2024, 9, 1),
                TuitionPaymentStart = new DateTime(2024, 8, 20),
                TuitionPaymentEnd = new DateTime(2024, 9, 5),
                // 状态
                Status = SemesterStatus.InProgress,
                IsCurrent = true,
                IsActive = true,
                IsEditable = true,
                IsDeletable = false,
                Description = "2024-2025学年第一学期（秋季学期）",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // 种子数据 - 校历模板
        modelBuilder.Entity<CalendarTemplate>().HasData(
            new CalendarTemplate
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Name = "标准学期模板（20周）",
                Description = "适用于常规春秋学期，包含18周教学+2周考试",
                IsDefault = true,
                TemplateData = "{\"totalWeeks\":20,\"teachingWeeks\":18,\"examWeeks\":2,\"schedule\":[{\"week\":1,\"type\":\"teaching\"},{\"week\":19,\"type\":\"exam\"},{\"week\":20,\"type\":\"exam\"}]}",
                IsActive = true,
                CreatedAt = seedDate
            },
            new CalendarTemplate
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Name = "短学期模板（4周）",
                Description = "适用于小学期或暑期课程",
                IsDefault = false,
                TemplateData = "{\"totalWeeks\":4,\"teachingWeeks\":3,\"examWeeks\":1}",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 节次时间
        modelBuilder.Entity<PeriodTime>().HasData(
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000001"), PeriodNumber = 1, Name = "第1-2节", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 40, 0), IsActive = true, CreatedAt = seedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000002"), PeriodNumber = 2, Name = "第3-4节", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 40, 0), IsActive = true, CreatedAt = seedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000003"), PeriodNumber = 3, Name = "第5-6节", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 40, 0), IsActive = true, CreatedAt = seedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000004"), PeriodNumber = 4, Name = "第7-8节", StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 40, 0), IsActive = true, CreatedAt = seedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000005"), PeriodNumber = 5, Name = "第9-10节", StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 40, 0), IsActive = true, CreatedAt = seedDate }
        );

        // 种子数据 - 专业
        var majorId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        modelBuilder.Entity<Major>().HasData(
            new Major
            {
                Id = majorId,
                Code = "CS",
                Name = "计算机科学与技术",
                DepartmentId = csDeptId,
                Description = "计算机科学与技术专业",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 班级
        var classId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        modelBuilder.Entity<Class>().HasData(
            new Class
            {
                Id = classId,
                Code = "CS202401",
                Name = "计算机科学与技术2024级1班",
                Grade = "2024",
                MajorId = majorId,
                DepartmentId = csDeptId,
                HeadTeacherId = teacherUserId,
                StudentCount = 30,
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 班级学生关联
        modelBuilder.Entity<ClassStudent>().HasData(
            new ClassStudent { ClassId = classId, StudentId = studentUserId, JoinedAt = seedDate }
        );

        // 种子数据 - 课程
        var courseId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = courseId,
                Code = "CS101",
                Name = "程序设计基础",
                EnglishName = "Fundamentals of Programming",
                CourseType = "必修",
                Credits = 4,
                TotalHours = 64,
                TheoryHours = 32,
                PracticeHours = 16,
                ExperimentHours = 16,
                OnlineHours = 0,
                SemesterType = 1,
                DepartmentId = csDeptId,
                Description = "计算机专业基础课程",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 教学任务
        var taskId = Guid.Parse("22222222-3333-4444-5555-666666666666");
        modelBuilder.Entity<TeachingTask>().HasData(
            new TeachingTask
            {
                Id = taskId,
                SemesterId = semesterId,
                CourseId = courseId,
                ClassId = classId,
                TaskType = "主讲",
                Description = "程序设计基础教学任务",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 教学任务教师关联
        modelBuilder.Entity<TeachingTaskTeacher>().HasData(
            new TeachingTaskTeacher { TeachingTaskId = taskId, TeacherId = teacherUserId, IsMainTeacher = true, AssignedAt = seedDate }
        );

        // ========== 实验室和设备种子数据 ==========

        // 种子数据 - 实验室
        var lab1Id = Guid.Parse("f0000000-0000-0000-0000-000000000001");
        var lab2Id = Guid.Parse("f0000000-0000-0000-0000-000000000002");
        var lab3Id = Guid.Parse("f0000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Lab>().HasData(
            new Lab
            {
                Id = lab1Id,
                Code = "LAB001",
                Name = "计算机基础实验室",
                DepartmentId = csDeptId,
                Location = "实验楼A101",
                Capacity = 60,
                LabType = "计算机实验室",
                SafetyLevel = "一般",
                ManagerId = teacherUserId,
                Description = "配备高性能计算机，用于程序设计、数据结构等课程实验",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Lab
            {
                Id = lab2Id,
                Code = "LAB002",
                Name = "网络工程实验室",
                DepartmentId = csDeptId,
                Location = "实验楼A102",
                Capacity = 40,
                LabType = "网络实验室",
                SafetyLevel = "一般",
                ManagerId = teacherUserId,
                Description = "配备网络交换机和路由器，用于计算机网络课程实验",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Lab
            {
                Id = lab3Id,
                Code = "LAB003",
                Name = "嵌入式系统实验室",
                DepartmentId = csDeptId,
                Location = "实验楼A103",
                Capacity = 30,
                LabType = "嵌入式实验室",
                SafetyLevel = "较高",
                ManagerId = teacherUserId,
                Description = "配备嵌入式开发板和示波器，用于嵌入式系统课程实验",
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // 种子数据 - 设备
        modelBuilder.Entity<Equipment>().HasData(
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"),
                Code = "PC001",
                Name = "高性能计算机",
                Model = "Dell OptiPlex 7090",
                Manufacturer = "Dell",
                SerialNumber = "SN123456789",
                LabId = lab1Id,
                Category = "计算机设备",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 9, 1),
                WarrantyMonths = 36,
                Price = 8000,
                Location = "实验楼A101-01",
                RequiresBooking = false,
                Description = "i7处理器，32GB内存，512GB SSD",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000002"),
                Code = "PC002",
                Name = "高性能计算机",
                Model = "Dell OptiPlex 7090",
                Manufacturer = "Dell",
                SerialNumber = "SN123456790",
                LabId = lab1Id,
                Category = "计算机设备",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 9, 1),
                WarrantyMonths = 36,
                Price = 8000,
                Location = "实验楼A101-02",
                RequiresBooking = false,
                Description = "i7处理器，32GB内存，512GB SSD",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000003"),
                Code = "SW001",
                Name = "三层交换机",
                Model = "H3C S5120V3-28P-SI",
                Manufacturer = "H3C",
                SerialNumber = "SN987654321",
                LabId = lab2Id,
                Category = "网络设备",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 6, 15),
                WarrantyMonths = 24,
                Price = 5000,
                Location = "实验楼A102机柜A",
                RequiresBooking = true,
                MaxBookingHours = 4,
                Description = "24口千兆交换机，支持VLAN和路由功能",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000004"),
                Code = "RT001",
                Name = "企业级路由器",
                Model = "H3C MSR3600-28",
                Manufacturer = "H3C",
                SerialNumber = "SN987654322",
                LabId = lab2Id,
                Category = "网络设备",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 6, 15),
                WarrantyMonths = 24,
                Price = 12000,
                Location = "实验楼A102机柜A",
                RequiresBooking = true,
                MaxBookingHours = 4,
                Description = "多业务路由器，支持多种路由协议",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000005"),
                Code = "OSC001",
                Name = "数字示波器",
                Model = "Rigol DS1054Z",
                Manufacturer = "Rigol",
                SerialNumber = "SN555566667",
                LabId = lab3Id,
                Category = "测试仪器",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 3, 10),
                WarrantyMonths = 24,
                Price = 3500,
                Location = "实验楼A103仪器柜",
                RequiresBooking = true,
                MaxBookingHours = 2,
                Description = "4通道，50MHz带宽，1GSa/s采样率",
                IsActive = true,
                CreatedAt = seedDate
            },
            new Equipment
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000006"),
                Code = "DEV001",
                Name = "STM32开发板",
                Model = "STM32F407VGT6",
                Manufacturer = "ST",
                SerialNumber = "SN777788889",
                LabId = lab3Id,
                Category = "开发板",
                Status = "正常",
                PurchaseDate = new DateTime(2023, 9, 1),
                WarrantyMonths = 12,
                Price = 200,
                Location = "实验楼A103储物柜",
                RequiresBooking = false,
                Description = "ARM Cortex-M4内核，1MB Flash，192KB SRAM",
                IsActive = true,
                CreatedAt = seedDate
            }
        );
    }
}
