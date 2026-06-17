using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 学期管理 DTO
// ============================================================

public class CreateSemesterRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(20)]
    public string? SchoolYear { get; set; }
    public int SemesterNo { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalWeeks { get; set; }
    public int IsCurrent { get; set; } = 0;
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateSemesterRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(20)]
    public string? SchoolYear { get; set; }
    public int? SemesterNo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? TotalWeeks { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class SemesterDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? SchoolYear { get; set; }
    public int SemesterNo { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalWeeks { get; set; }
    public int IsCurrent { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

// ============================================================
// 课程管理 DTO
// ============================================================

public class CreateCourseRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? NameEn { get; set; }
    [MaxLength(20)]
    public string? Nature { get; set; }
    public decimal? Credits { get; set; }
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int OnlineHours { get; set; }
    [MaxLength(50)]
    public string? OpenSemesters { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class UpdateCourseRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(200)]
    public string? NameEn { get; set; }
    [MaxLength(20)]
    public string? Nature { get; set; }
    public decimal? Credits { get; set; }
    public int? TotalHours { get; set; }
    public int? LectureHours { get; set; }
    public int? PracticeHours { get; set; }
    public int? LabHours { get; set; }
    public int? OnlineHours { get; set; }
    [MaxLength(50)]
    public string? OpenSemesters { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class CourseDto
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
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ============================================================
// 专业管理 DTO
// ============================================================

public class CreateMajorRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    [MaxLength(20)]
    public string? DegreeLevel { get; set; }
    public int Duration { get; set; }
    [MaxLength(100)]
    public string? DegreeName { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateMajorRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(200)]
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    [MaxLength(20)]
    public string? DegreeLevel { get; set; }
    public int? Duration { get; set; }
    [MaxLength(100)]
    public string? DegreeName { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class MajorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid? InstitutionId { get; set; }
    public string? InstitutionName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? DegreeLevel { get; set; }
    public int Duration { get; set; }
    public string? DegreeName { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ============================================================
// 班级管理 DTO
// ============================================================

public class CreateClassRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? MajorId { get; set; }
    [MaxLength(20)]
    public string? GradeName { get; set; }
    public Guid? MonitorId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public int StudentCount { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateClassRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public Guid? InstitutionId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? MajorId { get; set; }
    [MaxLength(20)]
    public string? GradeName { get; set; }
    public Guid? MonitorId { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public int? StudentCount { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class ClassDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? InstitutionId { get; set; }
    public string? InstitutionName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? MajorId { get; set; }
    public string? MajorName { get; set; }
    public string? GradeName { get; set; }
    public Guid? MonitorId { get; set; }
    public string? MonitorName { get; set; }
    public Guid? HeadTeacherId { get; set; }
    public string? HeadTeacherName { get; set; }
    public int StudentCount { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserBriefDto> Students { get; set; } = new();
}

public class UserBriefDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? EmployeeNo { get; set; }
    public string? Mobile { get; set; }
}

public class ClassStudentsRequest
{
    public List<Guid> StudentIds { get; set; } = new();
}

// ============================================================
// 教学任务 DTO
// ============================================================

public class CreateTeachingTaskRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    public Guid? SemesterId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? ClassId { get; set; }
    public int WeeklyHours { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
    [MaxLength(50)]
    public string? ExamMode { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Description { get; set; }
    public List<Guid> TeacherIds { get; set; } = new();
}

public class UpdateTeachingTaskRequest
{
    [MaxLength(50)]
    public string? Code { get; set; }
    public Guid? SemesterId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? ClassId { get; set; }
    public int? WeeklyHours { get; set; }
    public int? StartWeek { get; set; }
    public int? EndWeek { get; set; }
    [MaxLength(50)]
    public string? ExamMode { get; set; }
    public int? SortOrder { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class TeachingTaskDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid? SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }
    public Guid? MajorId { get; set; }
    public string? MajorName { get; set; }
    public Guid? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int WeeklyHours { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
    public string? ExamMode { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TeacherBriefDto> Teachers { get; set; } = new();
}

public class TeacherBriefDto
{
    public Guid Id { get; set; }
    public string RealName { get; set; } = string.Empty;
    public string? EmployeeNo { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class TeachingTaskTeachersRequest
{
    public List<TeacherAssignment> Teachers { get; set; } = new();
}

public class TeacherAssignment
{
    public Guid TeacherId { get; set; }
    [MaxLength(20)]
    public string Role { get; set; } = "主讲";
}
