using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data.Configuration;

namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 基础教学实体
// ============================================================

/// <summary>
/// 学期实体 (Edu_Semester)
/// </summary>
[EntityTypeConfiguration(typeof(EduSemesterConfiguration))]
public class EduSemester
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? SchoolYear { get; set; }
    public int SemesterNo { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalWeeks { get; set; }
    public int IsCurrent { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ICollection<EduTeachingTask> TeachingTasks { get; set; } = new List<EduTeachingTask>();
}

/// <summary>
/// 课程实体 (Edu_Course)
/// </summary>
[EntityTypeConfiguration(typeof(EduCourseConfiguration))]
public class EduCourse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Nature { get; set; }
    public decimal? Credits { get; set; }
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int OnlineHours { get; set; }
    public string? OpenSemesters { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ICollection<EduTeachingTask> TeachingTasks { get; set; } = new List<EduTeachingTask>();
    public ICollection<LabExperimentItem> ExperimentItems { get; set; } = new List<LabExperimentItem>();
}

/// <summary>
/// 专业实体 (Edu_Major)
/// </summary>
[EntityTypeConfiguration(typeof(EduMajorConfiguration))]
public class EduMajor
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DegreeLevel { get; set; }
    public int Duration { get; set; }
    public string? DegreeName { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? Institution { get; set; }
    public SysDepartment? Department { get; set; }
    public ICollection<EduClass> Classes { get; set; } = new List<EduClass>();
    public ICollection<EduTeachingTask> TeachingTasks { get; set; } = new List<EduTeachingTask>();
}

/// <summary>
/// 班级实体 (Edu_Class)
/// </summary>
[EntityTypeConfiguration(typeof(EduClassConfiguration))]
public class EduClass
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? MajorId { get; set; }
    public string? GradeName { get; set; }
    public Guid? MonitorId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public int StudentCount { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public SysInstitution? Institution { get; set; }
    public SysDepartment? Department { get; set; }
    public EduMajor? Major { get; set; }
    public ICollection<EduClassStudent> ClassStudents { get; set; } = new List<EduClassStudent>();
    public ICollection<EduTeachingTask> TeachingTasks { get; set; } = new List<EduTeachingTask>();
}

/// <summary>
/// 班级学生关联 (Edu_ClassStudent)
/// </summary>
[EntityTypeConfiguration(typeof(EduClassStudentConfiguration))]
public class EduClassStudent
{
    public Guid ClassId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public EduClass Class { get; set; } = null!;
    public SysUser Student { get; set; } = null!;
}

/// <summary>
/// 教学任务实体 (Edu_TeachingTask)
/// </summary>
[EntityTypeConfiguration(typeof(EduTeachingTaskConfiguration))]
public class EduTeachingTask
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid? SemesterId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? ClassId { get; set; }
    public int WeeklyHours { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
    public string? ExamMode { get; set; }
    public int SortOrder { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Description { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public EduSemester? Semester { get; set; }
    public EduCourse? Course { get; set; }
    public EduMajor? Major { get; set; }
    public EduClass? Class { get; set; }
    public ICollection<EduTeachingTaskTeacher> Teachers { get; set; } = new List<EduTeachingTaskTeacher>();
}

/// <summary>
/// 教学任务教师关联 (Edu_TeachingTaskTeacher)
/// </summary>
[EntityTypeConfiguration(typeof(EduTeachingTaskTeacherConfiguration))]
public class EduTeachingTaskTeacher
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid TeacherId { get; set; }
    public string Role { get; set; } = "主讲";
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public EduTeachingTask Task { get; set; } = null!;
    public SysUser Teacher { get; set; } = null!;
}
