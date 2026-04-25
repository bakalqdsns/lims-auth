using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using System.Text.Json;

namespace LimsAuth.Api.Services;

public interface ISemesterService
{
    // 基础 CRUD
    Task<List<Semester>> GetListAsync(string? keyword = null, SemesterStatus? status = null, SemesterType? type = null, bool? isCurrent = null);
    Task<Semester?> GetByIdAsync(Guid id);
    Task<Semester?> GetCurrentAsync();
    Task<Semester> CreateAsync(CreateSemesterRequest request, Guid? createdBy = null);
    Task<Semester?> UpdateAsync(Guid id, UpdateSemesterRequest request, Guid? updatedBy = null);
    Task<bool> DeleteAsync(Guid id);
    
    // 状态管理
    Task<bool> SetCurrentAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, SemesterStatus status, Guid? updatedBy = null);
    Task<bool> ArchiveAsync(Guid id, Guid? archivedBy = null);
    
    // 业务规则校验
    Task<SemesterValidationResult> ValidateAsync(CreateSemesterRequest request, Guid? excludeId = null);
    Task<bool> CheckTimeOverlapAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null);
    
    // 校历生成
    Task<List<AcademicCalendar>> GenerateCalendarAsync(Guid semesterId, GenerateCalendarRequest? request = null);
    Task<List<AcademicCalendar>> GetCalendarAsync(Guid semesterId);
    Task<AcademicCalendar?> UpdateCalendarDayAsync(Guid calendarId, UpdateCalendarDayRequest request);
    
    // 数据复制
    Task<Semester> CopyFromTemplateAsync(Guid templateId, CopySemesterRequest request, Guid? createdBy = null);
    Task<Semester> CopyFromSemesterAsync(Guid sourceSemesterId, CopySemesterRequest request, Guid? createdBy = null);
    
    // 沙箱管理
    Task<Semester> CreateSandboxAsync(Guid sourceSemesterId, string sandboxName, Guid? createdBy = null);
    Task<bool> ApplySandboxAsync(Guid sandboxId, Guid? updatedBy = null);
    Task<bool> DiscardSandboxAsync(Guid sandboxId);
    
    // 自动状态流转
    Task AutoTransitionStatusAsync();
    
    // 日志
    Task<List<SemesterLog>> GetLogsAsync(Guid semesterId);
}

public class SemesterService : ISemesterService
{
    private readonly AppDbContext _dbContext;

    public SemesterService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region 基础 CRUD

    public async Task<List<Semester>> GetListAsync(string? keyword = null, SemesterStatus? status = null, SemesterType? type = null, bool? isCurrent = null)
    {
        var query = _dbContext.Semesters
            .Include(s => s.ChildSemesters)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.Name.Contains(keyword) || 
                                     (s.Code != null && s.Code.Contains(keyword)));
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(s => s.SemesterType == type.Value);
        }

        if (isCurrent.HasValue)
        {
            query = query.Where(s => s.IsCurrent == isCurrent.Value);
        }

        return await query.OrderByDescending(s => s.StartDate).ToListAsync();
    }

    public async Task<Semester?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Semesters
            .Include(s => s.ChildSemesters)
            .Include(s => s.ParentSemester)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Semester?> GetCurrentAsync()
    {
        return await _dbContext.Semesters
            .FirstOrDefaultAsync(s => s.IsCurrent && s.IsActive);
    }

    public async Task<Semester> CreateAsync(CreateSemesterRequest request, Guid? createdBy = null)
    {
        // 校验时间重叠
        var overlap = await CheckTimeOverlapAsync(request.StartDate, request.EndDate);
        if (overlap)
        {
            throw new InvalidOperationException("学期时间段与已有学期重叠");
        }

        var semester = new Semester
        {
            Name = request.Name,
            Code = request.Code,
            AcademicYear = request.AcademicYear,
            SemesterType = request.SemesterType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TeachingStartDate = request.TeachingStartDate,
            TeachingEndDate = request.TeachingEndDate,
            TotalWeeks = request.TotalWeeks,
            TeachingWeeks = request.TeachingWeeks,
            CourseSelectionStart = request.CourseSelectionStart,
            CourseSelectionEnd = request.CourseSelectionEnd,
            CourseSelectionEndWithdraw = request.CourseSelectionEndWithdraw,
            SchedulingStart = request.SchedulingStart,
            SchedulingEnd = request.SchedulingEnd,
            SchedulePublishTime = request.SchedulePublishTime,
            ExamWeekStart = request.ExamWeekStart,
            ExamWeekEnd = request.ExamWeekEnd,
            GradeEntryStart = request.GradeEntryStart,
            GradeEntryEnd = request.GradeEntryEnd,
            GradePublishTime = request.GradePublishTime,
            RegistrationStart = request.RegistrationStart,
            RegistrationEnd = request.RegistrationEnd,
            TuitionPaymentStart = request.TuitionPaymentStart,
            TuitionPaymentEnd = request.TuitionPaymentEnd,
            Status = SemesterStatus.NotStarted,
            IsCurrent = false,
            IsActive = true,
            IsEditable = true,
            IsDeletable = true,
            Description = request.Description,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _dbContext.Semesters.Add(semester);
        await _dbContext.SaveChangesAsync();

        // 记录日志
        await AddLogAsync(semester.Id, "CREATE", "创建学期", null, JsonSerializer.Serialize(request), createdBy);

        return semester;
    }

    public async Task<Semester?> UpdateAsync(Guid id, UpdateSemesterRequest request, Guid? updatedBy = null)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return null;

        // 检查是否可编辑
        if (!semester.IsEditable)
        {
            throw new InvalidOperationException("该学期已锁定，不可编辑");
        }

        var oldValues = JsonSerializer.Serialize(semester);

        // 更新时间时检查重叠
        if (request.StartDate.HasValue || request.EndDate.HasValue)
        {
            var newStart = request.StartDate ?? semester.StartDate;
            var newEnd = request.EndDate ?? semester.EndDate;
            var overlap = await CheckTimeOverlapAsync(newStart, newEnd, id);
            if (overlap)
            {
                throw new InvalidOperationException("学期时间段与已有学期重叠");
            }
        }

        if (request.Name != null) semester.Name = request.Name;
        if (request.Code != null) semester.Code = request.Code;
        if (request.AcademicYear != null) semester.AcademicYear = request.AcademicYear;
        if (request.StartDate.HasValue) semester.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) semester.EndDate = request.EndDate.Value;
        if (request.TeachingStartDate.HasValue) semester.TeachingStartDate = request.TeachingStartDate.Value;
        if (request.TeachingEndDate.HasValue) semester.TeachingEndDate = request.TeachingEndDate.Value;
        if (request.TotalWeeks.HasValue) semester.TotalWeeks = request.TotalWeeks.Value;
        if (request.TeachingWeeks.HasValue) semester.TeachingWeeks = request.TeachingWeeks.Value;
        if (request.CourseSelectionStart.HasValue) semester.CourseSelectionStart = request.CourseSelectionStart.Value;
        if (request.CourseSelectionEnd.HasValue) semester.CourseSelectionEnd = request.CourseSelectionEnd.Value;
        if (request.SchedulingStart.HasValue) semester.SchedulingStart = request.SchedulingStart.Value;
        if (request.SchedulingEnd.HasValue) semester.SchedulingEnd = request.SchedulingEnd.Value;
        if (request.ExamWeekStart.HasValue) semester.ExamWeekStart = request.ExamWeekStart.Value;
        if (request.ExamWeekEnd.HasValue) semester.ExamWeekEnd = request.ExamWeekEnd.Value;
        if (request.GradeEntryStart.HasValue) semester.GradeEntryStart = request.GradeEntryStart.Value;
        if (request.GradeEntryEnd.HasValue) semester.GradeEntryEnd = request.GradeEntryEnd.Value;
        if (request.Description != null) semester.Description = request.Description;
        
        semester.UpdatedAt = DateTime.UtcNow;
        semester.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();

        var newValues = JsonSerializer.Serialize(semester);
        await AddLogAsync(semester.Id, "UPDATE", "更新学期", oldValues, newValues, updatedBy);

        return semester;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        if (!semester.IsDeletable)
        {
            throw new InvalidOperationException("该学期不可删除");
        }

        if (semester.IsCurrent)
        {
            throw new InvalidOperationException("当前学期不可删除，请先切换当前学期");
        }

        _dbContext.Semesters.Remove(semester);
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(id, "DELETE", "删除学期", null, null, null);

        return true;
    }

    #endregion

    #region 状态管理

    public async Task<bool> SetCurrentAsync(Guid id)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        if (!semester.IsActive)
        {
            throw new InvalidOperationException("未启用的学期不能设为当前学期");
        }

        // 取消其他学期的当前状态
        var currentSemesters = await _dbContext.Semesters.Where(s => s.IsCurrent).ToListAsync();
        foreach (var s in currentSemesters)
        {
            s.IsCurrent = false;
        }

        semester.IsCurrent = true;
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(id, "SET_CURRENT", "设为当前学期", null, null, null);

        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, SemesterStatus status, Guid? updatedBy = null)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        var oldStatus = semester.Status;
        semester.Status = status;
        semester.UpdatedAt = DateTime.UtcNow;
        semester.UpdatedBy = updatedBy;

        // 状态变更时的业务逻辑
        switch (status)
        {
            case SemesterStatus.InProgress:
                semester.IsEditable = false;  // 进行中时锁定编辑
                break;
            case SemesterStatus.Ended:
                semester.IsEditable = false;
                semester.IsDeletable = false;
                break;
            case SemesterStatus.Archived:
                semester.IsEditable = false;
                semester.IsDeletable = false;
                if (semester.IsCurrent)
                {
                    semester.IsCurrent = false;
                }
                break;
        }

        await _dbContext.SaveChangesAsync();

        await AddLogAsync(id, "STATUS_CHANGE", $"状态变更: {oldStatus} -> {status}", 
            oldStatus.ToString(), status.ToString(), updatedBy);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id, Guid? archivedBy = null)
    {
        var semester = await _dbContext.Semesters.FindAsync(id);
        if (semester == null) return false;

        semester.Status = SemesterStatus.Archived;
        semester.IsEditable = false;
        semester.IsDeletable = false;
        semester.IsCurrent = false;
        semester.ArchivedAt = DateTime.UtcNow;
        semester.ArchivedBy = archivedBy;
        semester.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        await AddLogAsync(id, "ARCHIVE", "归档学期", null, null, archivedBy);

        return true;
    }

    #endregion

    #region 业务规则校验

    public async Task<SemesterValidationResult> ValidateAsync(CreateSemesterRequest request, Guid? excludeId = null)
    {
        var result = new SemesterValidationResult { IsValid = true };

        // 时间范围校验
        if (request.EndDate <= request.StartDate)
        {
            result.IsValid = false;
            result.Errors.Add("结束日期必须晚于开始日期");
        }

        // 重叠校验
        var overlap = await CheckTimeOverlapAsync(request.StartDate, request.EndDate, excludeId);
        if (overlap)
        {
            result.IsValid = false;
            result.Errors.Add("学期时间段与已有学期重叠");
        }

        // 关键节点时间校验
        if (request.CourseSelectionEnd.HasValue && request.CourseSelectionStart.HasValue)
        {
            if (request.CourseSelectionEnd.Value <= request.CourseSelectionStart.Value)
            {
                result.Warnings.Add("选课结束时间应晚于开始时间");
            }
        }

        if (request.GradeEntryEnd.HasValue && request.GradeEntryStart.HasValue)
        {
            if (request.GradeEntryEnd.Value <= request.GradeEntryStart.Value)
            {
                result.Warnings.Add("成绩录入结束时间应晚于开始时间");
            }
        }

        return result;
    }

    public async Task<bool> CheckTimeOverlapAsync(DateTime startDate, DateTime endDate, Guid? excludeId = null)
    {
        var query = _dbContext.Semesters.Where(s => 
            (s.StartDate <= endDate && s.EndDate >= startDate));

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    #endregion

    #region 校历生成

    public async Task<List<AcademicCalendar>> GenerateCalendarAsync(Guid semesterId, GenerateCalendarRequest? request = null)
    {
        var semester = await _dbContext.Semesters.FindAsync(semesterId);
        if (semester == null) throw new Exception("学期不存在");

        // 删除旧校历
        var oldCalendars = await _dbContext.AcademicCalendars
            .Where(c => c.SemesterId == semesterId)
            .ToListAsync();
        _dbContext.AcademicCalendars.RemoveRange(oldCalendars);

        var calendars = new List<AcademicCalendar>();
        var currentDate = semester.StartDate.Date;
        var endDate = semester.EndDate.Date;
        int weekNumber = 1;

        // 获取节假日配置（如果有）
        var holidays = request?.Holidays ?? new List<HolidayConfig>();
        // 获取特殊事件配置（如果有）
        var specialEvents = request?.SpecialEvents ?? new List<SpecialEventConfig>();

        while (currentDate <= endDate)
        {
            var dayOfWeek = (int)currentDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // 周日作为第7天

            // 检查是否为节假日
            var holiday = holidays.FirstOrDefault(h => h.Date.Date == currentDate);
            var isHoliday = holiday != null;

            // 检查是否为特殊事件日
            var specialEvent = specialEvents.FirstOrDefault(e => e.Date.Date == currentDate);

            // 确定事件类型
            var eventType = specialEvent?.EventType ?? CalendarEventType.Teaching;
            if (isHoliday)
            {
                eventType = CalendarEventType.Holiday;
            }
            else if (semester.ExamWeekStart.HasValue && semester.ExamWeekEnd.HasValue)
            {
                if (currentDate >= semester.ExamWeekStart.Value.Date && 
                    currentDate <= semester.ExamWeekEnd.Value.Date)
                {
                    eventType = CalendarEventType.Exam;
                }
            }

            // 确定是否为工作日
            var isWorkday = !isHoliday || (holiday?.IsWorkday ?? false);
            
            // 确定是否为教学日
            var isTeachingDay = isWorkday && dayOfWeek <= 5 && 
                !(semester.ExamWeekStart.HasValue && currentDate >= semester.ExamWeekStart.Value.Date &&
                  semester.ExamWeekEnd.HasValue && currentDate <= semester.ExamWeekEnd.Value.Date);

            var calendar = new AcademicCalendar
            {
                SemesterId = semesterId,
                Date = currentDate,
                WeekNumber = weekNumber,
                DayOfWeek = dayOfWeek,
                EventType = eventType,
                EventName = specialEvent?.EventName ?? holiday?.Name,
                EventPriority = specialEvent?.Priority ?? CalendarEventPriority.Normal,
                IsHoliday = isHoliday,
                IsWorkday = isWorkday,
                IsTeachingDay = isTeachingDay,
                IsExamDay = eventType == CalendarEventType.Exam,
                HolidayName = holiday?.Name,
                HolidayType = holiday?.Type,
                Description = specialEvent?.Description ?? holiday?.Description,
                Color = specialEvent?.Color,
                AffectsCourseSelection = specialEvent?.AffectsCourseSelection ?? false,
                AffectsScheduling = specialEvent?.AffectsScheduling ?? false,
                AffectsGradeEntry = specialEvent?.AffectsGradeEntry ?? false,
                AffectsRegistration = specialEvent?.AffectsRegistration ?? false,
                AutoTriggerAction = specialEvent?.AutoTriggerAction
            };

            calendars.Add(calendar);

            if (dayOfWeek == 7)
            {
                weekNumber++;
            }

            currentDate = currentDate.AddDays(1);
        }

        _dbContext.AcademicCalendars.AddRange(calendars);
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(semesterId, "GENERATE_CALENDAR", "生成校历", null, $"生成了 {calendars.Count} 天", null);

        return calendars;
    }

    public async Task<List<AcademicCalendar>> GetCalendarAsync(Guid semesterId)
    {
        return await _dbContext.AcademicCalendars
            .Where(c => c.SemesterId == semesterId)
            .OrderBy(c => c.Date)
            .ToListAsync();
    }

    public async Task<AcademicCalendar?> UpdateCalendarDayAsync(Guid calendarId, UpdateCalendarDayRequest request)
    {
        var calendar = await _dbContext.AcademicCalendars.FindAsync(calendarId);
        if (calendar == null) return null;

        if (request.IsHoliday.HasValue) calendar.IsHoliday = request.IsHoliday.Value;
        if (request.HolidayName != null) calendar.HolidayName = request.HolidayName;
        if (request.IsWorkday.HasValue) calendar.IsWorkday = request.IsWorkday.Value;
        if (request.IsTeachingDay.HasValue) calendar.IsTeachingDay = request.IsTeachingDay.Value;
        if (request.EventType.HasValue) calendar.EventType = request.EventType.Value;
        if (request.Description != null) calendar.Description = request.Description;
        if (request.Color != null) calendar.Color = request.Color;

        calendar.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return calendar;
    }

    #endregion

    #region 数据复制

    public async Task<Semester> CopyFromTemplateAsync(Guid templateId, CopySemesterRequest request, Guid? createdBy = null)
    {
        var template = await _dbContext.CalendarTemplates.FindAsync(templateId);
        if (template == null) throw new Exception("模板不存在");

        // 解析模板数据（TemplateData 可能为空）
        var templateData = string.IsNullOrWhiteSpace(template.TemplateData)
            ? null
            : JsonSerializer.Deserialize<TemplateData>(template.TemplateData);

        var semester = new Semester
        {
            Name = request.Name,
            Code = request.Code,
            AcademicYear = request.AcademicYear,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalWeeks = templateData?.TotalWeeks ?? 20,
            TeachingWeeks = templateData?.TeachingWeeks ?? 18,
            Status = SemesterStatus.NotStarted,
            IsTemplate = false,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _dbContext.Semesters.Add(semester);
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(semester.Id, "COPY_FROM_TEMPLATE", $"从模板 {template.Name} 复制", null, null, createdBy);

        return semester;
    }

    public async Task<Semester> CopyFromSemesterAsync(Guid sourceSemesterId, CopySemesterRequest request, Guid? createdBy = null)
    {
        var source = await _dbContext.Semesters.FindAsync(sourceSemesterId);
        if (source == null) throw new Exception("源学期不存在");

        var semester = new Semester
        {
            Name = request.Name,
            Code = request.Code,
            AcademicYear = request.AcademicYear,
            SemesterType = source.SemesterType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalWeeks = source.TotalWeeks,
            TeachingWeeks = source.TeachingWeeks,
            SourceSemesterId = sourceSemesterId,
            CopiedAt = DateTime.UtcNow,
            CopySettings = JsonSerializer.Serialize(request.CopyOptions),
            Status = SemesterStatus.NotStarted,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _dbContext.Semesters.Add(semester);
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(semester.Id, "COPY_FROM_SEMESTER", $"从学期 {source.Name} 复制", null, null, createdBy);

        return semester;
    }

    #endregion

    #region 沙箱管理

    public async Task<Semester> CreateSandboxAsync(Guid sourceSemesterId, string sandboxName, Guid? createdBy = null)
    {
        var source = await _dbContext.Semesters.FindAsync(sourceSemesterId);
        if (source == null) throw new Exception("源学期不存在");

        var sandbox = new Semester
        {
            Name = $"[沙箱] {sandboxName}",
            Code = $"SANDBOX-{DateTime.Now:yyyyMMdd-HHmmss}",
            AcademicYear = source.AcademicYear,
            SemesterType = source.SemesterType,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            TotalWeeks = source.TotalWeeks,
            TeachingWeeks = source.TeachingWeeks,
            ParentSemesterId = sourceSemesterId,
            IsSandbox = true,
            Status = SemesterStatus.NotStarted,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _dbContext.Semesters.Add(sandbox);
        await _dbContext.SaveChangesAsync();

        await AddLogAsync(sandbox.Id, "CREATE_SANDBOX", $"创建沙箱，来源: {source.Name}", null, null, createdBy);

        return sandbox;
    }

    public async Task<bool> ApplySandboxAsync(Guid sandboxId, Guid? updatedBy = null)
    {
        var sandbox = await _dbContext.Semesters.FindAsync(sandboxId);
        if (sandbox == null || !sandbox.IsSandbox) return false;

        if (sandbox.ParentSemesterId == null) return false;

        // 这里可以实现将沙箱数据应用到父学期的逻辑
        // 例如复制排课结果等

        sandbox.IsSandbox = false;  // 标记为已应用
        sandbox.UpdatedAt = DateTime.UtcNow;
        sandbox.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync();

        await AddLogAsync(sandboxId, "APPLY_SANDBOX", "应用沙箱到正式环境", null, null, updatedBy);

        return true;
    }

    public async Task<bool> DiscardSandboxAsync(Guid sandboxId)
    {
        var sandbox = await _dbContext.Semesters.FindAsync(sandboxId);
        if (sandbox == null || !sandbox.IsSandbox) return false;

        _dbContext.Semesters.Remove(sandbox);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region 自动状态流转

    public async Task AutoTransitionStatusAsync()
    {
        var today = DateTime.UtcNow.Date;
        var semesters = await _dbContext.Semesters
            .Where(s => s.IsActive && s.Status != SemesterStatus.Archived)
            .ToListAsync();

        foreach (var semester in semesters)
        {
            var oldStatus = semester.Status;
            var newStatus = oldStatus;

            // 根据日期自动判断状态
            if (today < semester.StartDate.Date)
            {
                newStatus = SemesterStatus.NotStarted;
            }
            else if (today >= semester.StartDate.Date && today <= semester.EndDate.Date)
            {
                newStatus = SemesterStatus.InProgress;
            }
            else if (today > semester.EndDate.Date)
            {
                newStatus = SemesterStatus.Ended;
            }

            if (newStatus != oldStatus)
            {
                semester.Status = newStatus;
                semester.UpdatedAt = DateTime.UtcNow;

                await AddLogAsync(semester.Id, "AUTO_STATUS_CHANGE", 
                    $"自动状态流转: {oldStatus} -> {newStatus}", 
                    oldStatus.ToString(), newStatus.ToString(), null);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region 日志

    private async Task AddLogAsync(Guid semesterId, string action, string description, 
        string? oldValues = null, string? newValues = null, Guid? performedBy = null)
    {
        var log = new SemesterLog
        {
            SemesterId = semesterId,
            Action = action,
            ActionDescription = description,
            OldValues = oldValues,
            NewValues = newValues,
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };

        _dbContext.SemesterLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SemesterLog>> GetLogsAsync(Guid semesterId)
    {
        return await _dbContext.SemesterLogs
            .Where(l => l.SemesterId == semesterId)
            .OrderByDescending(l => l.PerformedAt)
            .ToListAsync();
    }

    #endregion
}

#region DTOs

public class SemesterValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class CreateSemesterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? AcademicYear { get; set; }
    public SemesterType SemesterType { get; set; } = SemesterType.Regular;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? TeachingStartDate { get; set; }
    public DateTime? TeachingEndDate { get; set; }
    public int TotalWeeks { get; set; }
    public int TeachingWeeks { get; set; }
    
    // 选课时间
    public DateTime? CourseSelectionStart { get; set; }
    public DateTime? CourseSelectionEnd { get; set; }
    public DateTime? CourseSelectionEndWithdraw { get; set; }
    
    // 排课时间
    public DateTime? SchedulingStart { get; set; }
    public DateTime? SchedulingEnd { get; set; }
    public DateTime? SchedulePublishTime { get; set; }
    
    // 考试时间
    public DateTime? ExamWeekStart { get; set; }
    public DateTime? ExamWeekEnd { get; set; }
    public DateTime? GradeEntryStart { get; set; }
    public DateTime? GradeEntryEnd { get; set; }
    public DateTime? GradePublishTime { get; set; }
    
    // 注册缴费
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public DateTime? TuitionPaymentStart { get; set; }
    public DateTime? TuitionPaymentEnd { get; set; }
    
    public string? Description { get; set; }
}

public class UpdateSemesterRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? AcademicYear { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TeachingStartDate { get; set; }
    public DateTime? TeachingEndDate { get; set; }
    public int? TotalWeeks { get; set; }
    public int? TeachingWeeks { get; set; }
    public DateTime? CourseSelectionStart { get; set; }
    public DateTime? CourseSelectionEnd { get; set; }
    public DateTime? SchedulingStart { get; set; }
    public DateTime? SchedulingEnd { get; set; }
    public DateTime? ExamWeekStart { get; set; }
    public DateTime? ExamWeekEnd { get; set; }
    public DateTime? GradeEntryStart { get; set; }
    public DateTime? GradeEntryEnd { get; set; }
    public string? Description { get; set; }
}

public class GenerateCalendarRequest
{
    public List<HolidayConfig>? Holidays { get; set; }
    public List<SpecialEventConfig>? SpecialEvents { get; set; }
}

public class SpecialEventConfig
{
    public DateTime Date { get; set; }
    public CalendarEventType EventType { get; set; } = CalendarEventType.Custom;
    public string? EventName { get; set; }
    public CalendarEventPriority Priority { get; set; } = CalendarEventPriority.Normal;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool AffectsCourseSelection { get; set; } = false;
    public bool AffectsScheduling { get; set; } = false;
    public bool AffectsGradeEntry { get; set; } = false;
    public bool AffectsRegistration { get; set; } = false;
    public string? AutoTriggerAction { get; set; }
}

public class HolidayConfig
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public bool IsWorkday { get; set; } = false;
    public string? Description { get; set; }
}

public class UpdateCalendarDayRequest
{
    public bool? IsHoliday { get; set; }
    public string? HolidayName { get; set; }
    public bool? IsWorkday { get; set; }
    public bool? IsTeachingDay { get; set; }
    public CalendarEventType? EventType { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class CopySemesterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CopyOptions? CopyOptions { get; set; }
}

public class CopyOptions
{
    public bool CopyCourses { get; set; } = true;
    public bool CopyClasses { get; set; } = true;
    public bool CopyTeachingTasks { get; set; } = false;
    public bool CopyPeriodTimes { get; set; } = true;
}

public class TemplateData
{
    public int TotalWeeks { get; set; }
    public int TeachingWeeks { get; set; }
    public int ExamWeeks { get; set; }
    public List<WeekSchedule>? Schedule { get; set; }
}

public class WeekSchedule
{
    public int Week { get; set; }
    public string Type { get; set; } = string.Empty;
}

#endregion
