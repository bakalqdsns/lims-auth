using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 学期状态
/// </summary>
public enum SemesterStatus
{
    NotStarted = 0,    // 未开始
    InProgress = 1,    // 进行中
    Ended = 2,         // 已结束
    Archived = 3       // 已归档
}

/// <summary>
/// 学期类型
/// </summary>
public enum SemesterType
{
    Regular = 0,       // 常规学期（春秋学期）
    Short = 1,         // 短学期（小学期）
    Winter = 2,        // 寒假学期
    Summer = 3,        // 暑假学期
    Custom = 4         // 自定义学期
}

/// <summary>
/// 学期实体 - 时间中枢
/// </summary>
[Table("semesters")]
public class Semester
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    #region 基础标识
    
    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;  // 如：2025-2026学年第一学期

    [Column("code")]
    [MaxLength(50)]
    public string? Code { get; set; }  // 学期编码，如：2025-2026-1

    [Column("external_id")]
    [MaxLength(100)]
    public string? ExternalId { get; set; }  // 外部系统对接ID

    [Column("academic_year")]
    [MaxLength(20)]
    public string? AcademicYear { get; set; }  // 学年，如：2025-2026

    [Column("semester_type")]
    public SemesterType SemesterType { get; set; } = SemesterType.Regular;

    [Column("display_order")]
    public int DisplayOrder { get; set; } = 0;  // 显示排序

    #endregion

    #region 时间范围

    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }  // 学期开始日期

    [Required]
    [Column("end_date")]
    public DateTime EndDate { get; set; }  // 学期结束日期

    [Column("teaching_start_date")]
    public DateTime? TeachingStartDate { get; set; }  // 教学开始日期（可能晚于学期开始）

    [Column("teaching_end_date")]
    public DateTime? TeachingEndDate { get; set; }  // 教学结束日期

    [Column("total_weeks")]
    public int TotalWeeks { get; set; } = 0;  // 总周数

    [Column("teaching_weeks")]
    public int TeachingWeeks { get; set; } = 0;  // 教学周数

    #endregion

    #region 关键节点 - 选课时间

    [Column("course_selection_start")]
    public DateTime? CourseSelectionStart { get; set; }  // 选课开始时间

    [Column("course_selection_end")]
    public DateTime? CourseSelectionEnd { get; set; }  // 选课结束时间

    [Column("course_selection_end_withdraw")]
    public DateTime? CourseSelectionEndWithdraw { get; set; }  // 退选截止时间

    #endregion

    #region 关键节点 - 排课时间

    [Column("scheduling_start")]
    public DateTime? SchedulingStart { get; set; }  // 排课开始时间

    [Column("scheduling_end")]
    public DateTime? SchedulingEnd { get; set; }  // 排课结束时间

    [Column("schedule_publish_time")]
    public DateTime? SchedulePublishTime { get; set; }  // 课表发布时间

    #endregion

    #region 关键节点 - 考试时间

    [Column("exam_week_start")]
    public DateTime? ExamWeekStart { get; set; }  // 考试周开始

    [Column("exam_week_end")]
    public DateTime? ExamWeekEnd { get; set; }  // 考试周结束

    [Column("grade_entry_start")]
    public DateTime? GradeEntryStart { get; set; }  // 成绩录入开始

    [Column("grade_entry_end")]
    public DateTime? GradeEntryEnd { get; set; }  // 成绩录入截止

    [Column("grade_publish_time")]
    public DateTime? GradePublishTime { get; set; }  // 成绩发布时间

    #endregion

    #region 关键节点 - 注册缴费

    [Column("registration_start")]
    public DateTime? RegistrationStart { get; set; }  // 注册开始

    [Column("registration_end")]
    public DateTime? RegistrationEnd { get; set; }  // 注册截止

    [Column("tuition_payment_start")]
    public DateTime? TuitionPaymentStart { get; set; }  // 缴费开始

    [Column("tuition_payment_end")]
    public DateTime? TuitionPaymentEnd { get; set; }  // 缴费截止

    #endregion

    #region 状态与控制

    [Column("status")]
    public SemesterStatus Status { get; set; } = SemesterStatus.NotStarted;

    [Column("is_current")]
    public bool IsCurrent { get; set; } = false;  // 是否当前学期

    [Column("is_active")]
    public bool IsActive { get; set; } = true;  // 是否启用

    [Column("is_editable")]
    public bool IsEditable { get; set; } = true;  // 是否可编辑

    [Column("is_deletable")]
    public bool IsDeletable { get; set; } = true;  // 是否可删除

    [Column("is_sandbox")]
    public bool IsSandbox { get; set; } = false;  // 是否为沙箱环境（用于模拟排课）

    [Column("is_template")]
    public bool IsTemplate { get; set; } = false;  // 是否为模板学期

    [Column("parent_semester_id")]
    public Guid? ParentSemesterId { get; set; }  // 父学期ID（用于沙箱关联）

    #endregion

    #region 数据继承与复制

    [Column("source_semester_id")]
    public Guid? SourceSemesterId { get; set; }  // 数据来源学期ID（复制时记录）

    [Column("copied_at")]
    public DateTime? CopiedAt { get; set; }  // 复制时间

    [Column("copy_settings")]
    public string? CopySettings { get; set; }  // 复制配置（JSON格式，记录复制了哪些数据）

    #endregion

    #region 描述与备注

    [Column("description")]
    [MaxLength(2000)]
    public string? Description { get; set; }  // 学期描述

    [Column("remarks")]
    [MaxLength(2000)]
    public string? Remarks { get; set; }  // 备注

    #endregion

    #region 审计字段

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public Guid? UpdatedBy { get; set; }

    [Column("archived_at")]
    public DateTime? ArchivedAt { get; set; }  // 归档时间

    [Column("archived_by")]
    public Guid? ArchivedBy { get; set; }  // 归档人

    #endregion

    // 导航属性
    public virtual ICollection<AcademicCalendar> CalendarDays { get; set; } = new List<AcademicCalendar>();
    public virtual ICollection<TeachingTask> TeachingTasks { get; set; } = new List<TeachingTask>();
    public virtual Semester? ParentSemester { get; set; }
    public virtual ICollection<Semester> ChildSemesters { get; set; } = new List<Semester>();
}

/// <summary>
/// 校历事件类型
/// </summary>
public enum CalendarEventType
{
    Teaching = 0,      // 教学日
    Exam = 1,          // 考试
    Holiday = 2,       // 节假日
    Registration = 3,  // 注册
    CourseSelection = 4, // 选课
    GradeEntry = 5,    // 成绩录入
    Sports = 6,        // 运动会
    Activity = 7,      // 校园活动
    Maintenance = 8,   // 系统维护
    Custom = 9         // 自定义
}

/// <summary>
/// 校历事件重要性
/// </summary>
public enum CalendarEventPriority
{
    Low = 0,      // 低
    Normal = 1,   // 普通
    High = 2,     // 高
    Critical = 3  // 紧急
}

/// <summary>
/// 校历/日历实体 - 业务导航
/// </summary>
[Table("academic_calendars")]
public class AcademicCalendar
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// 第几周
    /// </summary>
    [Column("week_number")]
    public int WeekNumber { get; set; }

    /// <summary>
    /// 星期几(1=周一,7=周日)
    /// </summary>
    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    #region 事件类型与属性

    [Column("event_type")]
    public CalendarEventType EventType { get; set; } = CalendarEventType.Teaching;

    [Column("event_priority")]
    public CalendarEventPriority EventPriority { get; set; } = CalendarEventPriority.Normal;

    [Column("event_name")]
    [MaxLength(200)]
    public string? EventName { get; set; }  // 事件名称

    [Column("event_code")]
    [MaxLength(50)]
    public string? EventCode { get; set; }  // 事件编码

    #endregion

    #region 时间属性

    [Column("is_holiday")]
    public bool IsHoliday { get; set; } = false;

    [Column("is_workday")]
    public bool IsWorkday { get; set; } = true;  // 是否工作日（调休时可能为假）

    [Column("is_teaching_day")]
    public bool IsTeachingDay { get; set; } = true;  // 是否教学日

    [Column("is_exam_day")]
    public bool IsExamDay { get; set; } = false;  // 是否考试日

    [Column("is_adjusted")]
    public bool IsAdjusted { get; set; } = false;  // 是否为调休调整日

    [Column("adjusted_from")]
    public DateTime? AdjustedFrom { get; set; }  // 调休来源日期

    #endregion

    #region 节假日信息

    [Column("holiday_name")]
    [MaxLength(100)]
    public string? HolidayName { get; set; }

    [Column("holiday_type")]
    [MaxLength(50)]
    public string? HolidayType { get; set; }  // 法定节假日、校定假日等

    [Column("holiday_duration")]
    public int? HolidayDuration { get; set; }  // 假期天数

    #endregion

    #region 业务联动与权限

    [Column("affects_course_selection")]
    public bool AffectsCourseSelection { get; set; } = false;  // 影响选课

    [Column("affects_scheduling")]
    public bool AffectsScheduling { get; set; } = false;  // 影响排课

    [Column("affects_grade_entry")]
    public bool AffectsGradeEntry { get; set; } = false;  // 影响成绩录入

    [Column("affects_registration")]
    public bool AffectsRegistration { get; set; } = false;  // 影响注册

    [Column("auto_trigger_action")]
    [MaxLength(100)]
    public string? AutoTriggerAction { get; set; }  // 自动触发动作（如关闭选课入口）

    [Column("triggered_at")]
    public DateTime? TriggeredAt { get; set; }  // 触发时间

    #endregion

    #region 描述与样式

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("color")]
    [MaxLength(20)]
    public string? Color { get; set; }  // 日历显示颜色

    [Column("icon")]
    [MaxLength(50)]
    public string? Icon { get; set; }  // 图标标识

    #endregion

    #region 审计字段

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public Guid? UpdatedBy { get; set; }

    #endregion

    // 导航属性
    public virtual Semester Semester { get; set; } = null!;
}

/// <summary>
/// 校历模板 - 用于快速生成校历
/// </summary>
[Table("calendar_templates")]
public class CalendarTemplate
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;  // 是否为默认模板

    [Column("template_data")]
    public string? TemplateData { get; set; }  // 模板数据（JSON格式）

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 学期操作日志
/// </summary>
[Table("semester_logs")]
public class SemesterLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("action")]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;  // 操作类型

    [Column("action_description")]
    [MaxLength(500)]
    public string? ActionDescription { get; set; }

    [Column("old_values")]
    public string? OldValues { get; set; }  // 旧值（JSON）

    [Column("new_values")]
    public string? NewValues { get; set; }  // 新值（JSON）

    [Column("performed_by")]
    public Guid? PerformedBy { get; set; }

    [Column("performed_at")]
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    [Column("ip_address")]
    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    [MaxLength(500)]
    public string? UserAgent { get; set; }
}
