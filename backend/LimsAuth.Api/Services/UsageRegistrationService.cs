using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IUsageRegistrationService
{
    Task<List<UsageRegistrationDto>> GetRegistrationsAsync(UsageRegistrationQuery query);
    Task<UsageRegistrationDto?> GetRegistrationByIdAsync(Guid id);
    Task<UsageRegistrationDto> CreateRegistrationAsync(CreateUsageRegistrationRequest request, Guid filledById, string filledByName, string? createdBy = null);
    Task<List<UsageRegistrationDto>> GetPendingRegistrationsAsync(Guid? userId, Guid? semesterId);
    Task<List<UsageRegistrationDto>> GetOverdueRegistrationsAsync(Guid? semesterId);
    Task<int> CheckOverdueRegistrationsAsync();
}

public class UsageRegistrationService : IUsageRegistrationService
{
    private readonly AppDbContext _db;

    public UsageRegistrationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UsageRegistrationDto>> GetRegistrationsAsync(UsageRegistrationQuery query)
    {
        var q = _db.UsageRegistrations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.LabId.HasValue)
            q = q.Where(x => x.LabId == query.LabId.Value);
        if (query.FilledById.HasValue)
            q = q.Where(x => x.FilledById == query.FilledById.Value);
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status.ToString() == query.Status);
        if (query.StartDate.HasValue)
            q = q.Where(x => x.UseDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.UseDate <= query.EndDate.Value);

        var list = await q
            .OrderByDescending(x => x.UseDate)
            .ThenBy(x => x.PeriodNumber)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    public async Task<UsageRegistrationDto?> GetRegistrationByIdAsync(Guid id)
    {
        var r = await _db.UsageRegistrations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .FirstOrDefaultAsync(x => x.Id == id);
        return r == null ? null : MapToDto(r);
    }

    public async Task<UsageRegistrationDto> CreateRegistrationAsync(
        CreateUsageRegistrationRequest request,
        Guid filledById,
        string filledByName,
        string? createdBy = null)
    {
        var r = new UsageRegistration
        {
            Id = Guid.NewGuid(),
            SemesterId = request.SemesterId,
            LabId = request.LabId,
            LabName = request.LabName,
            BuildingName = request.BuildingName,
            RoomNumber = request.RoomNumber,
            UseDate = request.UseDate,
            WeekNumber = request.WeekNumber,
            DayOfWeek = request.DayOfWeek,
            PeriodNumber = request.PeriodNumber,
            Source = Enum.TryParse<ScheduleSource>(request.Source, true, out var s) ? s : ScheduleSource.CentralScheduling,
            ScheduleEntryId = request.ScheduleEntryId,
            ReservationId = request.ReservationId,
            TeachingApplicationId = request.TeachingApplicationId,
            ExperimentTaskId = request.ExperimentTaskId,
            TeachingTaskId = request.TeachingTaskId,
            CourseName = request.CourseName,
            ProjectName = request.ProjectName,
            ExperimentItemName = request.ExperimentItemName,
            ExperimentItemType = request.ExperimentItemType,
            PlannedHours = request.PlannedHours,
            ActualHours = request.ActualHours,
            ClassName = request.ClassName,
            ExpectedStudentCount = request.ExpectedStudentCount,
            ActualStudentCount = request.ActualStudentCount,
            AttendanceRecord = request.AttendanceRecord,
            TeachingCondition = request.TeachingCondition,
            EquipmentCondition = request.EquipmentCondition,
            Status = RegistrationStatus.Registered,
            FilledById = filledById,
            FilledByName = filledByName,
            FilledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _db.UsageRegistrations.Add(r);
        await _db.SaveChangesAsync();

        return MapToDto(r);
    }

    public async Task<List<UsageRegistrationDto>> GetPendingRegistrationsAsync(Guid? userId, Guid? semesterId)
    {
        var q = _db.UsageRegistrations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .Where(x => x.Status == RegistrationStatus.Pending);

        if (userId.HasValue)
            q = q.Where(x => x.FilledById == userId.Value);
        if (semesterId.HasValue)
            q = q.Where(x => x.SemesterId == semesterId.Value);

        var list = await q.OrderBy(x => x.UseDate).ThenBy(x => x.PeriodNumber).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<List<UsageRegistrationDto>> GetOverdueRegistrationsAsync(Guid? semesterId)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        var q = _db.UsageRegistrations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .Where(x => x.Status == RegistrationStatus.Pending
                && x.UseDate < today);

        if (semesterId.HasValue)
            q = q.Where(x => x.SemesterId == semesterId.Value);

        var list = await q.OrderBy(x => x.UseDate).ThenBy(x => x.PeriodNumber).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<int> CheckOverdueRegistrationsAsync()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        var overdue = await _db.UsageRegistrations
            .Where(x => x.Status == RegistrationStatus.Pending && x.UseDate < today)
            .ToListAsync();

        foreach (var r in overdue)
        {
            r.Status = RegistrationStatus.Overdue;
            r.UpdatedAt = now;
        }

        await _db.SaveChangesAsync();
        return overdue.Count;
    }

    private static UsageRegistrationDto MapToDto(UsageRegistration r)
    {
        return new UsageRegistrationDto
        {
            Id = r.Id,
            SemesterId = r.SemesterId,
            SemesterName = r.Semester?.Name,
            LabId = r.LabId,
            LabName = r.LabName,
            BuildingName = r.BuildingName ?? r.Lab?.Building?.Name,
            RoomNumber = r.RoomNumber ?? r.Lab?.RoomNumber,
            UseDate = r.UseDate,
            WeekNumber = r.WeekNumber,
            DayOfWeek = r.DayOfWeek,
            PeriodNumber = r.PeriodNumber,
            Source = r.Source.ToString(),
            ScheduleEntryId = r.ScheduleEntryId,
            ReservationId = r.ReservationId,
            TeachingApplicationId = r.TeachingApplicationId,
            CourseName = r.CourseName,
            ProjectName = r.ProjectName,
            ExperimentItemName = r.ExperimentItemName,
            ExperimentItemType = r.ExperimentItemType,
            PlannedHours = r.PlannedHours,
            ActualHours = r.ActualHours,
            ClassName = r.ClassName,
            ExpectedStudentCount = r.ExpectedStudentCount,
            ActualStudentCount = r.ActualStudentCount,
            AttendanceRecord = r.AttendanceRecord,
            TeachingCondition = r.TeachingCondition,
            EquipmentCondition = r.EquipmentCondition,
            Status = r.Status.ToString(),
            RemindedAt = r.RemindedAt,
            FilledById = r.FilledById,
            FilledByName = r.FilledByName,
            FilledAt = r.FilledAt,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy
        };
    }
}
