using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimsAuth.Api.Models;

/// <summary>
/// 实验教学任务实体
/// </summary>
[Table("experiment_teaching_tasks")]
public class ExperimentTeachingTask
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Required]
    [Column("major_id")]
    public Guid MajorId { get; set; }

    [Required]
    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("student_count")]
    public int StudentCount { get; set; }

    [Column("student_level")]
    [MaxLength(50)]
    public string? StudentLevel { get; set; }

    [Required]
    [Column("course_name")]
    [MaxLength(200)]
    public string CourseName { get; set; } = string.Empty;

    [Column("course_type")]
    [MaxLength(50)]
    public string? CourseType { get; set; }

    [Column("is_independent_course")]
    public bool IsIndependentCourse { get; set; }

    [Column("total_experiment_hours")]
    public int TotalExperimentHours { get; set; }

    [Column("current_semester_experiment_hours")]
    public int CurrentSemesterExperimentHours { get; set; }

    [Column("total_practice_hours")]
    public int TotalPracticeHours { get; set; }

    [Column("current_semester_practice_hours")]
    public int CurrentSemesterPracticeHours { get; set; }

    [Column("total_training_hours")]
    public int TotalTrainingHours { get; set; }

    [Column("current_semester_training_hours")]
    public int CurrentSemesterTrainingHours { get; set; }

    [Column("institution_id")]
    public Guid? InstitutionId { get; set; }

    [Column("department_id")]
    public Guid? DepartmentId { get; set; }

    [Column("teacher_ids")]
    [MaxLength(500)]
    public string? TeacherIds { get; set; }

    [Column("teacher_titles")]
    [MaxLength(200)]
    public string? TeacherTitles { get; set; }

    [Column("technical_staff")]
    [MaxLength(100)]
    public string? TechnicalStaff { get; set; }

    [Column("technical_title")]
    [MaxLength(50)]
    public string? TechnicalTitle { get; set; }

    [Column("textbook_name")]
    [MaxLength(200)]
    public string? TextbookName { get; set; }

    [Column("experiment_guide_name")]
    [MaxLength(200)]
    public string? ExperimentGuideName { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual Semester? Semester { get; set; }
    public virtual Major? Major { get; set; }
    public virtual Class? Class { get; set; }
    public virtual Department? Department { get; set; }
    public virtual ICollection<ExperimentItemSchedule> Schedules { get; set; } = new List<ExperimentItemSchedule>();
    public virtual ExperimentQualityAssessment? QualityAssessment { get; set; }
}

/// <summary>
/// 实验项目实体
/// </summary>
[Table("experiment_items")]
public class ExperimentItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("course_code")]
    [MaxLength(50)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [Column("experiment_name")]
    [MaxLength(200)]
    public string ExperimentName { get; set; } = string.Empty;

    [Column("experiment_hours")]
    public int ExperimentHours { get; set; }

    [Column("experiment_type")]
    [MaxLength(50)]
    public string? ExperimentType { get; set; }

    [Column("experiment_requirement")]
    [MaxLength(50)]
    public string? ExperimentRequirement { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ICollection<ExperimentItemSchedule> Schedules { get; set; } = new List<ExperimentItemSchedule>();
}

/// <summary>
/// 实验项目开出安排实体
/// </summary>
[Table("experiment_item_schedules")]
public class ExperimentItemSchedule
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("experiment_task_id")]
    public Guid ExperimentTaskId { get; set; }

    [Required]
    [Column("experiment_item_id")]
    public Guid ExperimentItemId { get; set; }

    [Column("week_number")]
    public int? WeekNumber { get; set; }

    [Column("day_of_week")]
    public int? DayOfWeek { get; set; }

    [Column("period_number")]
    public int? PeriodNumber { get; set; }

    [Column("parallel_groups")]
    public int? ParallelGroups { get; set; }

    [Column("students_per_group")]
    public int? StudentsPerGroup { get; set; }

    [Column("cycle_count")]
    public int? CycleCount { get; set; }

    [Column("experiment_requirement")]
    [MaxLength(100)]
    public string? ExperimentRequirement { get; set; }

    [Column("location")]
    [MaxLength(200)]
    public string? Location { get; set; }

    [Column("lab_id")]
    public Guid? LabId { get; set; }

    [Column("is_conducted")]
    public bool IsConducted { get; set; }

    [Column("reason_if_not_conducted")]
    [MaxLength(500)]
    public string? ReasonIfNotConducted { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ExperimentTeachingTask? ExperimentTask { get; set; }
    public virtual ExperimentItem? ExperimentItem { get; set; }
    public virtual Lab? Lab { get; set; }
}

/// <summary>
/// 实验课程教学质量评估实体
/// </summary>
[Table("experiment_quality_assessments")]
public class ExperimentQualityAssessment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("experiment_task_id")]
    public Guid ExperimentTaskId { get; set; }

    [Column("institution_id")]
    public Guid? InstitutionId { get; set; }

    [Required]
    [Column("course_name")]
    [MaxLength(200)]
    public string CourseName { get; set; } = string.Empty;

    [Column("experiment_hours")]
    public int ExperimentHours { get; set; }

    [Column("is_independent_course")]
    public bool IsIndependentCourse { get; set; }

    [Column("main_teacher")]
    [MaxLength(100)]
    public string? MainTeacher { get; set; }

    [Column("teacher_title")]
    [MaxLength(50)]
    public string? TeacherTitle { get; set; }

    [Column("technical_staff")]
    [MaxLength(100)]
    public string? TechnicalStaff { get; set; }

    [Column("technical_title")]
    [MaxLength(50)]
    public string? TechnicalTitle { get; set; }

    [Column("class_name")]
    [MaxLength(100)]
    public string? ClassName { get; set; }

    [Column("class_student_count")]
    public int ClassStudentCount { get; set; }

    [Column("planned_experiment_count")]
    public int PlannedExperimentCount { get; set; }

    [Column("actual_experiment_count")]
    public int ActualExperimentCount { get; set; }

    [Column("missed_experiment_items")]
    [MaxLength(1000)]
    public string? MissedExperimentItems { get; set; }

    [Column("assessment_method")]
    [MaxLength(100)]
    public string? AssessmentMethod { get; set; }

    [Column("assessment_student_count")]
    public int? AssessmentStudentCount { get; set; }

    [Column("assessment_time")]
    [MaxLength(50)]
    public string? AssessmentTime { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ExperimentTeachingTask? ExperimentTask { get; set; }
    public virtual SysInstitution? Institution { get; set; }
}

/// <summary>
/// 实训教学计划实体
/// </summary>
[Table("training_teaching_plans")]
public class TrainingTeachingPlan
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("course_id")]
    public Guid CourseId { get; set; }

    [Column("teaching_organization_method")]
    [MaxLength(200)]
    public string? TeachingOrganizationMethod { get; set; }

    [Column("teaching_location")]
    [MaxLength(200)]
    public string? TeachingLocation { get; set; }

    [Column("teaching_purpose")]
    [MaxLength(1000)]
    public string? TeachingPurpose { get; set; }

    [Column("teaching_content")]
    [MaxLength(2000)]
    public string? TeachingContent { get; set; }

    [Column("training_method")]
    [MaxLength(200)]
    public string? TrainingMethod { get; set; }

    [Column("assessment_method")]
    [MaxLength(100)]
    public string? AssessmentMethod { get; set; }

    [Column("quality_assurance_measures")]
    [MaxLength(1000)]
    public string? QualityAssuranceMeasures { get; set; }

    [Column("experiment_center_opinion")]
    [MaxLength(1000)]
    public string? ExperimentCenterOpinion { get; set; }

    [Column("department_opinion")]
    [MaxLength(1000)]
    public string? DepartmentOpinion { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual Course? Course { get; set; }
}

/// <summary>
/// 楼宇实体
/// </summary>
[Table("ven_buildings")]
public class VenBuilding
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("code")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("english_name")]
    [MaxLength(200)]
    public string? EnglishName { get; set; }

    [Column("address")]
    [MaxLength(500)]
    public string? Address { get; set; }

    [Column("total_floors")]
    public int TotalFloors { get; set; }

    [Column("area")]
    public double Area { get; set; }

    [Column("build_year")]
    public int? BuildYear { get; set; }

    [Column("use_type")]
    [MaxLength(50)]
    public string? UseType { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual ICollection<VenRoom> Rooms { get; set; } = new List<VenRoom>();
}

/// <summary>
/// 场地/实验室实体
/// </summary>
[Table("ven_rooms")]
public class VenRoom
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("code")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("building_id")]
    public Guid? BuildingId { get; set; }

    [Column("floor_no")]
    public int? FloorNo { get; set; }

    [Column("room_number")]
    [MaxLength(50)]
    public string? RoomNumber { get; set; }

    [Column("seat_count")]
    public int SeatCount { get; set; }

    [Column("area")]
    public double Area { get; set; }

    [Column("room_type")]
    [MaxLength(50)]
    public string? RoomType { get; set; }

    [Column("photo")]
    [MaxLength(500)]
    public string? Photo { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("experiment_location_code")]
    [MaxLength(50)]
    public string? ExperimentLocationCode { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual VenBuilding? Building { get; set; }
}

/// <summary>
/// 机构实体
/// </summary>
[Table("sys_institutions")]
public class SysInstitution
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("code")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    [Column("institution_type")]
    [MaxLength(50)]
    public string? InstitutionType { get; set; }

    [Column("level")]
    public int Level { get; set; }

    [Column("full_path")]
    [MaxLength(500)]
    public string? FullPath { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public string? CreatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public string? UpdatedBy { get; set; }

    // 导航属性
    public virtual SysInstitution? Parent { get; set; }
    public virtual ICollection<SysInstitution> Children { get; set; } = new List<SysInstitution>();
}
