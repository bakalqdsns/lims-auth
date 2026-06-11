using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 排课记录 DTO
// ============================================================

public class CreateScheduleRequest
{
    [Required]
    public Guid SemesterId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public int WeekNo { get; set; }
    public int DayOfWeek { get; set; }
    [MaxLength(50)]
    public string SectionNo { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? CourseName { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid? ExperimentItemId { get; set; }
    [MaxLength(50)]
    public string ScheduleType { get; set; } = "CentralScheduling";
    public int Status { get; set; } = 1;
    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class UpdateScheduleRequest
{
    public Guid? RoomId { get; set; }
    public int? WeekNo { get; set; }
    public int? DayOfWeek { get; set; }
    [MaxLength(50)]
    public string? SectionNo { get; set; }
    public Guid? TeacherId { get; set; }
    public int? Status { get; set; }
    [MaxLength(500)]
    public string? Remark { get; set; }
}

public class ScheduleDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? RoomId { get; set; }
    public string? RoomName { get; set; }
    public string? BuildingName { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public string? CourseName { get; set; }
    public Guid? ClassId { get; set; }
    public string? ClassName { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public Guid? ExperimentItemId { get; set; }
    public string? ExperimentItemName { get; set; }
    public int WeekNo { get; set; }
    public int DayOfWeek { get; set; }
    public string SectionNo { get; set; } = string.Empty;
    public string ScheduleType { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

public class ScheduleQueryRequest : PagedQueryRequest
{
    public Guid? SemesterId { get; set; }
    public int? WeekNo { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid? CourseId { get; set; }
    public string? ScheduleType { get; set; }
    public int? Status { get; set; }
}

// ============================================================
// 实验项目 DTO
// ============================================================

public class CreateExperimentItemRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    [MaxLength(50)]
    public string? ExperimentType { get; set; }
    public int StandardHours { get; set; }
    public int Status { get; set; } = 1;
}

public class UpdateExperimentItemRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    public Guid? CourseId { get; set; }
    [MaxLength(50)]
    public string? ExperimentType { get; set; }
    public int? StandardHours { get; set; }
    public int? Status { get; set; }
}

public class ExperimentItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? ExperimentType { get; set; }
    public int StandardHours { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ============================================================
// 预约申请 DTO
// ============================================================

public class CreateBookingApplyRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    [MaxLength(20)]
    public string ApplicantType { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    [MaxLength(1000)]
    public string? Purpose { get; set; }
    public DateTime TargetDate { get; set; }
    [MaxLength(50)]
    public string TargetSection { get; set; } = string.Empty;
    public int WeekNo { get; set; }
    public int EstimatedPeople { get; set; }
}

public class BookingApplyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantType { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public string? RoomName { get; set; }
    public string? Purpose { get; set; }
    public DateTime TargetDate { get; set; }
    public string TargetSection { get; set; } = string.Empty;
    public int WeekNo { get; set; }
    public int EstimatedPeople { get; set; }
    public string AuditStatus { get; set; } = string.Empty;
    public string? AuditOpinion { get; set; }
    public Guid? AuditorId { get; set; }
    public string? AuditorName { get; set; }
    public DateTime? AuditTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingApplyQueryRequest : PagedQueryRequest
{
    public Guid? RoomId { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? AuditStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class AuditBookingRequest
{
    [Required]
    public bool Approved { get; set; }
    [MaxLength(500)]
    public string? Opinion { get; set; }
}

// ============================================================
// 使用登记 DTO
// ============================================================

public class CreateUsageRegisterRequest
{
    public Guid? ScheduleId { get; set; }
    public Guid? BookingApplyId { get; set; }
    public Guid? SemesterId { get; set; }
    public Guid? RoomId { get; set; }
    [MaxLength(200)]
    public string? ItemName { get; set; }
    [MaxLength(50)]
    public string? ExperimentType { get; set; }
    public int PlannedHours { get; set; }
    public double ActualHours { get; set; }
    [MaxLength(100)]
    public string? ClassName { get; set; }
    public int? ExpectedCount { get; set; }
    public int? ActualCount { get; set; }
    [MaxLength(500)]
    public string? AttendanceRecord { get; set; }
    [MaxLength(200)]
    public string? TeachingRecord { get; set; }
    [MaxLength(200)]
    public string? DeviceRecord { get; set; }
    [MaxLength(50)]
    public string RegisterStatus { get; set; } = "Pending";
}

public class UsageRegisterDto
{
    public Guid Id { get; set; }
    public Guid? ScheduleId { get; set; }
    public Guid? BookingApplyId { get; set; }
    public Guid? SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? RoomId { get; set; }
    public string? RoomName { get; set; }
    public DateTime? UseDate { get; set; }
    public string? ItemName { get; set; }
    public string? ExperimentType { get; set; }
    public int PlannedHours { get; set; }
    public double ActualHours { get; set; }
    public string? ClassName { get; set; }
    public int? ExpectedCount { get; set; }
    public int? ActualCount { get; set; }
    public string? AttendanceRecord { get; set; }
    public string? TeachingRecord { get; set; }
    public string? DeviceRecord { get; set; }
    public string RegisterStatus { get; set; } = string.Empty;
    public Guid RegisterUserId { get; set; }
    public string RegisterUserName { get; set; } = string.Empty;
    public DateTime RegisterTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UsageRegisterQueryRequest : PagedQueryRequest
{
    public Guid? SemesterId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? RegisterUserId { get; set; }
    public string? RegisterStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ============================================================
// 统计数据 DTO
// ============================================================

public class ScheduleStatisticsDto
{
    public Guid? SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public int? WeekNo { get; set; }
    public int TotalSchedules { get; set; }
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public double OccupancyRate { get; set; }
    public int TotalStudentCount { get; set; }
}

public class DashboardDataDto
{
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int PendingBookings { get; set; }
    public int TodaySchedules { get; set; }
    public double TodayOccupancyRate { get; set; }
    public int WeekOccupancyRate { get; set; }
    public List<AlertItemDto> Alerts { get; set; } = new();
}

public class AlertItemDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
