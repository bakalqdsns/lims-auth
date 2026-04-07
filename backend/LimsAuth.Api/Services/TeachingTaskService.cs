using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface ITeachingTaskService
{
    Task<List<TeachingTaskDto>> GetListAsync(string? semesterId = null, string? courseId = null, string? classId = null, string? teacherId = null);
    Task<TeachingTaskDto?> GetByIdAsync(Guid id);
    Task<TeachingTask> CreateAsync(CreateTeachingTaskRequest request);
    Task<TeachingTask?> UpdateAsync(Guid id, UpdateTeachingTaskRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
    Task<bool> AddTeacherAsync(Guid taskId, Guid teacherId, bool isMainTeacher = false);
    Task<bool> RemoveTeacherAsync(Guid taskId, Guid teacherId);
}

public class TeachingTaskService : ITeachingTaskService
{
    private readonly AppDbContext _dbContext;

    public TeachingTaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TeachingTaskDto>> GetListAsync(string? semesterId = null, string? courseId = null, string? classId = null, string? teacherId = null)
    {
        var query = _dbContext.TeachingTasks
            .Include(t => t.Semester)
            .Include(t => t.Course)
            .Include(t => t.Class)
            .Include(t => t.Teachers)
                .ThenInclude(tt => tt.Teacher)
            .AsQueryable();

        if (!string.IsNullOrEmpty(semesterId) && Guid.TryParse(semesterId, out var semId))
        {
            query = query.Where(t => t.SemesterId == semId);
        }

        if (!string.IsNullOrEmpty(courseId) && Guid.TryParse(courseId, out var crsId))
        {
            query = query.Where(t => t.CourseId == crsId);
        }

        if (!string.IsNullOrEmpty(classId) && Guid.TryParse(classId, out var clsId))
        {
            query = query.Where(t => t.ClassId == clsId);
        }

        if (!string.IsNullOrEmpty(teacherId) && Guid.TryParse(teacherId, out var tchId))
        {
            query = query.Where(t => t.Teachers.Any(tt => tt.TeacherId == tchId));
        }

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

        return tasks.Select(t => new TeachingTaskDto
        {
            Id = t.Id,
            SemesterId = t.SemesterId,
            SemesterName = t.Semester?.Name ?? "",
            CourseId = t.CourseId,
            CourseName = t.Course?.Name ?? "",
            CourseCode = t.Course?.Code ?? "",
            ClassId = t.ClassId,
            ClassName = t.Class?.Name ?? "",
            TaskType = t.TaskType,
            Description = t.Description,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt,
            Teachers = t.Teachers.Select(tt => new TaskTeacherDto
            {
                Id = tt.TeacherId,
                Name = tt.Teacher?.FullName ?? tt.Teacher?.Username ?? "",
                IsMainTeacher = tt.IsMainTeacher
            }).ToList(),
            StudentCount = t.Class?.StudentCount ?? 0
        }).ToList();
    }

    public async Task<TeachingTaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _dbContext.TeachingTasks
            .Include(t => t.Semester)
            .Include(t => t.Course)
            .Include(t => t.Class)
            .Include(t => t.Teachers)
                .ThenInclude(tt => tt.Teacher)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return null;

        return new TeachingTaskDto
        {
            Id = task.Id,
            SemesterId = task.SemesterId,
            SemesterName = task.Semester?.Name ?? "",
            CourseId = task.CourseId,
            CourseName = task.Course?.Name ?? "",
            CourseCode = task.Course?.Code ?? "",
            ClassId = task.ClassId,
            ClassName = task.Class?.Name ?? "",
            TaskType = task.TaskType,
            Description = task.Description,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt,
            Teachers = task.Teachers.Select(tt => new TaskTeacherDto
            {
                Id = tt.TeacherId,
                Name = tt.Teacher?.FullName ?? tt.Teacher?.Username ?? "",
                IsMainTeacher = tt.IsMainTeacher
            }).ToList(),
            StudentCount = task.Class?.StudentCount ?? 0
        };
    }

    public async Task<TeachingTask> CreateAsync(CreateTeachingTaskRequest request)
    {
        var task = new TeachingTask
        {
            SemesterId = request.SemesterId,
            CourseId = request.CourseId,
            ClassId = request.ClassId,
            TaskType = request.TaskType ?? "主讲",
            Description = request.Description,
            IsActive = true
        };

        _dbContext.TeachingTasks.Add(task);
        await _dbContext.SaveChangesAsync();

        // 添加教师关联
        if (request.TeacherIds != null && request.TeacherIds.Any())
        {
            foreach (var teacherId in request.TeacherIds)
            {
                _dbContext.TeachingTaskTeachers.Add(new TeachingTaskTeacher
                {
                    TeachingTaskId = task.Id,
                    TeacherId = teacherId,
                    IsMainTeacher = teacherId == request.TeacherIds.First(),
                    AssignedAt = DateTime.UtcNow
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        return task;
    }

    public async Task<TeachingTask?> UpdateAsync(Guid id, UpdateTeachingTaskRequest request)
    {
        var task = await _dbContext.TeachingTasks.FindAsync(id);
        if (task == null) return null;

        if (request.SemesterId.HasValue) task.SemesterId = request.SemesterId.Value;
        if (request.CourseId.HasValue) task.CourseId = request.CourseId.Value;
        if (request.ClassId.HasValue) task.ClassId = request.ClassId.Value;
        if (request.TaskType != null) task.TaskType = request.TaskType;
        if (request.Description != null) task.Description = request.Description;
        if (request.IsActive.HasValue) task.IsActive = request.IsActive.Value;

        // 更新教师关联
        if (request.TeacherIds != null)
        {
            var existingTeachers = await _dbContext.TeachingTaskTeachers
                .Where(tt => tt.TeachingTaskId == id)
                .ToListAsync();

            _dbContext.TeachingTaskTeachers.RemoveRange(existingTeachers);

            foreach (var teacherId in request.TeacherIds)
            {
                _dbContext.TeachingTaskTeachers.Add(new TeachingTaskTeacher
                {
                    TeachingTaskId = id,
                    TeacherId = teacherId,
                    IsMainTeacher = teacherId == request.TeacherIds.First(),
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        await _dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _dbContext.TeachingTasks.FindAsync(id);
        if (task == null) return false;

        // 删除教师关联
        var teachers = await _dbContext.TeachingTaskTeachers
            .Where(tt => tt.TeachingTaskId == id)
            .ToListAsync();
        _dbContext.TeachingTaskTeachers.RemoveRange(teachers);

        _dbContext.TeachingTasks.Remove(task);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var task = await _dbContext.TeachingTasks.FindAsync(id);
        if (task == null) return false;

        task.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTeacherAsync(Guid taskId, Guid teacherId, bool isMainTeacher = false)
    {
        var task = await _dbContext.TeachingTasks.FindAsync(taskId);
        if (task == null) return false;

        var existing = await _dbContext.TeachingTaskTeachers
            .FirstOrDefaultAsync(tt => tt.TeachingTaskId == taskId && tt.TeacherId == teacherId);

        if (existing != null) return false;

        _dbContext.TeachingTaskTeachers.Add(new TeachingTaskTeacher
        {
            TeachingTaskId = taskId,
            TeacherId = teacherId,
            IsMainTeacher = isMainTeacher,
            AssignedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveTeacherAsync(Guid taskId, Guid teacherId)
    {
        var teacher = await _dbContext.TeachingTaskTeachers
            .FirstOrDefaultAsync(tt => tt.TeachingTaskId == taskId && tt.TeacherId == teacherId);

        if (teacher == null) return false;

        _dbContext.TeachingTaskTeachers.Remove(teacher);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class TeachingTaskDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskTeacherDto> Teachers { get; set; } = new();
    public int StudentCount { get; set; }
}

public class TaskTeacherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsMainTeacher { get; set; }
}

public class CreateTeachingTaskRequest
{
    public Guid SemesterId { get; set; }
    public Guid CourseId { get; set; }
    public Guid ClassId { get; set; }
    public string? TaskType { get; set; }
    public string? Description { get; set; }
    public List<Guid>? TeacherIds { get; set; }
}

public class UpdateTeachingTaskRequest
{
    public Guid? SemesterId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? ClassId { get; set; }
    public string? TaskType { get; set; }
    public string? Description { get; set; }
    public List<Guid>? TeacherIds { get; set; }
    public bool? IsActive { get; set; }
}
