using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IPeriodTimeService
{
    Task<List<PeriodTime>> GetListAsync();
    Task<PeriodTime?> GetByIdAsync(Guid id);
    Task<PeriodTime> CreateAsync(CreatePeriodTimeRequest request);
    Task<PeriodTime?> UpdateAsync(Guid id, UpdatePeriodTimeRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
}

public class PeriodTimeService : IPeriodTimeService
{
    private readonly AppDbContext _dbContext;

    public PeriodTimeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PeriodTime>> GetListAsync()
    {
        return await _dbContext.PeriodTimes
            .OrderBy(p => p.PeriodNumber)
            .ToListAsync();
    }

    public async Task<PeriodTime?> GetByIdAsync(Guid id)
    {
        return await _dbContext.PeriodTimes.FindAsync(id);
    }

    public async Task<PeriodTime> CreateAsync(CreatePeriodTimeRequest request)
    {
        // 解析时间字符串
        var startTime = TimeSpan.Parse(request.StartTime);
        var endTime = TimeSpan.Parse(request.EndTime);

        var periodTime = new PeriodTime
        {
            PeriodNumber = request.PeriodNumber,
            Name = request.Name,
            StartTime = startTime,
            EndTime = endTime,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.PeriodTimes.Add(periodTime);
        await _dbContext.SaveChangesAsync();
        return periodTime;
    }

    public async Task<PeriodTime?> UpdateAsync(Guid id, UpdatePeriodTimeRequest request)
    {
        var periodTime = await _dbContext.PeriodTimes.FindAsync(id);
        if (periodTime == null) return null;

        if (request.PeriodNumber.HasValue) periodTime.PeriodNumber = request.PeriodNumber.Value;
        if (request.Name != null) periodTime.Name = request.Name;
        if (request.StartTime != null) periodTime.StartTime = TimeSpan.Parse(request.StartTime);
        if (request.EndTime != null) periodTime.EndTime = TimeSpan.Parse(request.EndTime);
        if (request.Description != null) periodTime.Description = request.Description;
        if (request.IsActive.HasValue) periodTime.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        return periodTime;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var periodTime = await _dbContext.PeriodTimes.FindAsync(id);
        if (periodTime == null) return false;

        _dbContext.PeriodTimes.Remove(periodTime);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var periodTime = await _dbContext.PeriodTimes.FindAsync(id);
        if (periodTime == null) return false;

        periodTime.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

public class CreatePeriodTimeRequest
{
    public int PeriodNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty; // Format: "HH:mm"
    public string EndTime { get; set; } = string.Empty; // Format: "HH:mm"
    public string? Description { get; set; }
}

public class UpdatePeriodTimeRequest
{
    public int? PeriodNumber { get; set; }
    public string? Name { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
