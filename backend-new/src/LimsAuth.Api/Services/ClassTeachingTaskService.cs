using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IClassService
{
    Task<ApiResponse<PagedResponse<ClassDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<ClassDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ClassDto>> CreateAsync(CreateClassRequest request);
    Task<ApiResponse<ClassDto>> UpdateAsync(Guid id, UpdateClassRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request);
    Task<ApiResponse> UpdateStudentsAsync(Guid id, ClassStudentsRequest request);
    Task<ApiResponse<List<UserBriefDto>>> GetStudentsAsync(Guid id);
}

public class ClassService : IClassService
{
    private readonly AppDbContext _db;
    public ClassService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<ClassDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.EduClasses
            .Include(c => c.Institution).Include(c => c.Department).Include(c => c.Major)
            .Where(c => c.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(c => c.Name.Contains(query.Keyword) || c.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(c => c.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(c => new ClassDto
            {
                Id = c.Id, Code = c.Code, Name = c.Name,
                InstitutionId = c.InstitutionId, InstitutionName = c.Institution != null ? c.Institution.Name : null,
                DepartmentId = c.DepartmentId, DepartmentName = c.Department != null ? c.Department.Name : null,
                MajorId = c.MajorId, MajorName = c.Major != null ? c.Major.Name : null,
                GradeName = c.GradeName, MonitorId = c.MonitorId, HeadTeacherId = c.HeadTeacherId,
                StudentCount = c.StudentCount, Status = c.Status, Description = c.Description, CreatedAt = c.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<ClassDto>>.Success(new PagedResponse<ClassDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<ClassDto>> GetByIdAsync(Guid id)
    {
        var c = await _db.EduClasses.Include(x => x.Institution).Include(x => x.Department).Include(x => x.Major)
            .Include(x => x.ClassStudents).ThenInclude(cs => cs.Student)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return ApiResponse<ClassDto>.Error(404, "班级不存在");
        return ApiResponse<ClassDto>.Success(MapToDto(c));
    }

    public async Task<ApiResponse<ClassDto>> CreateAsync(CreateClassRequest request)
    {
        var c = new EduClass
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name,
            InstitutionId = request.InstitutionId, DepartmentId = request.DepartmentId, MajorId = request.MajorId,
            GradeName = request.GradeName, MonitorId = request.MonitorId, HeadTeacherId = request.HeadTeacherId,
            StudentCount = request.StudentCount, Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.EduClasses.Add(c);
        await _db.SaveChangesAsync();
        return ApiResponse<ClassDto>.Success(new ClassDto { Id = c.Id, Code = c.Code, Name = c.Name }, "班级创建成功");
    }

    public async Task<ApiResponse<ClassDto>> UpdateAsync(Guid id, UpdateClassRequest request)
    {
        var c = await _db.EduClasses.FindAsync(id);
        if (c == null) return ApiResponse<ClassDto>.Error(404, "班级不存在");
        if (!string.IsNullOrEmpty(request.Name)) c.Name = request.Name;
        if (request.StudentCount.HasValue) c.StudentCount = request.StudentCount.Value;
        if (request.Status.HasValue) c.Status = request.Status.Value;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<ClassDto>.Success(new ClassDto { Id = c.Id, Code = c.Code, Name = c.Name }, "班级更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var c = await _db.EduClasses.FindAsync(id);
        if (c == null) return ApiResponse.Error(404, "班级不存在");
        c.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("班级删除成功");
    }

    public async Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request)
    {
        var c = await _db.EduClasses.FindAsync(id);
        if (c == null) return ApiResponse.Error(404, "班级不存在");
        c.Status = request.IsActive ? 1 : 0;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("状态更新成功");
    }

    public async Task<ApiResponse> UpdateStudentsAsync(Guid id, ClassStudentsRequest request)
    {
        var classEntity = await _db.EduClasses.Include(c => c.ClassStudents).FirstOrDefaultAsync(c => c.Id == id);
        if (classEntity == null) return ApiResponse.Error(404, "班级不存在");

        _db.EduClassStudents.RemoveRange(classEntity.ClassStudents);
        foreach (var studentId in request.StudentIds)
        {
            _db.EduClassStudents.Add(new EduClassStudent { ClassId = id, StudentId = studentId });
        }
        classEntity.StudentCount = request.StudentIds.Count;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("学生分配成功");
    }

    public async Task<ApiResponse<List<UserBriefDto>>> GetStudentsAsync(Guid id)
    {
        var students = await _db.EduClassStudents
            .Where(cs => cs.ClassId == id)
            .Include(cs => cs.Student)
            .Select(cs => new UserBriefDto
            {
                Id = cs.Student.Id,
                Username = cs.Student.Username,
                RealName = cs.Student.RealName,
                EmployeeNo = cs.Student.EmployeeNo,
                Mobile = cs.Student.Mobile
            }).ToListAsync();
        return ApiResponse<List<UserBriefDto>>.Success(students);
    }

    private ClassDto MapToDto(EduClass c)
    {
        return new ClassDto
        {
            Id = c.Id, Code = c.Code, Name = c.Name,
            InstitutionId = c.InstitutionId, InstitutionName = c.Institution?.Name,
            DepartmentId = c.DepartmentId, DepartmentName = c.Department?.Name,
            MajorId = c.MajorId, MajorName = c.Major?.Name,
            GradeName = c.GradeName, MonitorId = c.MonitorId, HeadTeacherId = c.HeadTeacherId,
            StudentCount = c.StudentCount, Status = c.Status, Description = c.Description, CreatedAt = c.CreatedAt,
            Students = c.ClassStudents.Select(cs => new UserBriefDto
            {
                Id = cs.Student.Id, Username = cs.Student.Username,
                RealName = cs.Student.RealName, EmployeeNo = cs.Student.EmployeeNo, Mobile = cs.Student.Mobile
            }).ToList()
        };
    }
}

public interface ITeachingTaskService
{
    Task<ApiResponse<PagedResponse<TeachingTaskDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<TeachingTaskDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<TeachingTaskDto>> CreateAsync(CreateTeachingTaskRequest request);
    Task<ApiResponse<TeachingTaskDto>> UpdateAsync(Guid id, UpdateTeachingTaskRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> UpdateTeachersAsync(Guid id, TeachingTaskTeachersRequest request);
}

public class TeachingTaskService : ITeachingTaskService
{
    private readonly AppDbContext _db;
    public TeachingTaskService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<TeachingTaskDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.EduTeachingTasks
            .Include(t => t.Semester).Include(t => t.Course).Include(t => t.Major).Include(t => t.Class)
            .Include(t => t.Teachers).ThenInclude(tt => tt.Teacher)
            .Where(t => t.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(t => t.Code.Contains(query.Keyword) || (t.Course != null && t.Course.Name.Contains(query.Keyword)));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(t => t.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(t => new TeachingTaskDto
            {
                Id = t.Id, Code = t.Code, SemesterId = t.SemesterId, SemesterName = t.Semester != null ? t.Semester.Name : null,
                CourseId = t.CourseId, CourseName = t.Course != null ? t.Course.Name : null,
                MajorId = t.MajorId, MajorName = t.Major != null ? t.Major.Name : null,
                ClassId = t.ClassId, ClassName = t.Class != null ? t.Class.Name : null,
                WeeklyHours = t.WeeklyHours, StartWeek = t.StartWeek, EndWeek = t.EndWeek,
                ExamMode = t.ExamMode, Status = t.Status, Description = t.Description, CreatedAt = t.CreatedAt,
                Teachers = t.Teachers.Select(tt => new TeacherBriefDto { Id = tt.TeacherId, RealName = tt.Teacher.RealName, EmployeeNo = tt.Teacher.EmployeeNo, Role = tt.Role }).ToList()
            }).ToListAsync();
        return ApiResponse<PagedResponse<TeachingTaskDto>>.Success(new PagedResponse<TeachingTaskDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<TeachingTaskDto>> GetByIdAsync(Guid id)
    {
        var t = await _db.EduTeachingTasks
            .Include(x => x.Semester).Include(x => x.Course).Include(x => x.Major).Include(x => x.Class)
            .Include(x => x.Teachers).ThenInclude(tt => tt.Teacher)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return ApiResponse<TeachingTaskDto>.Error(404, "教学任务不存在");
        return ApiResponse<TeachingTaskDto>.Success(MapToDto(t));
    }

    public async Task<ApiResponse<TeachingTaskDto>> CreateAsync(CreateTeachingTaskRequest request)
    {
        var t = new EduTeachingTask
        {
            Id = Guid.NewGuid(), Code = request.Code, SemesterId = request.SemesterId,
            CourseId = request.CourseId, MajorId = request.MajorId, ClassId = request.ClassId,
            WeeklyHours = request.WeeklyHours, StartWeek = request.StartWeek, EndWeek = request.EndWeek,
            ExamMode = request.ExamMode, SortOrder = request.SortOrder,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.EduTeachingTasks.Add(t);
        foreach (var teacherId in request.TeacherIds)
        {
            _db.EduTeachingTaskTeachers.Add(new EduTeachingTaskTeacher { Id = Guid.NewGuid(), TaskId = t.Id, TeacherId = teacherId });
        }
        await _db.SaveChangesAsync();
        return ApiResponse<TeachingTaskDto>.Success(new TeachingTaskDto { Id = t.Id, Code = t.Code }, "教学任务创建成功");
    }

    public async Task<ApiResponse<TeachingTaskDto>> UpdateAsync(Guid id, UpdateTeachingTaskRequest request)
    {
        var t = await _db.EduTeachingTasks.FindAsync(id);
        if (t == null) return ApiResponse<TeachingTaskDto>.Error(404, "教学任务不存在");
        if (request.CourseId.HasValue) t.CourseId = request.CourseId;
        if (request.ClassId.HasValue) t.ClassId = request.ClassId;
        if (request.Status.HasValue) t.Status = request.Status.Value;
        t.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<TeachingTaskDto>.Success(new TeachingTaskDto { Id = t.Id, Code = t.Code }, "教学任务更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var t = await _db.EduTeachingTasks.FindAsync(id);
        if (t == null) return ApiResponse.Error(404, "教学任务不存在");
        t.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("教学任务删除成功");
    }

    public async Task<ApiResponse> UpdateTeachersAsync(Guid id, TeachingTaskTeachersRequest request)
    {
        var task = await _db.EduTeachingTasks.Include(t => t.Teachers).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return ApiResponse.Error(404, "教学任务不存在");
        _db.EduTeachingTaskTeachers.RemoveRange(task.Teachers);
        foreach (var ta in request.Teachers)
        {
            _db.EduTeachingTaskTeachers.Add(new EduTeachingTaskTeacher { Id = Guid.NewGuid(), TaskId = id, TeacherId = ta.TeacherId, Role = ta.Role });
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success("教师分配成功");
    }

    private TeachingTaskDto MapToDto(EduTeachingTask t)
    {
        return new TeachingTaskDto
        {
            Id = t.Id, Code = t.Code, SemesterId = t.SemesterId, SemesterName = t.Semester?.Name,
            CourseId = t.CourseId, CourseName = t.Course?.Name, MajorId = t.MajorId, MajorName = t.Major?.Name,
            ClassId = t.ClassId, ClassName = t.Class?.Name, WeeklyHours = t.WeeklyHours,
            StartWeek = t.StartWeek, EndWeek = t.EndWeek, ExamMode = t.ExamMode,
            Status = t.Status, Description = t.Description, CreatedAt = t.CreatedAt,
            Teachers = t.Teachers.Select(tt => new TeacherBriefDto { Id = tt.TeacherId, RealName = tt.Teacher.RealName, EmployeeNo = tt.Teacher.EmployeeNo, Role = tt.Role }).ToList()
        };
    }
}
