using System.ComponentModel.DataAnnotations;

namespace LimsAuth.Api.Models;

// ============================================================
// 通用查询参数
// ============================================================

public class ScheduleQuery
{
    public Guid? SemesterId { get; set; }
    public int? WeekNumber { get; set; }
    public int? DayOfWeek { get; set; }
    public Guid? LabId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid? CourseId { get; set; }
    public string? Source { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AvailabilityQuery
{
    public Guid SemesterId { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public List<int> PeriodNumbers { get; set; } = new();
    public Guid? BuildingId { get; set; }
}

public class ConflictCheckResult
{
    public bool HasHardConflict { get; set; }
    public bool HasSoftConflict { get; set; }
    public List<ConflictItem> HardConflicts { get; set; } = new();
    public List<ConflictItem> SoftConflicts { get; set; } = new();
    public bool CanForceSchedule => !HasHardConflict;
}

public class ConflictItem
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? LabName { get; set; }
    public int? WeekNumber { get; set; }
    public int? DayOfWeek { get; set; }
    public int? PeriodNumber { get; set; }
}

public class ScheduleTableCell
{
    public Guid? ScheduleEntryId { get; set; }
    public string? CourseName { get; set; }
    public string? TeacherName { get; set; }
    public string? ClassName { get; set; }
    public string? LabName { get; set; }
    public string? Source { get; set; }
    public string? Status { get; set; }
    public bool HasConflict { get; set; }
    public int StudentCount { get; set; }
}

public class ScheduleTableRow
{
    public int PeriodNumber { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public List<ScheduleTableCell?> Cells { get; set; } = new();
}

// ============================================================
// ScheduleEntry DTOs
// ============================================================

public class ScheduleEntryDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? LabId { get; set; }
    public string? LabName { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? ReservationId { get; set; }
    public Guid? TeachingApplicationId { get; set; }
    public Guid? ExperimentTaskId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public string? CourseName { get; set; }
    public string? ProjectName { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public Guid? ClassId { get; set; }
    public string? ClassName { get; set; }
    public Guid? MajorId { get; set; }
    public string? MajorName { get; set; }
    public int? StudentCount { get; set; }
    public string? BuildingName { get; set; }
    public string? RoomNumber { get; set; }
    public string? Remark { get; set; }
    public bool HasConflict { get; set; }
    public string? ConflictInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateScheduleEntryRequest
{
    [Required]
    public Guid SemesterId { get; set; }
    public Guid? LabId { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public string Source { get; set; } = "CentralScheduling";
    public Guid? ReservationId { get; set; }
    public Guid? TeachingApplicationId { get; set; }
    public Guid? ExperimentTaskId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public string? CourseName { get; set; }
    public string? ProjectName { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public Guid? ClassId { get; set; }
    public string? ClassName { get; set; }
    public Guid? MajorId { get; set; }
    public string? MajorName { get; set; }
    public int? StudentCount { get; set; }
    public string? BuildingName { get; set; }
    public string? RoomNumber { get; set; }
    public string? Remark { get; set; }
    public bool ForceSchedule { get; set; }
}

public class UpdateScheduleEntryRequest
{
    public Guid? LabId { get; set; }
    public int? WeekNumber { get; set; }
    public int? DayOfWeek { get; set; }
    public int? PeriodNumber { get; set; }
    public string? Status { get; set; }
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public string? Remark { get; set; }
}

// ============================================================
// Reservation DTOs
// ============================================================

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid LabId { get; set; }
    public string? LabName { get; set; }
    public DateTime UseDate { get; set; }
    public int DayOfWeek { get; set; }
    public List<int> PeriodNumbers { get; set; } = new();
    public int WeekNumber { get; set; }
    public double? ExpectedDurationHours { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCategory { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public Guid? ProjectLeaderId { get; set; }
    public string? ProjectLeaderName { get; set; }
    public string? ProjectLeaderPhone { get; set; }
    public string? MemberGrade { get; set; }
    public Guid? MemberClassId { get; set; }
    public string? MemberClassName { get; set; }
    public int? MemberCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ApprovalComment { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateReservationRequest
{
    [Required]
    public Guid SemesterId { get; set; }
    [Required]
    public Guid LabId { get; set; }
    public DateTime UseDate { get; set; }
    public int DayOfWeek { get; set; }
    public List<int> PeriodNumbers { get; set; } = new();
    public int WeekNumber { get; set; }
    public double? ExpectedDurationHours { get; set; }
    [Required]
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCategory { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public Guid? ProjectLeaderId { get; set; }
    public string? ProjectLeaderName { get; set; }
    public string? ProjectLeaderPhone { get; set; }
    public string? MemberGrade { get; set; }
    public Guid? MemberClassId { get; set; }
    public string? MemberClassName { get; set; }
    public int? MemberCount { get; set; }
}

public class ReservationQuery
{
    public Guid? SemesterId { get; set; }
    public Guid? LabId { get; set; }
    public int? WeekNumber { get; set; }
    public string? Status { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ApprovalRequest
{
    public string Comment { get; set; } = string.Empty;
    public string? ApproverName { get; set; }
}

public class CancelRequest
{
    public string Reason { get; set; } = string.Empty;
}

// ============================================================
// TeachingApplication DTOs
// ============================================================

public class TeachingApplicationDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid TeachingTaskId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public Guid MajorId { get; set; }
    public string MajorName { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<int> WeekNumbers { get; set; } = new();
    public int DayOfWeek { get; set; }
    public List<int> PeriodNumbers { get; set; } = new();
    public Guid? ExpectedLabId { get; set; }
    public string? ExpectedLabName { get; set; }
    public string? Remark { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ApprovalComment { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateTeachingApplicationRequest
{
    [Required]
    public Guid SemesterId { get; set; }
    [Required]
    public Guid TeachingTaskId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public Guid MajorId { get; set; }
    public string MajorName { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<int> WeekNumbers { get; set; } = new();
    public int DayOfWeek { get; set; }
    public List<int> PeriodNumbers { get; set; } = new();
    public Guid? ExpectedLabId { get; set; }
    public string? Remark { get; set; }
}

public class TeachingApplicationQuery
{
    public Guid? SemesterId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public Guid? ApplicantId { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// UsageRegistration DTOs
// ============================================================

public class UsageRegistrationDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public string? RoomNumber { get; set; }
    public DateTime UseDate { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public string Source { get; set; } = string.Empty;
    public Guid? ScheduleEntryId { get; set; }
    public Guid? ReservationId { get; set; }
    public Guid? TeachingApplicationId { get; set; }
    public string? CourseName { get; set; }
    public string? ProjectName { get; set; }
    public string? ExperimentItemName { get; set; }
    public string? ExperimentItemType { get; set; }
    public int PlannedHours { get; set; }
    public double ActualHours { get; set; }
    public string? ClassName { get; set; }
    public int? ExpectedStudentCount { get; set; }
    public int? ActualStudentCount { get; set; }
    public string? AttendanceRecord { get; set; }
    public string? TeachingCondition { get; set; }
    public string? EquipmentCondition { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? RemindedAt { get; set; }
    public Guid FilledById { get; set; }
    public string FilledByName { get; set; } = string.Empty;
    public DateTime FilledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateUsageRegistrationRequest
{
    [Required]
    public Guid SemesterId { get; set; }
    public Guid? LabId { get; set; }
    public string LabName { get; set; } = string.Empty;
    public string? BuildingName { get; set; }
    public string? RoomNumber { get; set; }
    public DateTime UseDate { get; set; }
    public int WeekNumber { get; set; }
    public int DayOfWeek { get; set; }
    public int PeriodNumber { get; set; }
    public string Source { get; set; } = string.Empty;
    public Guid? ScheduleEntryId { get; set; }
    public Guid? ReservationId { get; set; }
    public Guid? TeachingApplicationId { get; set; }
    public Guid? ExperimentTaskId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public string? CourseName { get; set; }
    public string? ProjectName { get; set; }
    public string? ExperimentItemName { get; set; }
    public string? ExperimentItemType { get; set; }
    public int PlannedHours { get; set; }
    public double ActualHours { get; set; }
    public string? ClassName { get; set; }
    public int? ExpectedStudentCount { get; set; }
    public int? ActualStudentCount { get; set; }
    public string? AttendanceRecord { get; set; }
    public string? TeachingCondition { get; set; }
    public string? EquipmentCondition { get; set; }
}

public class UsageRegistrationQuery
{
    public Guid? SemesterId { get; set; }
    public Guid? LabId { get; set; }
    public Guid? FilledById { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ============================================================
// Statistics DTOs
// ============================================================

public class ScheduleStatisticsDto
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public string? SemesterName { get; set; }
    public Guid? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public Guid? LabId { get; set; }
    public string? LabName { get; set; }
    public int WeekNumber { get; set; }
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }
    public int ReservationSlots { get; set; }
    public double OccupancyRate { get; set; }
    public int TotalStudentCount { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class ScheduleStatisticsQuery
{
    public Guid? SemesterId { get; set; }
    public int? WeekNumber { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? LabId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class StatisticsQuery
{
    public Guid? SemesterId { get; set; }
    public int? WeekNumber { get; set; }
    public int? StartWeek { get; set; }
    public int? EndWeek { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? LabId { get; set; }
    public Guid? MajorId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? CourseId { get; set; }
}

public class DashboardQuery
{
    public Guid? SemesterId { get; set; }
    public int? WeekNumber { get; set; }
}

public class DashboardData
{
    public TodaySummary Today { get; set; } = new();
    public WeekSummary Week { get; set; } = new();
    public List<OccupancyTrend> OccupancyTrends { get; set; } = new();
    public List<LabOccupancy> LabOccupancyList { get; set; } = new();
    public List<CategoryStat> CategoryStats { get; set; } = new();
    public CompletionRate CompletionRate { get; set; } = new();
    public List<AlertItem> Alerts { get; set; } = new();
}

public class TodaySummary
{
    public int TotalLabs { get; set; }
    public int OccupiedLabs { get; set; }
    public int AvailableLabs { get; set; }
    public double OccupancyRate { get; set; }
    public int TotalSchedules { get; set; }
    public int PendingReservations { get; set; }
    public int PendingRegistrations { get; set; }
}

public class WeekSummary
{
    public int TotalSlots { get; set; }
    public int UsedSlots { get; set; }
    public double OccupancyRate { get; set; }
    public int TotalStudentCount { get; set; }
    public int TotalReservations { get; set; }
    public int ApprovedReservations { get; set; }
    public int TotalTeachingApplications { get; set; }
    public int ApprovedTeachingApplications { get; set; }
}

public class OccupancyTrend
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class LabOccupancy
{
    public string LabName { get; set; } = string.Empty;
    public double OccupancyRate { get; set; }
    public int UsedSlots { get; set; }
    public int TotalSlots { get; set; }
}

public class CategoryStat
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CompletionRate
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int Overdue { get; set; }
    public double Rate { get; set; }
}

public class AlertItem
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string? RelatedId { get; set; }
}
