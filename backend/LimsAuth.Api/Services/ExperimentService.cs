using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IExperimentService
{
    // ExperimentTeachingTask
    Task<List<ExperimentTeachingTask>> GetTasksAsync();
    Task<ExperimentTeachingTask?> GetTaskByIdAsync(Guid id);
    Task<ExperimentTeachingTask> CreateTaskAsync(ExperimentTeachingTask task);
    Task<bool> UpdateTaskAsync(Guid id, ExperimentTeachingTask task);
    Task<bool> DeleteTaskAsync(Guid id, string? deletedBy = null);

    // ExperimentItem
    Task<List<ExperimentItem>> GetItemsAsync();
    Task<ExperimentItem?> GetItemByIdAsync(Guid id);
    Task<ExperimentItem> CreateItemAsync(ExperimentItem item);
    Task<bool> UpdateItemAsync(Guid id, ExperimentItem item);
    Task<bool> DeleteItemAsync(Guid id);

    // ExperimentItemSchedule
    Task<List<ExperimentItemSchedule>> GetSchedulesByTaskAsync(Guid taskId);
    Task<ExperimentItemSchedule> CreateScheduleAsync(ExperimentItemSchedule schedule);
    Task<bool> UpdateScheduleAsync(Guid id, ExperimentItemSchedule schedule);
    Task<bool> DeleteScheduleAsync(Guid id);

    // ExperimentQualityAssessment
    Task<ExperimentQualityAssessment?> GetAssessmentAsync(Guid taskId);
    Task<ExperimentQualityAssessment> SaveAssessmentAsync(ExperimentQualityAssessment assessment);

    // TrainingTeachingPlan
    Task<List<TrainingTeachingPlan>> GetTrainingPlansAsync();
    Task<TrainingTeachingPlan?> GetTrainingPlanByIdAsync(Guid id);
    Task<TrainingTeachingPlan> CreateTrainingPlanAsync(TrainingTeachingPlan plan);
    Task<bool> UpdateTrainingPlanAsync(Guid id, TrainingTeachingPlan plan);
    Task<bool> DeleteTrainingPlanAsync(Guid id);
    Task<List<TrainingTeachingPlan>> GetTrainingPlansByStatusAsync(string? status, string? approvalStatus);
    Task<bool> ApproveTrainingPlanAsync(Guid id, string approvalType, string opinion, string? approver, string status);
}

public class ExperimentService : IExperimentService
{
    private readonly AppDbContext _db;

    public ExperimentService(AppDbContext db)
    {
        _db = db;
    }

    // =========================
    // ?????????
    // =========================

    public async Task<List<ExperimentTeachingTask>> GetTasksAsync()
    {
        return await _db.ExperimentTeachingTasks
            .Include(x => x.Semester)
            .Include(x => x.Major)
            .Include(x => x.Class)
            .Include(x => x.Department)
            .Include(x => x.Institution)
            .Include(x => x.Schedules)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ExperimentTeachingTask?> GetTaskByIdAsync(Guid id)
    {
        return await _db.ExperimentTeachingTasks
            .Include(x => x.Schedules)
                .ThenInclude(s => s.ExperimentItem)
            .Include(x => x.QualityAssessment)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ExperimentTeachingTask> CreateTaskAsync(ExperimentTeachingTask task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.ExperimentTeachingTasks.Add(task);
        await _db.SaveChangesAsync();

        return task;
    }

    public async Task<bool> UpdateTaskAsync(Guid id, ExperimentTeachingTask input)
    {
        var task = await _db.ExperimentTeachingTasks.FindAsync(id);
        if (task == null) return false;

        task.SemesterId = input.SemesterId;
        task.MajorId = input.MajorId;
        task.ClassId = input.ClassId;
        task.CourseName = input.CourseName;
        task.StudentCount = input.StudentCount;
        task.StudentLevel = input.StudentLevel;
        task.CourseType = input.CourseType;
        task.IsIndependentCourse = input.IsIndependentCourse;

        task.TotalExperimentHours = input.TotalExperimentHours;
        task.CurrentSemesterExperimentHours = input.CurrentSemesterExperimentHours;

        task.TotalPracticeHours = input.TotalPracticeHours;
        task.CurrentSemesterPracticeHours = input.CurrentSemesterPracticeHours;

        task.TotalTrainingHours = input.TotalTrainingHours;
        task.CurrentSemesterTrainingHours = input.CurrentSemesterTrainingHours;

        task.InstitutionId = input.InstitutionId;
        task.DepartmentId = input.DepartmentId;

        task.TeacherIds = input.TeacherIds;
        task.TeacherNames = input.TeacherNames;
        task.TeacherTitles = input.TeacherTitles;

        task.TechnicalStaff = input.TechnicalStaff;
        task.TechnicalTitle = input.TechnicalTitle;

        task.TextbookName = input.TextbookName;
        task.ExperimentGuideName = input.ExperimentGuideName;

        task.SortOrder = input.SortOrder;
        task.Description = input.Description;
        task.Status = input.Status;

        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id, string? deletedBy = null)
    {
        var task = await _db.ExperimentTeachingTasks
            .Include(t => t.Schedules)
            .Include(t => t.QualityAssessment)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return false;

        // 级联删除关联的 Schedules
        if (task.Schedules?.Count > 0)
            _db.ExperimentItemSchedules.RemoveRange(task.Schedules);

        // 级联删除关联的 QualityAssessment
        if (task.QualityAssessment != null)
            _db.ExperimentQualityAssessments.Remove(task.QualityAssessment);

        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedBy = deletedBy;
        _db.ExperimentTeachingTasks.Remove(task);
        await _db.SaveChangesAsync();

        return true;
    }

    // =========================
    // ??????
    // =========================

    public async Task<List<ExperimentItem>> GetItemsAsync()
    {
        return await _db.ExperimentItems
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
    }

    public async Task<ExperimentItem> CreateItemAsync(ExperimentItem item)
    {
        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _db.ExperimentItems.Add(item);
        await _db.SaveChangesAsync();

        return item;
    }

    public async Task<ExperimentItem?> GetItemByIdAsync(Guid id)
    {
        return await _db.ExperimentItems
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateItemAsync(Guid id, ExperimentItem input)
    {
        var item = await _db.ExperimentItems.FindAsync(id);
        if (item == null) return false;

        item.CourseCode = input.CourseCode;
        item.ExperimentName = input.ExperimentName;
        item.ExperimentHours = input.ExperimentHours;
        item.ExperimentType = input.ExperimentType;
        item.ExperimentRequirement = input.ExperimentRequirement;
        item.Status = input.Status;
        item.SortOrder = input.SortOrder;
        item.Description = input.Description;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemAsync(Guid id)
    {
        var item = await _db.ExperimentItems.FindAsync(id);
        if (item == null) return false;

        _db.ExperimentItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    // =========================
    // ?????Schedule??
    // =========================

    public async Task<List<ExperimentItemSchedule>> GetSchedulesByTaskAsync(Guid taskId)
    {
        return await _db.ExperimentItemSchedules
            .Include(x => x.ExperimentItem)
            .Include(x => x.Lab)
                .ThenInclude(l => l!.Building)
                    .ThenInclude(b => b!.Campus)
            .Where(x => x.ExperimentTaskId == taskId)
            .OrderBy(x => x.WeekNumber)
            .ThenBy(x => x.DayOfWeek)
            .ToListAsync();
    }

    public async Task<ExperimentItemSchedule> CreateScheduleAsync(ExperimentItemSchedule schedule)
    {
        schedule.Id = Guid.NewGuid();
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.UpdatedAt = DateTime.UtcNow;

        _db.ExperimentItemSchedules.Add(schedule);
        await _db.SaveChangesAsync();

        return schedule;
    }

    public async Task<bool> UpdateScheduleAsync(Guid id, ExperimentItemSchedule input)
    {
        var schedule = await _db.ExperimentItemSchedules.FindAsync(id);
        if (schedule == null) return false;

        schedule.ExperimentTaskId = input.ExperimentTaskId;
        schedule.ExperimentItemId = input.ExperimentItemId;
        schedule.WeekNumber = input.WeekNumber;
        schedule.DayOfWeek = input.DayOfWeek;
        schedule.PeriodNumber = input.PeriodNumber;
        schedule.ParallelGroups = input.ParallelGroups;
        schedule.StudentsPerGroup = input.StudentsPerGroup;
        schedule.CycleCount = input.CycleCount;
        schedule.ExperimentRequirement = input.ExperimentRequirement;
        schedule.LabId = input.LabId;
        schedule.Location = input.Location;
        schedule.IsConducted = input.IsConducted;
        schedule.ReasonIfNotConducted = input.ReasonIfNotConducted;
        schedule.Status = input.Status;
        schedule.SortOrder = input.SortOrder;
        schedule.Description = input.Description;
        schedule.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteScheduleAsync(Guid id)
    {
        var schedule = await _db.ExperimentItemSchedules.FindAsync(id);
        if (schedule == null) return false;

        _db.ExperimentItemSchedules.Remove(schedule);
        await _db.SaveChangesAsync();
        return true;
    }

    // =========================
    // ???????????
    // =========================

    public async Task<ExperimentQualityAssessment?> GetAssessmentAsync(Guid taskId)
    {
        return await _db.ExperimentQualityAssessments
            .FirstOrDefaultAsync(x => x.ExperimentTaskId == taskId);
    }

    public async Task<ExperimentQualityAssessment> SaveAssessmentAsync(ExperimentQualityAssessment input)
    {
        var existing = await _db.ExperimentQualityAssessments
            .FirstOrDefaultAsync(x => x.ExperimentTaskId == input.ExperimentTaskId);

        if (existing == null)
        {
            input.Id = Guid.NewGuid();
            input.CreatedAt = DateTime.UtcNow;

            _db.ExperimentQualityAssessments.Add(input);
        }
        else
        {
            existing.CourseName = input.CourseName;
            existing.ExperimentHours = input.ExperimentHours;
            existing.IsIndependentCourse = input.IsIndependentCourse;

            existing.MainTeacher = input.MainTeacher;
            existing.TeacherTitle = input.TeacherTitle;

            existing.TechnicalStaff = input.TechnicalStaff;
            existing.TechnicalTitle = input.TechnicalTitle;

            existing.InstitutionId = input.InstitutionId;
            existing.ClassName = input.ClassName;
            existing.ClassStudentCount = input.ClassStudentCount;

            existing.PlannedExperimentCount = input.PlannedExperimentCount;
            existing.ActualExperimentCount = input.ActualExperimentCount;

            existing.MissedExperimentItems = input.MissedExperimentItems;
            existing.AssessmentMethod = input.AssessmentMethod;
            existing.AssessmentStudentCount = input.AssessmentStudentCount;
            existing.AssessmentTime = input.AssessmentTime;

            existing.SortOrder = input.SortOrder;
            existing.Description = input.Description;
            existing.Status = input.Status;

            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return input;
    }

    // =========================
    // 实训教学计划
    // =========================

    public async Task<List<TrainingTeachingPlan>> GetTrainingPlansAsync()
    {
        return await _db.TrainingTeachingPlans
            .Include(x => x.Semester)
            .Include(x => x.Course)
            .Include(x => x.Major)
            .Include(x => x.Class)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<TrainingTeachingPlan?> GetTrainingPlanByIdAsync(Guid id)
    {
        return await _db.TrainingTeachingPlans
            .Include(x => x.Semester)
            .Include(x => x.Course)
            .Include(x => x.Major)
            .Include(x => x.Class)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TrainingTeachingPlan> CreateTrainingPlanAsync(TrainingTeachingPlan plan)
    {
        plan.Id = Guid.NewGuid();
        plan.CreatedAt = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;

        _db.TrainingTeachingPlans.Add(plan);
        await _db.SaveChangesAsync();

        return plan;
    }

    public async Task<bool> UpdateTrainingPlanAsync(Guid id, TrainingTeachingPlan input)
    {
        var plan = await _db.TrainingTeachingPlans.FindAsync(id);
        if (plan == null) return false;

        plan.SemesterId = input.SemesterId;
        plan.CourseId = input.CourseId;
        plan.CourseName = input.CourseName;
        plan.CourseCode = input.CourseCode;
        plan.MajorId = input.MajorId;
        plan.ClassId = input.ClassId;
        plan.StudentCount = input.StudentCount;
        plan.StudentLevel = input.StudentLevel;
        plan.TeachingOrganizationMethod = input.TeachingOrganizationMethod;
        plan.TeachingLocation = input.TeachingLocation;
        plan.TeachingPurpose = input.TeachingPurpose;
        plan.TeachingRequirements = input.TeachingRequirements;
        plan.TeachingContent = input.TeachingContent;
        plan.TeachingProgressSchedule = input.TeachingProgressSchedule;
        plan.TrainingMethod = input.TrainingMethod;
        plan.CycleGroupInfo = input.CycleGroupInfo;
        plan.AssessmentMethod = input.AssessmentMethod;
        plan.AssessmentRequirements = input.AssessmentRequirements;
        plan.QualityAssuranceMeasures = input.QualityAssuranceMeasures;
        plan.QualityAssuranceDetails = input.QualityAssuranceDetails;
        plan.ExperimentCenterOpinion = input.ExperimentCenterOpinion;
        plan.ExperimentCenterOpinionStatus = input.ExperimentCenterOpinionStatus;
        plan.ExperimentCenterApprovedBy = input.ExperimentCenterApprovedBy;
        plan.ExperimentCenterApprovalDate = input.ExperimentCenterApprovalDate;
        plan.DepartmentOpinion = input.DepartmentOpinion;
        plan.DepartmentOpinionStatus = input.DepartmentOpinionStatus;
        plan.DepartmentApprovedBy = input.DepartmentApprovedBy;
        plan.DepartmentApprovalDate = input.DepartmentApprovalDate;
        plan.SortOrder = input.SortOrder;
        plan.Description = input.Description;
        plan.Status = input.Status;

        plan.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTrainingPlanAsync(Guid id)
    {
        var plan = await _db.TrainingTeachingPlans.FindAsync(id);
        if (plan == null) return false;

        _db.TrainingTeachingPlans.Remove(plan);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ApproveTrainingPlanAsync(Guid id, string approvalType, string opinion, string? approver, string status)
    {
        var plan = await _db.TrainingTeachingPlans.FindAsync(id);
        if (plan == null) return false;

        if (approvalType == "ExperimentCenter")
        {
            plan.ExperimentCenterOpinion = opinion;
            plan.ExperimentCenterOpinionStatus = status;
            plan.ExperimentCenterApprovedBy = approver;
            plan.ExperimentCenterApprovalDate = status == "Approved" || status == "Rejected" ? DateTime.UtcNow : null;
        }
        else if (approvalType == "Department")
        {
            plan.DepartmentOpinion = opinion;
            plan.DepartmentOpinionStatus = status;
            plan.DepartmentApprovedBy = approver;
            plan.DepartmentApprovalDate = status == "Approved" || status == "Rejected" ? DateTime.UtcNow : null;
        }

        plan.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<TrainingTeachingPlan>> GetTrainingPlansByStatusAsync(string? status, string? approvalStatus)
    {
        var query = _db.TrainingTeachingPlans
            .Include(x => x.Semester)
            .Include(x => x.Course)
            .Include(x => x.Major)
            .Include(x => x.Class)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(approvalStatus))
        {
            if (approvalStatus == "PendingExperimentCenter")
                query = query.Where(x => x.ExperimentCenterOpinionStatus == "Pending" || x.ExperimentCenterOpinionStatus == null);
            else if (approvalStatus == "PendingDepartment")
                query = query.Where(x => x.DepartmentOpinionStatus == "Pending" || x.DepartmentOpinionStatus == null);
            else if (approvalStatus == "Approved")
                query = query.Where(x => x.ExperimentCenterOpinionStatus == "Approved" && x.DepartmentOpinionStatus == "Approved");
            else if (approvalStatus == "Rejected")
                query = query.Where(x =>
                    x.ExperimentCenterOpinionStatus == "Rejected" || x.DepartmentOpinionStatus == "Rejected");
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
}