using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IReservationService
{
    Task<List<ReservationDto>> GetReservationsAsync(ReservationQuery query);
    Task<ReservationDto?> GetReservationByIdAsync(Guid id);
    Task<ReservationDto> CreateReservationAsync(CreateReservationRequest request, Guid applicantId, string applicantName, string applicantPhone, string? createdBy = null);
    Task<bool> ApproveReservationAsync(Guid id, ApprovalRequest request, Guid approverId, string? approverName = null);
    Task<bool> RejectReservationAsync(Guid id, ApprovalRequest request, Guid approverId);
    Task<bool> CancelReservationAsync(Guid id, CancelRequest request, Guid cancelledById);
    Task<List<ReservationDto>> GetPendingReservationsAsync(Guid? semesterId);
}

public class ReservationService : IReservationService
{
    private readonly AppDbContext _db;
    private readonly IScheduleService _scheduleService;

    public ReservationService(AppDbContext db, IScheduleService scheduleService)
    {
        _db = db;
        _scheduleService = scheduleService;
    }

    public async Task<List<ReservationDto>> GetReservationsAsync(ReservationQuery query)
    {
        var q = _db.Reservations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .AsQueryable();

        if (query.SemesterId.HasValue)
            q = q.Where(x => x.SemesterId == query.SemesterId.Value);
        if (query.LabId.HasValue)
            q = q.Where(x => x.LabId == query.LabId.Value);
        if (query.WeekNumber.HasValue)
            q = q.Where(x => x.WeekNumber == query.WeekNumber.Value);
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status.ToString() == query.Status);
        if (query.ApplicantId.HasValue)
            q = q.Where(x => x.ApplicantId == query.ApplicantId.Value);
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(x => x.ProjectName.Contains(query.Keyword) || x.ApplicantName.Contains(query.Keyword));

        var list = await q
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(Guid id)
    {
        var r = await _db.Reservations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .FirstOrDefaultAsync(x => x.Id == id);
        return r == null ? null : MapToDto(r);
    }

    public async Task<ReservationDto> CreateReservationAsync(
        CreateReservationRequest request,
        Guid applicantId,
        string applicantName,
        string applicantPhone,
        string? createdBy = null)
    {
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            SemesterId = request.SemesterId,
            LabId = request.LabId,
            UseDate = request.UseDate,
            DayOfWeek = request.DayOfWeek,
            PeriodNumbers = request.PeriodNumbers,
            WeekNumber = request.WeekNumber,
            ExpectedDurationHours = request.ExpectedDurationHours,
            ProjectName = request.ProjectName,
            ProjectCategory = request.ProjectCategory,
            Remark = request.Remark,
            ApplicantId = applicantId,
            ApplicantName = applicantName,
            ApplicantPhone = applicantPhone,
            ProjectLeaderId = request.ProjectLeaderId,
            ProjectLeaderName = request.ProjectLeaderName,
            ProjectLeaderPhone = request.ProjectLeaderPhone,
            MemberGrade = request.MemberGrade,
            MemberClassId = request.MemberClassId,
            MemberClassName = request.MemberClassName,
            MemberCount = request.MemberCount,
            Status = ApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return MapToDto(reservation);
    }

    public async Task<bool> ApproveReservationAsync(Guid id, ApprovalRequest request, Guid approverId, string? approverName = null)
    {
        var reservation = await _db.Reservations
            .Include(x => x.Lab)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (reservation == null) return false;
        if (reservation.Status != ApprovalStatus.Pending) return false;

        reservation.Status = ApprovalStatus.Approved;
        reservation.ApprovalComment = request.Comment;
        reservation.ApprovedBy = approverId;
        reservation.ApprovedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        foreach (var period in reservation.PeriodNumbers)
        {
            await _scheduleService.CreateCentralScheduleAsync(new CreateScheduleEntryRequest
            {
                SemesterId = reservation.SemesterId,
                LabId = reservation.LabId,
                WeekNumber = reservation.WeekNumber,
                DayOfWeek = reservation.DayOfWeek,
                PeriodNumber = period,
                Source = ScheduleSource.Reservation.ToString(),
                ReservationId = reservation.Id,
                ProjectName = reservation.ProjectName,
                Remark = $"来源：预约申请 | {reservation.Remark}"
            }, approverName);
        }

        return true;
    }

    public async Task<bool> RejectReservationAsync(Guid id, ApprovalRequest request, Guid approverId)
    {
        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null) return false;
        if (reservation.Status != ApprovalStatus.Pending) return false;

        reservation.Status = ApprovalStatus.Rejected;
        reservation.ApprovalComment = request.Comment;
        reservation.ApprovedBy = approverId;
        reservation.ApprovedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelReservationAsync(Guid id, CancelRequest request, Guid cancelledById)
    {
        var reservation = await _db.Reservations.FindAsync(id);
        if (reservation == null) return false;
        if (reservation.Status == ApprovalStatus.Rejected) return false;

        reservation.IsCancelled = true;
        reservation.CancelReason = request.Reason;
        reservation.CancelledBy = cancelledById;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReservationDto>> GetPendingReservationsAsync(Guid? semesterId)
    {
        var q = _db.Reservations
            .Include(x => x.Semester)
            .Include(x => x.Lab)
            .Where(x => x.Status == ApprovalStatus.Pending && !x.IsCancelled);

        if (semesterId.HasValue)
            q = q.Where(x => x.SemesterId == semesterId.Value);

        var list = await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    private static ReservationDto MapToDto(Reservation r)
    {
        return new ReservationDto
        {
            Id = r.Id,
            SemesterId = r.SemesterId,
            SemesterName = r.Semester?.Name,
            LabId = r.LabId,
            LabName = r.Lab?.Name,
            UseDate = r.UseDate,
            DayOfWeek = r.DayOfWeek,
            PeriodNumbers = r.PeriodNumbers,
            WeekNumber = r.WeekNumber,
            ExpectedDurationHours = r.ExpectedDurationHours,
            ProjectName = r.ProjectName,
            ProjectCategory = r.ProjectCategory,
            Remark = r.Remark,
            ApplicantId = r.ApplicantId,
            ApplicantName = r.ApplicantName,
            ApplicantPhone = r.ApplicantPhone,
            ProjectLeaderId = r.ProjectLeaderId,
            ProjectLeaderName = r.ProjectLeaderName,
            ProjectLeaderPhone = r.ProjectLeaderPhone,
            MemberGrade = r.MemberGrade,
            MemberClassId = r.MemberClassId,
            MemberClassName = r.MemberClassName,
            MemberCount = r.MemberCount,
            Status = r.Status.ToString(),
            ApprovalComment = r.ApprovalComment,
            ApprovedBy = r.ApprovedBy,
            ApproverName = null,
            ApprovedAt = r.ApprovedAt,
            IsCancelled = r.IsCancelled,
            CancelReason = r.CancelReason,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy
        };
    }
}
