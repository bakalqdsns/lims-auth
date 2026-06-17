using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data;

public static class SeedData
{
    public static readonly Dictionary<string, string> ModuleNames = new()
    {
        ["user"] = "用户", ["role"] = "角色", ["permission"] = "权限",
        ["institution"] = "机构", ["department"] = "部门",
        ["semester"] = "学期", ["course"] = "课程", ["major"] = "专业",
        ["class"] = "班级", ["teachingtask"] = "教学任务",
        ["building"] = "楼宇", ["room"] = "实验室",
        ["schedule"] = "排课", ["booking"] = "预约", ["usage"] = "使用记录",
        ["asset"] = "设备", ["loan"] = "借还",
        ["consumable"] = "耗材", ["experimentitem"] = "实验项目"
    };

    public static readonly string[] AllPermissionCodes = new[]
    {
        "user:create", "user:read", "user:update", "user:delete",
        "role:create", "role:read", "role:update", "role:delete",
        "permission:read", "permission:assign",
        "institution:create", "institution:read", "institution:update", "institution:delete",
        "department:create", "department:read", "department:update", "department:delete",
        "semester:create", "semester:read", "semester:update", "semester:delete",
        "course:create", "course:read", "course:update", "course:delete",
        "major:create", "major:read", "major:update", "major:delete",
        "class:create", "class:read", "class:update", "class:delete",
        "teachingtask:create", "teachingtask:read", "teachingtask:update", "teachingtask:delete",
        "building:create", "building:read", "building:update", "building:delete",
        "room:create", "room:read", "room:update", "room:delete",
        "schedule:create", "schedule:read", "schedule:update", "schedule:delete",
        "booking:create", "booking:read", "booking:approve", "booking:delete",
        "usage:read", "usage:create", "usage:delete",
        "asset:create", "asset:read", "asset:update", "asset:delete",
        "loan:create", "loan:read", "loan:approve",
        "consumable:create", "consumable:read", "consumable:update", "consumable:delete", "consumable:approve",
        "experimentitem:create", "experimentitem:read", "experimentitem:update", "experimentitem:delete",
    };

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.SysPermissions.AnyAsync()) return;

        // ============================================================
        // 1. Permissions
        // ============================================================
        var actionNames = new Dictionary<string, string>
        {
            ["create"] = "创建", ["read"] = "查看", ["update"] = "编辑",
            ["delete"] = "删除", ["assign"] = "分配", ["approve"] = "审批"
        };
        var permissions = AllPermissionCodes.Select(code =>
        {
            var parts = code.Split(':');
            var module = parts[0];
            var action = parts[1];
            var name = $"{actionNames.GetValueOrDefault(action, action)}{ModuleNames.GetValueOrDefault(module, module)}";
            return new SysPermission { Id = Guid.NewGuid(), Code = code, Name = name, Module = module };
        }).ToList();

        db.SysPermissions.AddRange(permissions);
        await db.SaveChangesAsync();

        // ============================================================
        // 2. Roles
        // ============================================================
        var superAdminId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var roles = new List<SysRole>
        {
            new() { Id = superAdminId, Code = "super_admin", Name = "超级管理员", Description = "系统超级管理员，拥有所有权限", IsSystem = 1, Status = 1 },
            new() { Id = teacherId, Code = "teacher", Name = "教师", Description = "课程管理、学生管理", IsSystem = 1, Status = 1 },
            new() { Id = studentId, Code = "student", Name = "学生", Description = "查看课程、提交预约", IsSystem = 1, Status = 1 },
        };
        db.SysRoles.AddRange(roles);
        await db.SaveChangesAsync();

        // ============================================================
        // 3. Role Permissions (super_admin gets all)
        // ============================================================
        var rolePermissions = permissions.Select(p => new SysRolePermission
        {
            RoleId = superAdminId,
            PermissionId = p.Id
        }).ToList();
        db.SysRolePermissions.AddRange(rolePermissions);

        // Teacher gets read permissions + some write permissions
        var teacherPerms = permissions.Where(p => p.Code.EndsWith(":read") || p.Module is "course" or "booking" or "schedule" or "experimentitem" or "loan").ToList();
        var teacherRolePerms = teacherPerms.Select(p => new SysRolePermission { RoleId = teacherId, PermissionId = p.Id }).ToList();
        db.SysRolePermissions.AddRange(teacherRolePerms);

        // Student gets basic read permissions
        var studentPerms = permissions.Where(p => p.Code.EndsWith(":read") || p.Code == "booking:create" || p.Code == "consumable:read").ToList();
        var studentRolePerms = studentPerms.Select(p => new SysRolePermission { RoleId = studentId, PermissionId = p.Id }).ToList();
        db.SysRolePermissions.AddRange(studentRolePerms);
        await db.SaveChangesAsync();

        // ============================================================
        // 4. Institution
        // ============================================================
        var universityId = Guid.NewGuid();
        var collegeId = Guid.NewGuid();

        db.SysInstitutions.AddRange(new List<SysInstitution>
        {
            new() { Id = universityId, Code = "UNIV001", Name = "测试大学", InstitutionType = "University", Level = 1, Status = 1 },
            new() { Id = collegeId, Code = "CS001", Name = "计算机学院", InstitutionType = "College", ParentId = universityId, Level = 2, Status = 1 },
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 5. Department
        // ============================================================
        var deptId = Guid.NewGuid();
        db.SysDepartments.Add(new SysDepartment
        {
            Id = deptId,
            Code = "DEPT001",
            Name = "计算机系",
            InstitutionId = collegeId,
            Level = 1,
            Status = 1
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 6. Users
        // ============================================================
        var adminId = Guid.NewGuid();
        var teacherUserId = Guid.NewGuid();
        var studentUserId = Guid.NewGuid();

        var users = new List<SysUser>
        {
            new() { Id = adminId, Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin123"), RealName = "系统管理员", UserType = "Admin", Status = 1, MainInstitutionId = universityId, MainDepartmentId = deptId },
            new() { Id = teacherUserId, Username = "teacher", Password = BCrypt.Net.BCrypt.HashPassword("teacher123"), RealName = "张老师", EmployeeNo = "T001", UserType = "Teacher", Status = 1, MainInstitutionId = universityId, MainDepartmentId = deptId },
            new() { Id = studentUserId, Username = "student", Password = BCrypt.Net.BCrypt.HashPassword("student123"), RealName = "李同学", EmployeeNo = "S001", UserType = "Student", Status = 1, MainInstitutionId = universityId, MainDepartmentId = deptId },
        };
        db.SysUsers.AddRange(users);
        await db.SaveChangesAsync();

        // User Roles
        db.SysUserRoles.AddRange(new List<SysUserRole>
        {
            new() { UserId = adminId, RoleId = superAdminId },
            new() { UserId = teacherUserId, RoleId = teacherId },
            new() { UserId = studentUserId, RoleId = studentId },
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 7. Semester
        // ============================================================
        var semesterId = Guid.NewGuid();
        db.EduSemesters.Add(new EduSemester
        {
            Id = semesterId,
            Code = "2025-1",
            Name = "2024-2025学年 第一学期",
            SchoolYear = "2024-2025",
            SemesterNo = 1,
            StartDate = new DateTime(2025, 9, 1),
            EndDate = new DateTime(2026, 1, 15),
            TotalWeeks = 20,
            IsCurrent = 1,
            Status = 1
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 8. Courses
        // ============================================================
        var course1Id = Guid.NewGuid();
        var course2Id = Guid.NewGuid();
        db.EduCourses.AddRange(new List<EduCourse>
        {
            new() { Id = course1Id, Code = "CS101", Name = "数据结构", Credits = 4, TotalHours = 64, LectureHours = 32, LabHours = 32, Status = 1 },
            new() { Id = course2Id, Code = "CS201", Name = "算法设计", Credits = 3, TotalHours = 48, LectureHours = 24, LabHours = 24, Status = 1 },
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 9. Majors
        // ============================================================
        var majorId = Guid.NewGuid();
        db.EduMajors.Add(new EduMajor
        {
            Id = majorId,
            Code = "CS",
            Name = "计算机科学与技术",
            InstitutionId = collegeId,
            DepartmentId = deptId,
            DegreeLevel = "本科",
            Duration = 4,
            DegreeName = "工学学士",
            Status = 1
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 10. Classes
        // ============================================================
        var classId = Guid.NewGuid();
        db.EduClasses.Add(new EduClass
        {
            Id = classId,
            Code = "CS2021-1",
            Name = "计算机21级1班",
            InstitutionId = collegeId,
            DepartmentId = deptId,
            MajorId = majorId,
            GradeName = "2021",
            HeadTeacherId = teacherUserId,
            StudentCount = 30,
            Status = 1
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 11. Buildings
        // ============================================================
        var buildingId = Guid.NewGuid();
        db.VenBuildings.Add(new VenBuilding
        {
            Id = buildingId,
            Code = "BLD001",
            Name = "理学楼A",
            InstitutionId = universityId,
            Address = "测试大学理学楼A",
            TotalFloors = 5,
            Status = 1
        });
        await db.SaveChangesAsync();

        // ============================================================
        // 12. Rooms
        // ============================================================
        var room1Id = Guid.NewGuid();
        var room2Id = Guid.NewGuid();
        db.LabRooms.AddRange(new List<LabRoom>
        {
            new() { Id = room1Id, Code = "LAB101", Name = "计算机实验室101", BuildingId = buildingId, FloorNo = 1, SeatCount = 40, RoomType = "计算机实验室", Status = 1 },
            new() { Id = room2Id, Code = "LAB102", Name = "计算机实验室102", BuildingId = buildingId, FloorNo = 1, SeatCount = 40, RoomType = "计算机实验室", Status = 1 },
        });
        await db.SaveChangesAsync();
    }
}
