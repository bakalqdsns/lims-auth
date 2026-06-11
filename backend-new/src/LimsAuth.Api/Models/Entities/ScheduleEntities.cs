namespace LimsAuth.Api.Models.Entities;

// ============================================================
// 排课预约实体
// ============================================================

/// <summary>
/// 排课记录实体 (Lab_Schedule)
/// </summary>
[EntityTypeConfiguration(typeof(LabScheduleConfiguration))]
public class LabSchedule
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? TeachingTaskId { get; set; }
    public int WeekNo { get; set; }
    public int DayOfWeek { get; set; }
    public string SectionNo { get; set; } = string.Empty;
    public string? CourseName { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid? ExperimentItemId { get; set; }
    public string ScheduleType { get; set; } = "CentralScheduling";
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public EduSemester Semester { get; set; } = null!;
    public LabRoom? Room { get; set; }
    public EduTeachingTask? TeachingTask { get; set; }
    public EduClass? Class { get; set; }
    public SysUser? Teacher { get; set; }
    public LabExperimentItem? ExperimentItem { get; set; }
    public ICollection<LabUsageRegister> UsageRegisters { get; set; } = new List<LabUsageRegister>();
}

/// <summary>
/// 实验项目库实体 (Lab_ExperimentItem)
/// </summary>
[EntityTypeConfiguration(typeof(LabExperimentItemConfiguration))]
public class LabExperimentItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CourseId { get; set; }
    public string? ExperimentType { get; set; }
    public int StandardHours { get; set; }
    public int Status { get; set; } = 1;
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public EduCourse? Course { get; set; }
    public ICollection<LabSchedule> Schedules { get; set; } = new List<LabSchedule>();
}

/// <summary>
/// 预约申请表实体 (Lab_BookingApply)
/// </summary>
[EntityTypeConfiguration(typeof(LabBookingApplyConfiguration))]
public class LabBookingApply
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ApplicantId { get; set; }
    public string ApplicantType { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public string? Purpose { get; set; }
    public DateTime TargetDate { get; set; }
    public string TargetSection { get; set; } = string.Empty;
    public int WeekNo { get; set; }
    public int EstimatedPeople { get; set; }
    public string AuditStatus { get; set; } = "Pending";
    public string? AuditOpinion { get; set; }
    public Guid? AuditorId { get; set; }
    public DateTime? AuditTime { get; set; }
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public SysUser Applicant { get; set; } = null!;
    public LabRoom? Room { get; set; }
    public SysUser? Auditor { get; set; }
    public ICollection<LabUsageRegister> UsageRegisters { get; set; } = new List<LabUsageRegister>();
}

/// <summary>
/// 使用登记表实体 (Lab_UsageRegister)
/// </summary>
[EntityTypeConfiguration(typeof(LabUsageRegisterConfiguration))]
public class LabUsageRegister
{
    public Guid Id { get; set; }
    public Guid? ScheduleId { get; set; }
    public Guid? BookingApplyId { get; set; }
    public Guid? SemesterId { get; set; }
    public Guid? RoomId { get; set; }
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
    public string RegisterStatus { get; set; } = "Pending";
    public Guid RegisterUserId { get; set; }
    public DateTime RegisterTime { get; set; } = DateTime.UtcNow;
    public int IsDeleted { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public LabSchedule? Schedule { get; set; }
    public LabBookingApply? BookingApply { get; set; }
    public EduSemester? Semester { get; set; }
    public LabRoom? Room { get; set; }
    public SysUser RegisterUser { get; set; } = null!;
}
