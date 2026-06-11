using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

// Schedule
public interface IScheduleService
{
    Task<ApiResponse<PagedResponse<ScheduleDto>>> GetListAsync(ScheduleQueryRequest query);
    Task<ApiResponse<ScheduleDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ScheduleDto>> CreateAsync(CreateScheduleRequest request);
    Task<ApiResponse<ScheduleDto>> UpdateAsync(Guid id, UpdateScheduleRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<DashboardDataDto>> GetDashboardAsync();
}

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;
    public ScheduleService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<ScheduleDto>>> GetListAsync(ScheduleQueryRequest query)
    {
        var q = _db.LabSchedules
            .Include(s => s.Semester).Include(s => s.Room).Include(s => s.Room!.Building)
            .Include(s => s.Class).Include(s => s.Teacher).Include(s => s.ExperimentItem)
            .Where(s => s.IsDeleted == 0).AsQueryable();
        if (query.SemesterId.HasValue) q = q.Where(s => s.SemesterId == query.SemesterId);
        if (query.WeekNo.HasValue) q = q.Where(s => s.WeekNo == query.WeekNo);
        if (query.RoomId.HasValue) q = q.Where(s => s.RoomId == query.RoomId);
        if (query.ClassId.HasValue) q = q.Where(s => s.ClassId == query.ClassId);
        if (query.TeacherId.HasValue) q = q.Where(s => s.TeacherId == query.TeacherId);
        if (query.Status.HasValue) q = q.Where(s => s.Status == query.Status.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(s => s.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(s => new ScheduleDto
            {
                Id = s.Id, SemesterId = s.SemesterId, SemesterName = s.Semester != null ? s.Semester.Name : null,
                RoomId = s.RoomId, RoomName = s.Room != null ? s.Room.Name : null,
                BuildingName = s.Room != null && s.Room.Building != null ? s.Room.Building.Name : null,
                CourseName = s.CourseName, ClassId = s.ClassId, ClassName = s.Class != null ? s.Class.Name : null,
                TeacherId = s.TeacherId, TeacherName = s.Teacher != null ? s.Teacher.RealName : null,
                ExperimentItemId = s.ExperimentItemId, ExperimentItemName = s.ExperimentItem != null ? s.ExperimentItem.Name : null,
                WeekNo = s.WeekNo, DayOfWeek = s.DayOfWeek, SectionNo = s.SectionNo,
                ScheduleType = s.ScheduleType, Status = s.Status, Remark = s.Remark,
                CreatedAt = s.CreatedAt, CreatedBy = s.CreatedBy
            }).ToListAsync();
        return ApiResponse<PagedResponse<ScheduleDto>>.Success(new PagedResponse<ScheduleDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<ScheduleDto>> GetByIdAsync(Guid id)
    {
        var s = await _db.LabSchedules.Include(x => x.Semester).Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return ApiResponse<ScheduleDto>.Error(404, "排课记录不存在");
        return ApiResponse<ScheduleDto>.Success(new ScheduleDto { Id = s.Id, SemesterId = s.SemesterId, WeekNo = s.WeekNo, Status = s.Status });
    }

    public async Task<ApiResponse<ScheduleDto>> CreateAsync(CreateScheduleRequest request)
    {
        var s = new LabSchedule
        {
            Id = Guid.NewGuid(), SemesterId = request.SemesterId, RoomId = request.RoomId,
            TeachingTaskId = request.TeachingTaskId, WeekNo = request.WeekNo, DayOfWeek = request.DayOfWeek,
            SectionNo = request.SectionNo, CourseName = request.CourseName,
            ClassId = request.ClassId, TeacherId = request.TeacherId,
            ExperimentItemId = request.ExperimentItemId, ScheduleType = request.ScheduleType,
            Status = request.Status, Remark = request.Remark,
            CreatedAt = DateTime.UtcNow, CreatedBy = request.TeacherId
        };
        _db.LabSchedules.Add(s);
        await _db.SaveChangesAsync();
        return ApiResponse<ScheduleDto>.Success(new ScheduleDto { Id = s.Id }, "排课创建成功");
    }

    public async Task<ApiResponse<ScheduleDto>> UpdateAsync(Guid id, UpdateScheduleRequest request)
    {
        var s = await _db.LabSchedules.FindAsync(id);
        if (s == null) return ApiResponse<ScheduleDto>.Error(404, "排课记录不存在");
        if (request.RoomId.HasValue) s.RoomId = request.RoomId;
        if (request.Status.HasValue) s.Status = request.Status.Value;
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<ScheduleDto>.Success(new ScheduleDto { Id = s.Id }, "排课更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var s = await _db.LabSchedules.FindAsync(id);
        if (s == null) return ApiResponse.Error(404, "排课记录不存在");
        s.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("排课删除成功");
    }

    public async Task<ApiResponse<DashboardDataDto>> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var totalRooms = await _db.LabRooms.Where(r => r.IsDeleted == 0 && r.Status == 1).CountAsync();
        var pendingBookings = await _db.LabBookingApplies.Where(b => b.AuditStatus == "Pending" && b.IsDeleted == 0).CountAsync();
        var todaySchedules = await _db.LabSchedules.Where(s => s.IsDeleted == 0 && s.Status == 1).CountAsync();

        return ApiResponse<DashboardDataDto>.Success(new DashboardDataDto
        {
            TotalRooms = totalRooms,
            AvailableRooms = totalRooms,
            PendingBookings = pendingBookings,
            TodaySchedules = todaySchedules,
            TodayOccupancyRate = 0,
            WeekOccupancyRate = 0,
            Alerts = new List<AlertItemDto>()
        });
    }
}

// ExperimentItem
public interface IExperimentItemService
{
    Task<ApiResponse<PagedResponse<ExperimentItemDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<ExperimentItemDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ExperimentItemDto>> CreateAsync(CreateExperimentItemRequest request);
    Task<ApiResponse<ExperimentItemDto>> UpdateAsync(Guid id, UpdateExperimentItemRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class ExperimentItemService : IExperimentItemService
{
    private readonly AppDbContext _db;
    public ExperimentItemService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<ExperimentItemDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.LabExperimentItems.Include(e => e.Course).Where(e => e.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(e => e.Name.Contains(query.Keyword) || e.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(e => e.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(e => new ExperimentItemDto
            {
                Id = e.Id, Code = e.Code, Name = e.Name,
                CourseId = e.CourseId, CourseName = e.Course != null ? e.Course.Name : null,
                ExperimentType = e.ExperimentType, StandardHours = e.StandardHours, Status = e.Status, CreatedAt = e.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<ExperimentItemDto>>.Success(new PagedResponse<ExperimentItemDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<ExperimentItemDto>> GetByIdAsync(Guid id)
    {
        var e = await _db.LabExperimentItems.Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return ApiResponse<ExperimentItemDto>.Error(404, "实验项目不存在");
        return ApiResponse<ExperimentItemDto>.Success(new ExperimentItemDto { Id = e.Id, Code = e.Code, Name = e.Name, Status = e.Status });
    }

    public async Task<ApiResponse<ExperimentItemDto>> CreateAsync(CreateExperimentItemRequest request)
    {
        var e = new LabExperimentItem
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, CourseId = request.CourseId,
            ExperimentType = request.ExperimentType, StandardHours = request.StandardHours,
            Status = request.Status, CreatedAt = DateTime.UtcNow
        };
        _db.LabExperimentItems.Add(e);
        await _db.SaveChangesAsync();
        return ApiResponse<ExperimentItemDto>.Success(new ExperimentItemDto { Id = e.Id, Code = e.Code, Name = e.Name }, "实验项目创建成功");
    }

    public async Task<ApiResponse<ExperimentItemDto>> UpdateAsync(Guid id, UpdateExperimentItemRequest request)
    {
        var e = await _db.LabExperimentItems.FindAsync(id);
        if (e == null) return ApiResponse<ExperimentItemDto>.Error(404, "实验项目不存在");
        if (!string.IsNullOrEmpty(request.Name)) e.Name = request.Name;
        if (request.Status.HasValue) e.Status = request.Status.Value;
        await _db.SaveChangesAsync();
        return ApiResponse<ExperimentItemDto>.Success(new ExperimentItemDto { Id = e.Id, Code = e.Code, Name = e.Name }, "实验项目更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var e = await _db.LabExperimentItems.FindAsync(id);
        if (e == null) return ApiResponse.Error(404, "实验项目不存在");
        e.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("实验项目删除成功");
    }
}

// BookingApply
public interface IBookingApplyService
{
    Task<ApiResponse<PagedResponse<BookingApplyDto>>> GetListAsync(BookingApplyQueryRequest query);
    Task<ApiResponse<BookingApplyDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<BookingApplyDto>> CreateAsync(CreateBookingApplyRequest request);
    Task<ApiResponse> AuditAsync(Guid id, AuditBookingRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class BookingApplyService : IBookingApplyService
{
    private readonly AppDbContext _db;
    public BookingApplyService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<BookingApplyDto>>> GetListAsync(BookingApplyQueryRequest query)
    {
        var q = _db.LabBookingApplies.Include(b => b.Applicant).Include(b => b.Room).Include(b => b.Auditor)
            .Where(b => b.IsDeleted == 0).AsQueryable();
        if (query.RoomId.HasValue) q = q.Where(b => b.RoomId == query.RoomId);
        if (query.ApplicantId.HasValue) q = q.Where(b => b.ApplicantId == query.ApplicantId);
        if (!string.IsNullOrEmpty(query.AuditStatus)) q = q.Where(b => b.AuditStatus == query.AuditStatus);
        if (query.StartDate.HasValue) q = q.Where(b => b.TargetDate >= query.StartDate);
        if (query.EndDate.HasValue) q = q.Where(b => b.TargetDate <= query.EndDate);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(b => b.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(b => new BookingApplyDto
            {
                Id = b.Id, Code = b.Code, ApplicantId = b.ApplicantId, ApplicantName = b.Applicant != null ? b.Applicant.RealName : null,
                ApplicantType = b.ApplicantType, RoomId = b.RoomId, RoomName = b.Room != null ? b.Room.Name : null,
                Purpose = b.Purpose, TargetDate = b.TargetDate, TargetSection = b.TargetSection,
                WeekNo = b.WeekNo, EstimatedPeople = b.EstimatedPeople,
                AuditStatus = b.AuditStatus, AuditOpinion = b.AuditOpinion,
                AuditorId = b.AuditorId, AuditorName = b.Auditor != null ? b.Auditor.RealName : null,
                AuditTime = b.AuditTime, CreatedAt = b.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<BookingApplyDto>>.Success(new PagedResponse<BookingApplyDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<BookingApplyDto>> GetByIdAsync(Guid id)
    {
        var b = await _db.LabBookingApplies.Include(x => x.Applicant).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return ApiResponse<BookingApplyDto>.Error(404, "预约申请不存在");
        return ApiResponse<BookingApplyDto>.Success(new BookingApplyDto { Id = b.Id, Code = b.Code, AuditStatus = b.AuditStatus });
    }

    public async Task<ApiResponse<BookingApplyDto>> CreateAsync(CreateBookingApplyRequest request)
    {
        var b = new LabBookingApply
        {
            Id = Guid.NewGuid(), Code = request.Code, ApplicantId = request.ApplicantId,
            ApplicantType = request.ApplicantType, RoomId = request.RoomId,
            Purpose = request.Purpose, TargetDate = request.TargetDate,
            TargetSection = request.TargetSection, WeekNo = request.WeekNo,
            EstimatedPeople = request.EstimatedPeople,
            CreatedAt = DateTime.UtcNow
        };
        _db.LabBookingApplies.Add(b);
        await _db.SaveChangesAsync();
        return ApiResponse<BookingApplyDto>.Success(new BookingApplyDto { Id = b.Id, Code = b.Code }, "预约申请创建成功");
    }

    public async Task<ApiResponse> AuditAsync(Guid id, AuditBookingRequest request)
    {
        var b = await _db.LabBookingApplies.FindAsync(id);
        if (b == null) return ApiResponse.Error(404, "预约申请不存在");
        b.AuditStatus = request.Approved ? "Approved" : "Rejected";
        b.AuditOpinion = request.Opinion;
        await _db.SaveChangesAsync();
        return ApiResponse.Success(request.Approved ? "审批通过" : "审批驳回");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var b = await _db.LabBookingApplies.FindAsync(id);
        if (b == null) return ApiResponse.Error(404, "预约申请不存在");
        b.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("预约申请删除成功");
    }
}

// UsageRegister
public interface IUsageRegisterService
{
    Task<ApiResponse<PagedResponse<UsageRegisterDto>>> GetListAsync(UsageRegisterQueryRequest query);
    Task<ApiResponse<UsageRegisterDto>> CreateAsync(CreateUsageRegisterRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class UsageRegisterService : IUsageRegisterService
{
    private readonly AppDbContext _db;
    public UsageRegisterService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<UsageRegisterDto>>> GetListAsync(UsageRegisterQueryRequest query)
    {
        var q = _db.LabUsageRegisters
            .Include(u => u.Semester).Include(u => u.Room).Include(u => u.RegisterUser)
            .Where(u => u.IsDeleted == 0).AsQueryable();
        if (query.SemesterId.HasValue) q = q.Where(u => u.SemesterId == query.SemesterId);
        if (query.RoomId.HasValue) q = q.Where(u => u.RoomId == query.RoomId);
        if (!string.IsNullOrEmpty(query.RegisterStatus)) q = q.Where(u => u.RegisterStatus == query.RegisterStatus);
        if (query.StartDate.HasValue) q = q.Where(u => u.RegisterTime >= query.StartDate);
        if (query.EndDate.HasValue) q = q.Where(u => u.RegisterTime <= query.EndDate);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(u => u.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(u => new UsageRegisterDto
            {
                Id = u.Id, ScheduleId = u.ScheduleId, BookingApplyId = u.BookingApplyId,
                SemesterId = u.SemesterId, SemesterName = u.Semester != null ? u.Semester.Name : null,
                RoomId = u.RoomId, RoomName = u.Room != null ? u.Room.Name : null,
                ItemName = u.ItemName, ExperimentType = u.ExperimentType,
                PlannedHours = u.PlannedHours, ActualHours = u.ActualHours,
                ClassName = u.ClassName, ExpectedCount = u.ExpectedCount, ActualCount = u.ActualCount,
                AttendanceRecord = u.AttendanceRecord, TeachingRecord = u.TeachingRecord, DeviceRecord = u.DeviceRecord,
                RegisterStatus = u.RegisterStatus, RegisterUserId = u.RegisterUserId,
                RegisterUserName = u.RegisterUser != null ? u.RegisterUser.RealName : null,
                RegisterTime = u.RegisterTime, CreatedAt = u.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<UsageRegisterDto>>.Success(new PagedResponse<UsageRegisterDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<UsageRegisterDto>> CreateAsync(CreateUsageRegisterRequest request)
    {
        var u = new LabUsageRegister
        {
            Id = Guid.NewGuid(), ScheduleId = request.ScheduleId, BookingApplyId = request.BookingApplyId,
            SemesterId = request.SemesterId, RoomId = request.RoomId, ItemName = request.ItemName,
            ExperimentType = request.ExperimentType, PlannedHours = request.PlannedHours,
            ActualHours = request.ActualHours, ClassName = request.ClassName,
            ExpectedCount = request.ExpectedCount, ActualCount = request.ActualCount,
            AttendanceRecord = request.AttendanceRecord, TeachingRecord = request.TeachingRecord,
            DeviceRecord = request.DeviceRecord, RegisterStatus = request.RegisterStatus,
            RegisterUserId = request.ScheduleId, RegisterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _db.LabUsageRegisters.Add(u);
        await _db.SaveChangesAsync();
        return ApiResponse<UsageRegisterDto>.Success(new UsageRegisterDto { Id = u.Id }, "使用登记创建成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var u = await _db.LabUsageRegisters.FindAsync(id);
        if (u == null) return ApiResponse.Error(404, "使用登记不存在");
        u.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("使用登记删除成功");
    }
}
