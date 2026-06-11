using BCrypt.Net;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.SysPermissions.AnyAsync()) return;

        // ============================================================
        // 1. Permissions
        // ============================================================
        var permissions = new List<SysPermission>
        {
            // User
            new() { Id = Guid.NewGuid(), Code = "user:create", Name = "创建用户", Module = "user" },
            new() { Id = Guid.NewGuid(), Code = "user:read", Name = "查看用户", Module = "user" },
            new() { Id = Guid.NewGuid(), Code = "user:update", Name = "编辑用户", Module = "user" },
            new() { Id = Guid.NewGuid(), Code = "user:delete", Name = "删除用户", Module = "user" },
            // Role
            new() { Id = Guid.NewGuid(), Code = "role:create", Name = "创建角色", Module = "role" },
            new() { Id = Guid.NewGuid(), Code = "role:read", Name = "查看角色", Module = "role" },
            new() { Id = Guid.NewGuid(), Code = "role:update", Name = "编辑角色", Module = "role" },
            new() { Id = Guid.NewGuid(), Code = "role:delete", Name = "删除角色", Module = "role" },
            // Permission
            new() { Id = Guid.NewGuid(), Code = "permission:read", Name = "查看权限", Module = "permission" },
            new() { Id = Guid.NewGuid(), Code = "permission:assign", Name = "分配权限", Module = "permission" },
            // Institution
            new() { Id = Guid.NewGuid(), Code = "institution:create", Name = "创建机构", Module = "institution" },
            new() { Id = Guid.NewGuid(), Code = "institution:read", Name = "查看机构", Module = "institution" },
            new() { Id = Guid.NewGuid(), Code = "institution:update", Name = "编辑机构", Module = "institution" },
            new() { Id = Guid.NewGuid(), Code = "institution:delete", Name = "删除机构", Module = "institution" },
            // Department
            new() { Id = Guid.NewGuid(), Code = "department:create", Name = "创建部门", Module = "department" },
            new() { Id = Guid.NewGuid(), Code = "department:read", Name = "查看部门", Module = "department" },
            new() { Id = Guid.NewGuid(), Code = "department:update", Name = "编辑部门", Module = "department" },
            new() { Id = Guid.NewGuid(), Code = "department:delete", Name = "删除部门", Module = "department" },
            // Semester
            new() { Id = Guid.NewGuid(), Code = "semester:create", Name = "创建学期", Module = "semester" },
            new() { Id = Guid.NewGuid(), Code = "semester:read", Name = "查看学期", Module = "semester" },
            new() { Id = Guid.NewGuid(), Code = "semester:update", Name = "编辑学期", Module = "semester" },
            new() { Id = Guid.NewGuid(), Code = "semester:delete", Name = "删除学期", Module = "semester" },
            // Course
            new() { Id = Guid.NewGuid(), Code = "course:create", Name = "创建课程", Module = "course" },
            new() { Id = Guid.NewGuid(), Code = "course:read", Name = "查看课程", Module = "course" },
            new() { Id = Guid.NewGuid(), Code = "course:update", Name = "编辑课程", Module = "course" },
            new() { Id = Guid.NewGuid(), Code = "course:delete", Name = "删除课程", Module = "course" },
            // Major
            new() { Id = Guid.NewGuid(), Code = "major:create", Name = "创建专业", Module = "major" },
            new() { Id = Guid.NewGuid(), Code = "major:read", Name = "查看专业", Module = "major" },
            new() { Id = Guid.NewGuid(), Code = "major:update", Name = "编辑专业", Module = "major" },
            new() { Id = Guid.NewGuid(), Code = "major:delete", Name = "删除专业", Module = "major" },
            // Class
            new() { Id = Guid.NewGuid(), Code = "class:create", Name = "创建班级", Module = "class" },
            new() { Id = Guid.NewGuid(), Code = "class:read", Name = "查看班级", Module = "class" },
            new() { Id = Guid.NewGuid(), Code = "class:update", Name = "编辑班级", Module = "class" },
            new() { Id = Guid.NewGuid(), Code = "class:delete", Name = "删除班级", Module = "class" },
            // TeachingTask
            new() { Id = Guid.NewGuid(), Code = "teachingtask:create", Name = "创建教学任务", Module = "teachingtask" },
            new() { Id = Guid.NewGuid(), Code = "teachingtask:read", Name = "查看教学任务", Module = "teachingtask" },
            new() { Id = Guid.NewGuid(), Code = "teachingtask:update", Name = "编辑教学任务", Module = "teachingtask" },
            new() { Id = Guid.NewGuid(), Code = "teachingtask:delete", Name = "删除教学任务", Module = "teachingtask" },
            // Building
            new() { Id = Guid.NewGuid(), Code = "building:create", Name = "创建楼宇", Module = "building" },
            new() { Id = Guid.NewGuid(), Code = "building:read", Name = "查看楼宇", Module = "building" },
            new() { Id = Guid.NewGuid(), Code = "building:update", Name = "编辑楼宇", Module = "building" },
            new() { Id = Guid.NewGuid(), Code = "building:delete", Name = "删除楼宇", Module = "building" },
            // Room
            new() { Id = Guid.NewGuid(), Code = "room:create", Name = "创建实验室", Module = "room" },
            new() { Id = Guid.NewGuid(), Code = "room:read", Name = "查看实验室", Module = "room" },
            new() { Id = Guid.NewGuid(), Code = "room:update", Name = "编辑实验室", Module = "room" },
            new() { Id = Guid.NewGuid(), Code = "room:delete", Name = "删除实验室", Module = "room" },
            // Schedule
            new() { Id = Guid.NewGuid(), Code = "schedule:create", Name = "创建排课", Module = "schedule" },
            new() { Id = Guid.NewGuid(), Code = "schedule:read", Name = "查看排课", Module = "schedule" },
            new() { Id = Guid.NewGuid(), Code = "schedule:update", Name = "编辑排课", Module = "schedule" },
            new() { Id = Guid.NewGuid(), Code = "schedule:delete", Name = "删除排课", Module = "schedule" },
            // Booking
            new() { Id = Guid.NewGuid(), Code = "booking:create", Name = "创建预约", Module = "booking" },
            new() { Id = Guid.NewGuid(), Code = "booking:read", Name = "查看预约", Module = "booking" },
            new() { Id = Guid.NewGuid(), Code = "booking:approve", Name = "审批预约", Module = "booking" },
            new() { Id = Guid.NewGuid(), Code = "booking:delete", Name = "删除预约", Module = "booking" },
            // Usage
            new() { Id = Guid.NewGuid(), Code = "usage:read", Name = "查看使用记录", Module = "usage" },
            new() { Id = Guid.NewGuid(), Code = "usage:create", Name = "创建使用记录", Module = "usage" },
            // Asset
            new() { Id = Guid.NewGuid(), Code = "asset:create", Name = "创建设备", Module = "asset" },
            new() { Id = Guid.NewGuid(), Code = "asset:read", Name = "查看设备", Module = "asset" },
            new() { Id = Guid.NewGuid(), Code = "asset:update", Name = "编辑设备", Module = "asset" },
            new() { Id = Guid.NewGuid(), Code = "asset:delete", Name = "删除设备", Module = "asset" },
            // Loan
            new() { Id = Guid.NewGuid(), Code = "loan:create", Name = "创建借还", Module = "loan" },
            new() { Id = Guid.NewGuid(), Code = "loan:read", Name = "查看借还", Module = "loan" },
            new() { Id = Guid.NewGuid(), Code = "loan:approve", Name = "审批借还", Module = "loan" },
            // Consumable
            new() { Id = Guid.NewGuid(), Code = "consumable:create", Name = "创建耗材", Module = "consumable" },
            new() { Id = Guid.NewGuid(), Code = "consumable:read", Name = "查看耗材", Module = "consumable" },
            new() { Id = Guid.NewGuid(), Code = "consumable:update", Name = "编辑耗材", Module = "consumable" },
            new() { Id = Guid.NewGuid(), Code = "consumable:delete", Name = "删除耗材", Module = "consumable" },
            new() { Id = Guid.NewGuid(), Code = "consumable:approve", Name = "审批耗材", Module = "consumable" },
            // ExperimentItem
            new() { Id = Guid.NewGuid(), Code = "experimentitem:create", Name = "创建实验项目", Module = "experimentitem" },
            new() { Id = Guid.NewGuid(), Code = "experimentitem:read", Name = "查看实验项目", Module = "experimentitem" },
            new() { Id = Guid.NewGuid(), Code = "experimentitem:update", Name = "编辑实验项目", Module = "experimentitem" },
            new() { Id = Guid.NewGuid(), Code = "experimentitem:delete", Name = "删除实验项目", Module = "experimentitem" },
        };
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
