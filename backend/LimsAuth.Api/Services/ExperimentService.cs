using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IExperimentService
{
    Task<List<ExperimentTeachingTask>> GetTasksAsync();
    Task<ExperimentTeachingTask?> GetTaskByIdAsync(Guid id);

    Task<ExperimentTeachingTask> CreateTaskAsync(ExperimentTeachingTask task);
    Task<bool> UpdateTaskAsync(Guid id, ExperimentTeachingTask task);
    Task<bool> DeleteTaskAsync(Guid id);

    Task<List<ExperimentItem>> GetItemsAsync();
    Task<ExperimentItem> CreateItemAsync(ExperimentItem item);

    Task<List<ExperimentItemSchedule>> GetSchedulesByTaskAsync(Guid taskId);
    Task<ExperimentItemSchedule> CreateScheduleAsync(ExperimentItemSchedule schedule);

    Task<ExperimentQualityAssessment?> GetAssessmentAsync(Guid taskId);
    Task<ExperimentQualityAssessment> SaveAssessmentAsync(ExperimentQualityAssessment assessment);
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
            .Include(x => x.Major)
            .Include(x => x.Class)
            .Include(x => x.Semester)
            .Include(x => x.Department)
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

        // ???????????????????????
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

        task.TeacherIds = input.TeacherIds;
        task.TeacherTitles = input.TeacherTitles;

        task.TechnicalStaff = input.TechnicalStaff;
        task.TechnicalTitle = input.TechnicalTitle;

        task.Description = input.Description;
        task.Status = input.Status;

        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _db.ExperimentTeachingTasks.FindAsync(id);
        if (task == null) return false;

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

    // =========================
    // ?????Schedule??
    // =========================

    public async Task<List<ExperimentItemSchedule>> GetSchedulesByTaskAsync(Guid taskId)
    {
        return await _db.ExperimentItemSchedules
            .Include(x => x.ExperimentItem)
            .Include(x => x.Lab)
                .ThenInclude(l => l!.Building)
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

            existing.ClassName = input.ClassName;
            existing.ClassStudentCount = input.ClassStudentCount;

            existing.PlannedExperimentCount = input.PlannedExperimentCount;
            existing.ActualExperimentCount = input.ActualExperimentCount;

            existing.MissedExperimentItems = input.MissedExperimentItems;
            existing.AssessmentMethod = input.AssessmentMethod;

            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return input;
    }
}