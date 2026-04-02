using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IClassService
{
    Task<List<ClassDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? majorId = null, string? grade = null);
    Task<ClassDto?> GetByIdAsync(Guid id);
    Task<ClassDto> CreateAsync(CreateClassRequest request);
    Task<ClassDto?> UpdateAsync(Guid id, UpdateClassRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
    Task<List<User>> GetStudentsAsync(Guid classId);
    Task<bool> AddStudentsAsync(Guid classId, List<Guid> studentIds);
    Task<bool> RemoveStudentAsync(Guid classId, Guid studentId);
}

public class ClassService : IClassService
{
    private readonly AppDbContext _dbContext;

    public ClassService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ClassDto>> GetListAsync(string? keyword = null, string? departmentId = null, string? majorId = null, string? grade = null)
    {
        var query = _dbContext.Classes.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(c => c.Name.Contains(keyword) || c.Code.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(departmentId) && Guid.TryParse(departmentId, out var deptId))
        {
            query = query.Where(c => c.DepartmentId == deptId);
        }

        if (!string.IsNullOrEmpty(majorId) && Guid.TryParse(majorId, out var majId))
        {
            query = query.Where(c => c.MajorId == majId);
        }

        if (!string.IsNullOrEmpty(grade))
        {
            query = query.Where(c => c.Grade == grade);
        }

        var classes = await query.OrderByDescending(c => c.Grade).ThenBy(c => c.Code).ToListAsync();
        
        // 获取关联数据
        var majorIds = classes.Select(c => c.MajorId).Distinct().ToList();
        var majors = await _dbContext.Majors
            .Where(m => majorIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, m => m.Name);
        
        var teacherIds = classes.Where(c => c.HeadTeacherId.HasValue).Select(c => c.HeadTeacherId!.Value).Distinct().ToList();
        var teachers = await _dbContext.Users
            .Where(u => teacherIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName ?? u.Username);
        
        return classes.Select(c => new ClassDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Grade = c.Grade,
            MajorId = c.MajorId,
            MajorName = majors.GetValueOrDefault(c.MajorId),
            DepartmentId = c.DepartmentId,
            HeadTeacherId = c.HeadTeacherId,
            HeadTeacherName = c.HeadTeacherId.HasValue ? teachers.GetValueOrDefault(c.HeadTeacherId.Value) : null,
            AdminStudentId = c.AdminStudentId,
            StudentCount = c.StudentCount,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var classEntity = await _dbContext.Classes.FindAsync(id);
        if (classEntity == null) return null;
        
        var major = await _dbContext.Majors.FindAsync(classEntity.MajorId);
        var teacher = classEntity.HeadTeacherId.HasValue 
            ? await _dbContext.Users.FindAsync(classEntity.HeadTeacherId.Value) 
            : null;
        
        return new ClassDto
        {
            Id = classEntity.Id,
            Code = classEntity.Code,
            Name = classEntity.Name,
            Grade = classEntity.Grade,
            MajorId = classEntity.MajorId,
            MajorName = major?.Name,
            DepartmentId = classEntity.DepartmentId,
            HeadTeacherId = classEntity.HeadTeacherId,
            HeadTeacherName = teacher?.FullName ?? teacher?.Username,
            AdminStudentId = classEntity.AdminStudentId,
            StudentCount = classEntity.StudentCount,
            Description = classEntity.Description,
            IsActive = classEntity.IsActive,
            CreatedAt = classEntity.CreatedAt
        };
    }

    public async Task<ClassDto> CreateAsync(CreateClassRequest request)
    {
        var classEntity = new Class
        {
            Code = request.Code,
            Name = request.Name,
            Grade = request.Grade,
            MajorId = request.MajorId,
            DepartmentId = request.DepartmentId,
            HeadTeacherId = request.HeadTeacherId,
            AdminStudentId = request.AdminStudentId,
            Description = request.Description,
            StudentCount = 0,
            IsActive = true
        };

        _dbContext.Classes.Add(classEntity);
        await _dbContext.SaveChangesAsync();
        
        return await GetByIdAsync(classEntity.Id) ?? throw new Exception("创建班级失败");
    }

    public async Task<ClassDto?> UpdateAsync(Guid id, UpdateClassRequest request)
    {
        var classEntity = await _dbContext.Classes.FindAsync(id);
        if (classEntity == null) return null;

        if (request.Code != null) classEntity.Code = request.Code;
        if (request.Name != null) classEntity.Name = request.Name;
        if (request.Grade != null) classEntity.Grade = request.Grade;
        if (request.MajorId.HasValue) classEntity.MajorId = request.MajorId.Value;
        if (request.DepartmentId.HasValue) classEntity.DepartmentId = request.DepartmentId.Value;
        if (request.HeadTeacherId.HasValue) classEntity.HeadTeacherId = request.HeadTeacherId.Value;
        if (request.AdminStudentId.HasValue) classEntity.AdminStudentId = request.AdminStudentId.Value;
        if (request.Description != null) classEntity.Description = request.Description;
        if (request.IsActive.HasValue) classEntity.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var classEntity = await _dbContext.Classes.FindAsync(id);
        if (classEntity == null) return false;

        _dbContext.Classes.Remove(classEntity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var classEntity = await _dbContext.Classes.FindAsync(id);
        if (classEntity == null) return false;

        classEntity.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<User>> GetStudentsAsync(Guid classId)
    {
        var studentIds = await _dbContext.ClassStudents
            .Where(cs => cs.ClassId == classId)
            .Select(cs => cs.StudentId)
            .ToListAsync();

        return await _dbContext.Users
            .Where(u => studentIds.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<bool> AddStudentsAsync(Guid classId, List<Guid> studentIds)
    {
        var classEntity = await _dbContext.Classes.FindAsync(classId);
        if (classEntity == null) return false;

        var existingStudentIds = await _dbContext.ClassStudents
            .Where(cs => cs.ClassId == classId)
            .Select(cs => cs.StudentId)
            .ToListAsync();

        var newStudentIds = studentIds.Where(id => !existingStudentIds.Contains(id)).ToList();

        foreach (var studentId in newStudentIds)
        {
            _dbContext.ClassStudents.Add(new ClassStudent
            {
                ClassId = classId,
                StudentId = studentId,
                JoinedAt = DateTime.UtcNow
            });
        }

        classEntity.StudentCount += newStudentIds.Count;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveStudentAsync(Guid classId, Guid studentId)
    {
        var classStudent = await _dbContext.ClassStudents
            .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);

        if (classStudent == null) return false;

        _dbContext.ClassStudents.Remove(classStudent);

        var classEntity = await _dbContext.Classes.FindAsync(classId);
        if (classEntity != null && classEntity.StudentCount > 0)
        {
            classEntity.StudentCount--;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class CreateClassRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public Guid MajorId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public Guid? AdminStudentId { get; set; }
    public string? Description { get; set; }
}

public class UpdateClassRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Grade { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public Guid? AdminStudentId { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class ClassDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public Guid MajorId { get; set; }
    public string? MajorName { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public string? HeadTeacherName { get; set; }
    public Guid? AdminStudentId { get; set; }
    public int StudentCount { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
