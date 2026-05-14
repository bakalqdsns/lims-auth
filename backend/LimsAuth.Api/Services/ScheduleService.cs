using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IScheduleService
{
    Task<List<ScheduleEntryDto>> GetScheduleEntriesAsync(ScheduleQuery query);
    Task<ScheduleEntryDto?> GetScheduleByIdAsync(Guid id);
    Task<ScheduleEntryDto> CreateCentralScheduleAsync(CreateScheduleEntryRequest request, string? createdBy = null);
    Task<bool> UpdateCentralScheduleAsync(Guid id, UpdateScheduleEntryRequest request, string? updatedBy = null);
    Task<bool> DeleteCentralScheduleAsync(Guid id, string? deletedBy = null);
    Task<ConflictCheckResult> CheckConflictsAsync(ScheduleEntry entry);
    Task<List<ScheduleTableRow>> GetScheduleTableAsync(ScheduleQuery query);
    Task<List<Lab>> GetAvailableLabsAsync(AvailabilityQuery query);
}

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;

    public ScheduleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ScheduleEntryDto>> GetScheduleEntriesAsync(ScheduleQuery query)
    {
        var q = _db.ScheduleEntries
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.WeekNumber.HasValue)
            q = q.Where(x => x.WeekNumber == query.WeekNumber.Value);
        if (query.DayOfWeek.HasValue)
            q = q.Where(x => x.DayOfWeek == query.DayOfWeek.Value);
        if (query.LabId.HasValue)
            q = q.Where(x => x.LabId == query.LabId.Value);
        if (query.BuildingId.HasValue)
            q = q.Where(x => x.Lab != null && x.Lab.BuildingId == query.BuildingId.Value);
        if (query.ClassId.HasValue)
            q = q.Where(x => x.ClassId == query.ClassId.Value);
        if (query.TeacherId.HasValue)
            q = q.Where(x => x.TeacherId == query.TeacherId.Value);
        if (query.CourseId.HasValue)
            q = q.Where(x => x.CourseId == query.CourseId.Value);
        if (!string.IsNullOrEmpty(query.Source))
            q = q.Where(x => x.Source.ToString() == query.Source);
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status == query.Status);

        var list = await q
            .OrderBy(x => x.SemesterId)
            .ThenBy(x => x.WeekNumber)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.PeriodNumber)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    public async Task<ScheduleEntryDto?> GetScheduleByIdAsync(Guid id)
    {
        var entry = await _db.ScheduleEntries
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .FirstOrDefaultAsync(x => x.Id == id);
        return entry == null ? null : MapToDto(entry);
    }

    public async Task<ScheduleEntryDto> CreateCentralScheduleAsync(CreateScheduleEntryRequest request, string? createdBy = null)
    {
        var entry = new ScheduleEntry
        {
            Id = Guid.NewGuid(),
            SemesterId = request.SemesterId,
            LabId = request.LabId,
            WeekNumber = request.WeekNumber,
            DayOfWeek = request.DayOfWeek,
            PeriodNumber = request.PeriodNumber,
            Source = Enum.TryParse<ScheduleSource>(request.Source, true, out var s) ? s : ScheduleSource.CentralScheduling,
            Status = "Active",
            ReservationId = request.ReservationId,
            TeachingApplicationId = request.TeachingApplicationId,
            ExperimentTaskId = request.ExperimentTaskId,
            TeachingTaskId = request.TeachingTaskId,
            CourseName = request.CourseName,
            ProjectName = request.ProjectName,
            CourseId = request.CourseId,
            TeacherId = request.TeacherId,
            TeacherName = request.TeacherName,
            ClassId = request.ClassId,
            ClassName = request.ClassName,
            MajorId = request.MajorId,
            MajorName = request.MajorName,
            StudentCount = request.StudentCount,
            Remark = request.Remark,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        var conflict = await CheckConflictsAsync(entry);
        entry.HasConflict = conflict.HasSoftConflict;
        entry.ConflictInfo = conflict.HasSoftConflict
            ? string.Join(";", conflict.SoftConflicts.Select(c => c.Message))
            : null;

        _db.ScheduleEntries.Add(entry);
        await _db.SaveChangesAsync();

        return MapToDto(entry);
    }

    public async Task<bool> UpdateCentralScheduleAsync(Guid id, UpdateScheduleEntryRequest request, string? updatedBy = null)
    {
        var entry = await _db.ScheduleEntries.FindAsync(id);
        if (entry == null) return false;

        if (request.LabId.HasValue)
            entry.LabId = request.LabId.Value;
        if (request.WeekNumber.HasValue)
            entry.WeekNumber = request.WeekNumber.Value;
        if (request.DayOfWeek.HasValue)
            entry.DayOfWeek = request.DayOfWeek.Value;
        if (request.PeriodNumber.HasValue)
            entry.PeriodNumber = request.PeriodNumber.Value;
        if (!string.IsNullOrEmpty(request.Status))
            entry.Status = request.Status;
        if (request.TeacherId.HasValue)
        {
            entry.TeacherId = request.TeacherId.Value;
            entry.TeacherName = request.TeacherName;
        }
        entry.Remark = request.Remark ?? entry.Remark;
        entry.UpdatedAt = DateTime.UtcNow;
        entry.UpdatedBy = updatedBy;

        var conflict = await CheckConflictsAsync(entry);
        entry.HasConflict = conflict.HasSoftConflict;
        entry.ConflictInfo = conflict.HasSoftConflict
            ? string.Join(";", conflict.SoftConflicts.Select(c => c.Message))
            : null;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCentralScheduleAsync(Guid id, string? deletedBy = null)
    {
        var entry = await _db.ScheduleEntries.FindAsync(id);
        if (entry == null) return false;

        entry.Status = "Cancelled";
        entry.Source = ScheduleSource.Cancelled;
        entry.UpdatedAt = DateTime.UtcNow;
        entry.UpdatedBy = deletedBy;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ConflictCheckResult> CheckConflictsAsync(ScheduleEntry entry)
    {
        var result = new ConflictCheckResult();

        var hardConflicts = await _db.ScheduleEntries
            .Include(x => x.Lab)
            .Where(x => x.SemesterId == entry.SemesterId
                && x.LabId == entry.LabId
                && x.WeekNumber == entry.WeekNumber
                && x.DayOfWeek == entry.DayOfWeek
                && x.PeriodNumber == entry.PeriodNumber
                && x.Status == "Active"
                && x.Id != entry.Id)
            .ToListAsync();

        foreach (var c in hardConflicts)
        {
            result.HardConflicts.Add(new ConflictItem
            {
                Id = c.Id,
                Type = "HardConflict",
                Message = $"实验室 [{c.Lab?.Name}] 在第{c.WeekNumber}周 星期{c.DayOfWeek} 第{c.PeriodNumber}节 已被 [{c.CourseName ?? c.ProjectName}] 占用",
                LabName = c.Lab?.Name,
                WeekNumber = c.WeekNumber,
                DayOfWeek = c.DayOfWeek,
                PeriodNumber = c.PeriodNumber
            });
        }

        result.HasHardConflict = hardConflicts.Any();
        result.HasSoftConflict = false;

        if (result.HasHardConflict)
            return result;

        var teacherConflicts = await _db.ScheduleEntries
            .Where(x => x.SemesterId == entry.SemesterId
                && x.TeacherId == entry.TeacherId
                && x.WeekNumber == entry.WeekNumber
                && x.DayOfWeek == entry.DayOfWeek
                && x.PeriodNumber == entry.PeriodNumber
                && x.Status == "Active"
                && x.Id != entry.Id)
            .ToListAsync();

        foreach (var c in teacherConflicts)
        {
            result.SoftConflicts.Add(new ConflictItem
            {
                Id = c.Id,
                Type = "TeacherConflict",
                Message = $"教师 [{entry.TeacherName}] 在第{c.WeekNumber}周 星期{c.DayOfWeek} 第{c.PeriodNumber}节 已有 [{c.CourseName ?? c.ProjectName}] 的排课",
                WeekNumber = c.WeekNumber,
                DayOfWeek = c.DayOfWeek,
                PeriodNumber = c.PeriodNumber
            });
        }

        var classConflicts = await _db.ScheduleEntries
            .Where(x => x.SemesterId == entry.SemesterId
                && x.ClassId == entry.ClassId
                && x.WeekNumber == entry.WeekNumber
                && x.DayOfWeek == entry.DayOfWeek
                && x.PeriodNumber == entry.PeriodNumber
                && x.Status == "Active"
                && x.Id != entry.Id)
            .ToListAsync();

        foreach (var c in classConflicts)
        {
            result.SoftConflicts.Add(new ConflictItem
            {
                Id = c.Id,
                Type = "ClassConflict",
                Message = $"班级 [{entry.ClassName}] 在第{c.WeekNumber}周 星期{c.DayOfWeek} 第{c.PeriodNumber}节 已有 [{c.CourseName ?? c.ProjectName}] 的排课",
                WeekNumber = c.WeekNumber,
                DayOfWeek = c.DayOfWeek,
                PeriodNumber = c.PeriodNumber
            });
        }

        result.HasSoftConflict = result.SoftConflicts.Any();
        return result;
    }

    public async Task<List<ScheduleTableRow>> GetScheduleTableAsync(ScheduleQuery query)
    {
        var entries = await _db.ScheduleEntries
            .Include(x => x.Lab)
                .ThenInclude(l => l!.Building)
            .Include(x => x.Semester)
            .Where(x => x.SemesterId == query.SemesterId
                && x.Status == "Active")
            .ToListAsync();

        if (query.WeekNumber.HasValue)
            entries = entries.Where(x => x.WeekNumber == query.WeekNumber.Value).ToList();
        if (query.LabId.HasValue)
            entries = entries.Where(x => x.LabId == query.LabId.Value).ToList();
        if (query.BuildingId.HasValue)
            entries = entries.Where(x => x.Lab != null && x.Lab.BuildingId == query.BuildingId.Value).ToList();

        var dayOfWeekList = new[] { 1, 2, 3, 4, 5, 6, 7 };
        if (query.DayOfWeek.HasValue)
            dayOfWeekList = new[] { query.DayOfWeek.Value };

        var periodNumbers = await _db.PeriodTimes
            .Where(x => x.IsActive)
            .OrderBy(x => x.PeriodNumber)
            .Select(x => x.PeriodNumber)
            .ToListAsync();

        var rows = new List<ScheduleTableRow>();

        foreach (var period in periodNumbers)
        {
            var row = new ScheduleTableRow
            {
                PeriodNumber = period,
                PeriodName = $"第{period}节",
                Cells = new List<ScheduleTableCell?>()
            };

            foreach (var dow in dayOfWeekList)
            {
                var entry = entries
                    .FirstOrDefault(x => x.PeriodNumber == period && x.DayOfWeek == dow);

                if (entry == null)
                {
                    row.Cells.Add(null);
                }
                else
                {
                    row.Cells.Add(new ScheduleTableCell
                    {
                        ScheduleEntryId = entry.Id,
                        CourseName = entry.CourseName,
                        TeacherName = entry.TeacherName,
                        ClassName = entry.ClassName,
                        LabName = entry.Lab?.Name,
                        Source = entry.Source.ToString(),
                        Status = entry.Status,
                        HasConflict = entry.HasConflict,
                        StudentCount = entry.StudentCount ?? 0
                    });
                }
            }
            rows.Add(row);
        }

        return rows;
    }

    public async Task<List<Lab>> GetAvailableLabsAsync(AvailabilityQuery query)
    {
        var occupiedLabIds = await _db.ScheduleEntries
            .Where(x => x.SemesterId == query.SemesterId
                && x.WeekNumber == query.WeekNumber
                && x.DayOfWeek == query.DayOfWeek
                && query.PeriodNumbers.Contains(x.PeriodNumber)
                && x.Status == "Active")
            .Select(x => x.LabId)
            .Distinct()
            .ToListAsync();

        var q = _db.Labs
            .Include(x => x.Building)
                .ThenInclude(x => x!.Campus)
            .Where(x => x.IsActive);

        if (query.BuildingId.HasValue)
            q = q.Where(x => x.BuildingId == query.BuildingId.Value);

        return await q
            .Where(x => !occupiedLabIds.Contains(x.Id))
            .OrderBy(x => x.Building!.Name)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    private static ScheduleEntryDto MapToDto(ScheduleEntry e)
    {
        return new ScheduleEntryDto
        {
            Id = e.Id,
            SemesterId = e.SemesterId,
            SemesterName = e.Semester?.Name,
            LabId = e.LabId,
            LabName = e.Lab?.Name,
            WeekNumber = e.WeekNumber,
            DayOfWeek = e.DayOfWeek,
            PeriodNumber = e.PeriodNumber,
            Source = e.Source.ToString(),
            Status = e.Status,
            ReservationId = e.ReservationId,
            TeachingApplicationId = e.TeachingApplicationId,
            ExperimentTaskId = e.ExperimentTaskId,
            TeachingTaskId = e.TeachingTaskId,
            CourseName = e.CourseName,
            ProjectName = e.ProjectName,
            CourseId = e.CourseId,
            TeacherId = e.TeacherId,
            TeacherName = e.TeacherName,
            ClassId = e.ClassId,
            ClassName = e.ClassName,
            MajorId = e.MajorId,
            MajorName = e.MajorName,
            StudentCount = e.StudentCount,
            Remark = e.Remark,
            HasConflict = e.HasConflict,
            ConflictInfo = e.ConflictInfo,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy
        };
    }
}
