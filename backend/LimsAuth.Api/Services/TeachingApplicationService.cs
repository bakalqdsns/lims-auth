using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using System.Text.Json;

namespace LimsAuth.Api.Services;

public interface ITeachingApplicationService
{
    Task<List<TeachingApplicationDto>> GetApplicationsAsync(TeachingApplicationQuery query);
    Task<TeachingApplicationDto?> GetApplicationByIdAsync(Guid id);
    Task<TeachingApplicationDto> CreateApplicationAsync(CreateTeachingApplicationRequest request, Guid applicantId, string applicantName, string? createdBy = null);
    Task<bool> ApproveApplicationAsync(Guid id, ApprovalRequest request, Guid approverId, string? approverName = null);
    Task<bool> RejectApplicationAsync(Guid id, ApprovalRequest request, Guid approverId);
    Task<bool> CancelApplicationAsync(Guid id, string? cancelledBy = null);
    Task<List<TeachingApplicationDto>> GetPendingApplicationsAsync(Guid? semesterId);
    Task<List<TeachingApplicationDto>> GetMyApplicationsAsync(Guid userId, Guid? semesterId);
}

public class TeachingApplicationService : ITeachingApplicationService
{
    private readonly AppDbContext _db;
    private readonly IScheduleService _scheduleService;

    public TeachingApplicationService(AppDbContext db, IScheduleService scheduleService)
    {
        _db = db;
        _scheduleService = scheduleService;
    }

    public async Task<List<TeachingApplicationDto>> GetApplicationsAsync(TeachingApplicationQuery query)
    {
        var q = _db.TeachingApplications
            .Include(x => x.Semester)
            .Include(x => x.ExpectedLab)
            .AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.TeachingTaskId.HasValue)
            q = q.Where(x => x.TeachingTaskId == query.TeachingTaskId.Value);
        if (query.ApplicantId.HasValue)
            q = q.Where(x => x.ApplicantId == query.ApplicantId.Value);
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status.ToString().ToLower() == query.Status.ToLower());

        var list = await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<TeachingApplicationDto?> GetApplicationByIdAsync(Guid id)
    {
        var app = await _db.TeachingApplications
            .Include(x => x.Semester)
            .Include(x => x.ExpectedLab)
            .FirstOrDefaultAsync(x => x.Id == id);
        return app == null ? null : MapToDto(app);
    }

    public async Task<TeachingApplicationDto> CreateApplicationAsync(
        CreateTeachingApplicationRequest request,
        Guid applicantId,
        string applicantName,
        string? createdBy = null)
    {
        Guid? majorId = request.MajorId;
        string majorName = request.MajorName;
        Guid? classId = request.ClassId;
        string className = request.ClassName;

        if (request.TeachingTaskId.HasValue && (!majorId.HasValue || !classId.HasValue))
        {
            var task = await _db.ExperimentTeachingTasks
                .Include(t => t.Class)
                .Include(t => t.Major)
                .FirstOrDefaultAsync(t => t.Id == request.TeachingTaskId.Value);

            if (task != null)
            {
                if (!classId.HasValue)
                {
                    classId = task.ClassId;
                    className = task.Class?.Name ?? className;
                }
                if (!majorId.HasValue)
                {
                    majorId = task.MajorId != Guid.Empty ? task.MajorId : task.Class?.MajorId;
                    majorName = task.Major?.Name ?? task.Class?.Major?.Name ?? majorName;
                }
            }
        }

        var app = new TeachingApplication
        {
            Id = Guid.NewGuid(),
            SemesterId = request.SemesterId,
            TeachingTaskId = request.TeachingTaskId,
            CourseName = request.CourseName,
            MajorId = majorId,
            MajorName = majorName,
            ClassId = classId,
            ClassName = className,
            StartWeek = request.StartWeek,
            EndWeek = request.EndWeek,
            DayOfWeek = request.DayOfWeek,
            ExpectedLabId = request.ExpectedLabId,
            Remark = request.Remark,
            ApplicantId = applicantId,
            ApplicantName = applicantName,
            Status = ApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // FIX: 直接写入数据库列，不要通过 [NotMapped] 属性赋值
        app.PeriodNumbersJson = JsonSerializer.Serialize(request.PeriodNumbers ?? new List<int>());

        _db.TeachingApplications.Add(app);
        await _db.SaveChangesAsync();

        return MapToDto(app);
    }

    public async Task<bool> ApproveApplicationAsync(Guid id, ApprovalRequest request, Guid approverId, string? approverName = null)
    {
        var app = await _db.TeachingApplications
            .Include(x => x.ExpectedLab)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (app == null || app.Status != ApprovalStatus.Pending) return false;

        app.Status = ApprovalStatus.Approved;
        app.ApprovalComment = request.Comment;
        app.ApprovedBy = approverId;
        app.ApprovedAt = DateTime.UtcNow;
        app.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var periodList = JsonSerializer.Deserialize<List<int>>(app.PeriodNumbersJson ?? "[]") ?? new List<int>();
        var weeks = Enumerable.Range(app.StartWeek, app.EndWeek - app.StartWeek + 1);
        foreach (var week in weeks)
        {
            foreach (var period in periodList)
            {
                await _scheduleService.CreateCentralScheduleAsync(new CreateScheduleEntryRequest
                {
                    SemesterId = app.SemesterId ?? Guid.Empty,
                    LabId = app.ExpectedLabId,
                    WeekNumber = week,
                    DayOfWeek = app.DayOfWeek,
                    PeriodNumber = period,
                    Source = ScheduleSource.TeachingRequest.ToString(),
                    TeachingApplicationId = app.Id,
                    TeachingTaskId = app.TeachingTaskId,
                    CourseName = app.CourseName,
                    Remark = $"来源：授课申请 | {app.Remark}"
                }, approverName);
            }
        }

        return true;
    }

    public async Task<bool> RejectApplicationAsync(Guid id, ApprovalRequest request, Guid approverId)
    {
        var app = await _db.TeachingApplications.FindAsync(id);
        if (app == null || app.Status != ApprovalStatus.Pending) return false;

        app.Status = ApprovalStatus.Rejected;
        app.ApprovalComment = request.Comment;
        app.ApprovedBy = approverId;
        app.ApprovedAt = DateTime.UtcNow;
        app.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelApplicationAsync(Guid id, string? cancelledBy = null)
    {
        var app = await _db.TeachingApplications.FindAsync(id);
        if (app == null || app.Status == ApprovalStatus.Rejected) return false;

        app.IsCancelled = true;
        app.CancelReason = cancelledBy;
        app.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<TeachingApplicationDto>> GetPendingApplicationsAsync(Guid? semesterId)
    {
        var q = _db.TeachingApplications
            .Include(x => x.Semester)
            .Include(x => x.ExpectedLab)
            .Where(x => x.Status == ApprovalStatus.Pending && !x.IsCancelled);

        if (semesterId.HasValue)
            q = q.Where(x => x.SemesterId == semesterId.Value);

        return (await q.OrderByDescending(x => x.CreatedAt).ToListAsync()).Select(MapToDto).ToList();
    }

    public async Task<List<TeachingApplicationDto>> GetMyApplicationsAsync(Guid userId, Guid? semesterId)
    {
        var q = _db.TeachingApplications
            .Include(x => x.Semester)
            .Include(x => x.ExpectedLab)
            .Where(x => x.ApplicantId == userId);

        if (semesterId.HasValue)
            q = q.Where(x => x.SemesterId == semesterId.Value);

        return (await q.OrderByDescending(x => x.CreatedAt).ToListAsync()).Select(MapToDto).ToList();
    }

    private static TeachingApplicationDto MapToDto(TeachingApplication app)
    {
        var periodList = JsonSerializer.Deserialize<List<int>>(app.PeriodNumbersJson ?? "[]") ?? new List<int>();
        return new TeachingApplicationDto
        {
            Id = app.Id,
            SemesterId = app.SemesterId,
            SemesterName = app.Semester?.Name,
            TeachingTaskId = app.TeachingTaskId,
            CourseName = app.CourseName,
            MajorId = app.MajorId,
            MajorName = app.MajorName,
            ClassId = app.ClassId,
            ClassName = app.ClassName,
            StartWeek = app.StartWeek,
            EndWeek = app.EndWeek,
            DayOfWeek = app.DayOfWeek,
            PeriodNumbers = periodList,
            ExpectedLabId = app.ExpectedLabId,
            ExpectedLabName = app.ExpectedLab?.Name,
            Remark = app.Remark,
            ApplicantId = app.ApplicantId,
            ApplicantName = app.ApplicantName,
            Status = app.Status.ToString(),
            ApprovalComment = app.ApprovalComment,
            ApprovedBy = app.ApprovedBy,
            ApproverName = null,
            ApprovedAt = app.ApprovedAt,
            IsCancelled = app.IsCancelled,
            CancelReason = app.CancelReason,
            CreatedAt = app.CreatedAt,
            CreatedBy = app.CreatedBy
        };
    }
}
