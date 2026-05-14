using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IStatisticsService
{
    Task<List<ScheduleStatisticsDto>> GetWeeklyUsageSummaryAsync(ScheduleStatisticsQuery query);
    Task<List<LabOccupancy>> GetLabUsageCountAsync(StatisticsQuery query);
    Task<List<CategoryStat>> GetMajorUsageStatsAsync(StatisticsQuery query);
    Task<List<CategoryStat>> GetClassUsageStatsAsync(StatisticsQuery query);
    Task<List<CategoryStat>> GetGradeUsageStatsAsync(StatisticsQuery query);
    Task<List<CategoryStat>> GetCourseUsageStatsAsync(StatisticsQuery query);
    Task<List<CategoryStat>> GetReservationStatsAsync(StatisticsQuery query);
    Task<CompletionRate> GetRegistrationCompletionRateAsync(StatisticsQuery query);
    Task<DashboardData> GetDashboardDataAsync(DashboardQuery query);
}

public class StatisticsService : IStatisticsService
{
    private readonly AppDbContext _db;

    public StatisticsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ScheduleStatisticsDto>> GetWeeklyUsageSummaryAsync(ScheduleStatisticsQuery query)
    {
        var q = _db.ScheduleStatistics
            .Include(x => x.Semester)
            .Include(x => x.Building)
            .Include(x => x.Lab)
            .AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.WeekNumber.HasValue)
            q = q.Where(x => x.WeekNumber == query.WeekNumber.Value);
        if (query.BuildingId.HasValue)
            q = q.Where(x => x.BuildingId == query.BuildingId.Value);
        if (query.LabId.HasValue)
            q = q.Where(x => x.LabId == query.LabId.Value);

        var list = await q.OrderBy(x => x.WeekNumber).ToListAsync();
        return list.Select(MapStatDto).ToList();
    }

    public async Task<List<LabOccupancy>> GetLabUsageCountAsync(StatisticsQuery query)
    {
        var entries = await GetFilteredEntries(query).ToListAsync();

        var labGroups = entries
            .Where(x => x.LabId.HasValue)
            .GroupBy(x => x.LabId)
            .Select(g => new LabOccupancy
            {
                LabName = g.First().Lab?.Name ?? "未知实验室",
                UsedSlots = g.Count(),
                TotalSlots = 20 * 7 * 5,
                OccupancyRate = Math.Round(g.Count() * 100.0 / (20 * 7 * 5), 1)
            })
            .OrderByDescending(x => x.OccupancyRate)
            .ToList();

        return labGroups;
    }

    public async Task<List<CategoryStat>> GetMajorUsageStatsAsync(StatisticsQuery query)
    {
        var entries = await GetFilteredEntries(query)
            .Where(x => !string.IsNullOrEmpty(x.MajorName))
            .ToListAsync();

        var total = entries.Count;
        var groups = entries
            .GroupBy(x => x.MajorName ?? "未知")
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return groups;
    }

    public async Task<List<CategoryStat>> GetClassUsageStatsAsync(StatisticsQuery query)
    {
        var entries = await GetFilteredEntries(query)
            .Where(x => !string.IsNullOrEmpty(x.ClassName))
            .ToListAsync();

        var total = entries.Count;
        var groups = entries
            .GroupBy(x => x.ClassName ?? "未知")
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return groups;
    }

    public async Task<List<CategoryStat>> GetGradeUsageStatsAsync(StatisticsQuery query)
    {
        var entries = await GetFilteredEntries(query)
            .Where(x => !string.IsNullOrEmpty(x.ClassName))
            .ToListAsync();

        var total = entries.Count;
        var groups = entries
            .GroupBy(x => ExtractGrade(x.ClassName ?? ""))
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return groups;
    }

    public async Task<List<CategoryStat>> GetCourseUsageStatsAsync(StatisticsQuery query)
    {
        var entries = await GetFilteredEntries(query)
            .Where(x => !string.IsNullOrEmpty(x.CourseName))
            .ToListAsync();

        var total = entries.Count;
        var groups = entries
            .GroupBy(x => x.CourseName ?? "未知")
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = total > 0 ? Math.Round(g.Count() * 100.0 / total, 1) : 0
            })
            .OrderByDescending(x => x.Count)
            .Take(20)
            .ToList();

        return groups;
    }

    public async Task<List<CategoryStat>> GetReservationStatsAsync(StatisticsQuery query)
    {
        var reservations = _db.Reservations.AsQueryable();

        if (query.SemesterId.HasValue)
            reservations = reservations.Where(x => x.SemesterId == query.SemesterId.Value);

        var list = await reservations.ToListAsync();
        var total = list.Count;

        return new List<CategoryStat>
        {
            new() { Category = "全部预约", Count = total, Percentage = 100 },
            new() { Category = "待审批", Count = list.Count(x => x.Status == ApprovalStatus.Pending), Percentage = total > 0 ? Math.Round(list.Count(x => x.Status == ApprovalStatus.Pending) * 100.0 / total, 1) : 0 },
            new() { Category = "已通过", Count = list.Count(x => x.Status == ApprovalStatus.Approved), Percentage = total > 0 ? Math.Round(list.Count(x => x.Status == ApprovalStatus.Approved) * 100.0 / total, 1) : 0 },
            new() { Category = "已驳回", Count = list.Count(x => x.Status == ApprovalStatus.Rejected), Percentage = total > 0 ? Math.Round(list.Count(x => x.Status == ApprovalStatus.Rejected) * 100.0 / total, 1) : 0 },
            new() { Category = "已取消", Count = list.Count(x => x.IsCancelled), Percentage = total > 0 ? Math.Round(list.Count(x => x.IsCancelled) * 100.0 / total, 1) : 0 }
        };
    }

    public async Task<CompletionRate> GetRegistrationCompletionRateAsync(StatisticsQuery query)
    {
        var q = _db.UsageRegistrations.AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);

        var list = await q.ToListAsync();
        var total = list.Count;

        return new CompletionRate
        {
            Total = total,
            Completed = list.Count(x => x.Status == RegistrationStatus.Registered),
            Pending = list.Count(x => x.Status == RegistrationStatus.Pending),
            Overdue = list.Count(x => x.Status == RegistrationStatus.Overdue),
            Rate = total > 0 ? Math.Round(list.Count(x => x.Status == RegistrationStatus.Registered) * 100.0 / total, 1) : 0
        };
    }

    public async Task<DashboardData> GetDashboardDataAsync(DashboardQuery query)
    {
        var today = DateTime.UtcNow.Date;
        var todayDayOfWeek = (int)today.DayOfWeek;
        if (todayDayOfWeek == 0) todayDayOfWeek = 7;

        var semesterId = query.SemesterId;
        var weekNumber = query.WeekNumber ?? 1;

        if (!semesterId.HasValue)
        {
            var current = await _db.Semesters.FirstOrDefaultAsync(x => x.IsCurrent);
            if (current != null)
            {
                semesterId = current.Id;
                var daysSinceStart = (today - current.TeachingStartDate!.Value.Date).Days;
                weekNumber = Math.Max(1, Math.Min(current.TeachingWeeks, daysSinceStart / 7 + 1));
            }
        }

        var allLabs = await _db.Labs.Where(x => x.IsActive).ToListAsync();
        var todayEntries = new List<ScheduleEntry>();
        var weekEntries = new List<ScheduleEntry>();

        if (semesterId.HasValue)
        {
            todayEntries = await _db.ScheduleEntries
                .Where(x => x.SemesterId == semesterId.Value
                    && x.WeekNumber == weekNumber
                    && x.DayOfWeek == todayDayOfWeek
                    && x.Status == "Active")
                .Include(x => x.Lab)
                .ToListAsync();

            weekEntries = await _db.ScheduleEntries
                .Where(x => x.SemesterId == semesterId.Value
                    && x.WeekNumber == weekNumber
                    && x.Status == "Active")
                .Include(x => x.Lab)
                .ToListAsync();
        }

        var usedLabIds = todayEntries.Where(x => x.LabId.HasValue).Select(x => x.LabId!.Value).Distinct().ToHashSet();
        var pendingRes = await _db.Reservations.CountAsync(x => x.Status == ApprovalStatus.Pending && !x.IsCancelled);
        var pendingReg = await _db.UsageRegistrations.CountAsync(x => x.Status == RegistrationStatus.Pending);

        var result = new DashboardData
        {
            Today = new TodaySummary
            {
                TotalLabs = allLabs.Count,
                OccupiedLabs = usedLabIds.Count,
                AvailableLabs = allLabs.Count - usedLabIds.Count,
                OccupancyRate = allLabs.Count > 0 ? Math.Round(usedLabIds.Count * 100.0 / allLabs.Count, 1) : 0,
                TotalSchedules = todayEntries.Count,
                PendingReservations = pendingRes,
                PendingRegistrations = pendingReg
            },
            Week = new WeekSummary
            {
                TotalSlots = allLabs.Count * 7 * 5,
                UsedSlots = weekEntries.Count,
                OccupancyRate = allLabs.Count > 0 ? Math.Round(weekEntries.Count * 100.0 / (allLabs.Count * 7 * 5), 1) : 0,
                TotalStudentCount = weekEntries.Where(x => x.StudentCount.HasValue).Sum(x => x.StudentCount!.Value),
                TotalReservations = await _db.Reservations.CountAsync(x => x.SemesterId == semesterId && x.WeekNumber == weekNumber),
                ApprovedReservations = await _db.Reservations.CountAsync(x => x.SemesterId == semesterId && x.WeekNumber == weekNumber && x.Status == ApprovalStatus.Approved),
                TotalTeachingApplications = (await _db.TeachingApplications.Where(x => x.SemesterId == semesterId).ToListAsync()).Count(x => x.WeekNumbers.Contains(weekNumber)),
                ApprovedTeachingApplications = (await _db.TeachingApplications.Where(x => x.SemesterId == semesterId && x.Status == ApprovalStatus.Approved).ToListAsync()).Count(x => x.WeekNumbers.Contains(weekNumber))
            },
            LabOccupancyList = await GetLabUsageCountAsync(new StatisticsQuery { SemesterId = semesterId, WeekNumber = weekNumber }),
            CompletionRate = await GetRegistrationCompletionRateAsync(new StatisticsQuery { SemesterId = semesterId }),
            Alerts = new List<AlertItem>()
        };

        var overdueRegs = await _db.UsageRegistrations
            .Where(x => x.Status == RegistrationStatus.Overdue)
            .OrderByDescending(x => x.UseDate)
            .Take(10)
            .ToListAsync();

        foreach (var reg in overdueRegs)
        {
            result.Alerts.Add(new AlertItem
            {
                Type = "Overdue",
                Message = $"[{reg.LabName}] {reg.CourseName ?? reg.ProjectName} 逾期未登记（{reg.UseDate:yyyy-MM-dd}）",
                Time = reg.UseDate,
                RelatedId = reg.Id.ToString()
            });
        }

        var pendingApps = await _db.TeachingApplications
            .Where(x => x.Status == ApprovalStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .ToListAsync();

        foreach (var app in pendingApps)
        {
            result.Alerts.Add(new AlertItem
            {
                Type = "PendingApplication",
                Message = $"授课申请待审批：[{app.CourseName}] {app.ApplicantName}",
                Time = app.CreatedAt,
                RelatedId = app.Id.ToString()
            });
        }

        return result;
    }

    private IQueryable<ScheduleEntry> GetFilteredEntries(StatisticsQuery query)
    {
        var q = _db.ScheduleEntries
            .Include(x => x.Lab)
            .Include(x => x.Semester)
            .Where(x => x.Status == "Active");

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.WeekNumber.HasValue)
            q = q.Where(x => x.WeekNumber == query.WeekNumber.Value);
        if (query.StartWeek.HasValue && query.EndWeek.HasValue)
            q = q.Where(x => x.WeekNumber >= query.StartWeek.Value && x.WeekNumber <= query.EndWeek.Value);
        if (query.LabId.HasValue)
            q = q.Where(x => x.LabId == query.LabId.Value);
        if (query.MajorId.HasValue)
            q = q.Where(x => x.MajorId == query.MajorId.Value);
        if (query.ClassId.HasValue)
            q = q.Where(x => x.ClassId == query.ClassId.Value);
        if (query.CourseId.HasValue)
            q = q.Where(x => x.CourseId == query.CourseId.Value);

        return q;
    }

    private static string ExtractGrade(string className)
    {
        var match = System.Text.RegularExpressions.Regex.Match(className, @"(\d{4})");
        return match.Success ? $"{match.Groups[1].Value}级" : "未知年级";
    }

    private static ScheduleStatisticsDto MapStatDto(ScheduleStatistics s)
    {
        return new ScheduleStatisticsDto
        {
            Id = s.Id,
            SemesterId = s.SemesterId,
            SemesterName = s.Semester?.Name,
            BuildingId = s.BuildingId,
            BuildingName = s.Building?.Name,
            LabId = s.LabId,
            LabName = s.Lab?.Name,
            WeekNumber = s.WeekNumber,
            TotalSlots = s.TotalSlots,
            UsedSlots = s.UsedSlots,
            ReservationSlots = s.ReservationSlots,
            OccupancyRate = s.OccupancyRate,
            TotalStudentCount = s.TotalStudentCount,
            GeneratedAt = s.GeneratedAt
        };
    }
}
