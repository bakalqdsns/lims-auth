using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IEquipmentBorrowService
{
    Task<EquipmentBorrowRecord> CreateBorrowRequestAsync(CreateBorrowRequestDto request, Guid applicantId);
    Task<EquipmentBorrowRecord?> GetByIdAsync(Guid id);
    Task<EquipmentBorrowRecord?> GetByRecordNoAsync(string recordNo);
    Task<List<BorrowRecordDto>> GetMyRecordsAsync(Guid userId, string? status = null);
    Task<List<BorrowRecordDto>> GetPendingTeacherApprovalsAsync(bool isAdmin);
    Task<List<BorrowRecordDto>> GetAllRecordsAsync(string? status = null, string? keyword = null);
    Task<(bool Success, string Message)> DeleteAsync(Guid recordId);
    Task<(bool Success, string Message)> SupervisorApproveAsync(Guid recordId, Guid approverId, bool approved, string? remark);
    Task<(bool Success, string Message)> AdminApproveAsync(Guid recordId, Guid approverId, bool approved, string? remark);
    Task<(bool Success, string Message)> ApproveReturnAsync(Guid recordId, Guid checkerId, bool approved, string condition, string? remarks);
    Task<(bool Success, string Message)> ApproveRenewAsync(Guid recordId, Guid approverId, bool approved, string? remark);
    Task<(bool Success, string Message)> ConfirmBorrowAsync(Guid recordId, Guid operatorId);
    Task<(bool Success, string Message)> SubmitReturnAsync(Guid recordId, Guid userId);
    Task<(bool Success, string Message)> ConfirmReturnAsync(Guid recordId, Guid checkerId, string condition, string? remarks);
    Task<(bool Success, string Message)> RenewAsync(Guid recordId, Guid applicantId, DateTime newReturnDate);
    Task<List<OverdueRecordDto>> GetOverdueRecordsAsync();
    Task<List<ExpiringRecordDto>> GetExpiringRecordsAsync(int daysBefore = 1);
    Task<List<BorrowFlowDto>> GetBorrowFlowAsync(DateTime? startDate = null, DateTime? endDate = null);
}

public class EquipmentBorrowService : IEquipmentBorrowService
{
    private readonly AppDbContext _dbContext;

    public EquipmentBorrowService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static string GenerateRecordNo()
    {
        return $"BR{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }

    public async Task<EquipmentBorrowRecord> CreateBorrowRequestAsync(CreateBorrowRequestDto request, Guid applicantId)
    {
        var equipment = await _dbContext.Equipments.FindAsync(request.EquipmentId);
        if (equipment == null)
            throw new InvalidOperationException("设备不存在");

        if (!equipment.IsActive)
            throw new InvalidOperationException("设备已停用");

        if (equipment.Status == "在库-待维修" || equipment.Status == "报废" || equipment.Status == "丢失" || equipment.Status == "借出")
            throw new InvalidOperationException($"设备当前状态为「{equipment.Status}」，不可借用");

        if (request.ExpectedReturnDate <= request.BorrowDate)
            throw new InvalidOperationException("计划归还时间必须晚于借出时间");

        var applicant = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == applicantId);
        var isStudent = applicant?.UserRoles.Any(ur => ur.Role?.Code == "student") ?? false;

        var record = new EquipmentBorrowRecord
        {
            RecordNo = GenerateRecordNo(),
            EquipmentId = request.EquipmentId,
            ApplicantId = applicantId,
            Status = isStudent ? BorrowStatus.PendingTeacherApproval : BorrowStatus.PendingAdminApproval,
            BorrowDate = request.BorrowDate,
            ExpectedReturnDate = request.ExpectedReturnDate,
            Purpose = request.Purpose,
            Phone = request.Phone,
            UsageLocation = request.UsageLocation,
            Remarks = request.Remarks
        };

        _dbContext.EquipmentBorrowRecords.Add(record);
        equipment.Status = "在库-已预约";

        await _dbContext.SaveChangesAsync();
        return record;
    }

    public async Task<EquipmentBorrowRecord?> GetByIdAsync(Guid id)
    {
        return await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Include(r => r.Approver)
            .Include(r => r.ReturnChecker)
            .Include(r => r.Recipient)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<EquipmentBorrowRecord?> GetByRecordNoAsync(string recordNo)
    {
        return await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Include(r => r.Approver)
            .Include(r => r.ReturnChecker)
            .Include(r => r.Recipient)
            .FirstOrDefaultAsync(r => r.RecordNo == recordNo && !r.IsDeleted);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(Guid recordId)
    {
        var record = await _dbContext.EquipmentBorrowRecords.FindAsync(recordId);
        if (record == null) return (false, "借还记录不存在");
        if (record.IsDeleted) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.Returned)
            return (false, $"仅允许删除已归还的记录，当前状态为「{record.Status}」");

        record.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return (true, "删除成功");
    }

    public async Task<List<BorrowRecordDto>> GetMyRecordsAsync(Guid userId, string? status = null)
    {
        var query = _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Where(r => r.ApplicantId == userId && !r.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<List<BorrowRecordDto>> GetPendingTeacherApprovalsAsync(bool isAdmin)
    {
        var query = _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .ThenInclude(e => e!.Lab)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted &&
                (r.Status == BorrowStatus.PendingTeacherApproval ||
                r.Status == BorrowStatus.PendingAdminApproval ||
                r.Status == BorrowStatus.AwaitingPickup ||
                r.Status == BorrowStatus.ReturnPending ||
                r.Status == BorrowStatus.AwaitingReturn ||
                r.Status == BorrowStatus.RenewPending))
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(r =>
                r.Status == BorrowStatus.PendingTeacherApproval ||
                r.Status == BorrowStatus.RenewPending);

        var list = await query.OrderBy(r => r.CreatedAt).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<List<BorrowRecordDto>> GetAllRecordsAsync(string? status = null, string? keyword = null)
    {
        var query = _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .ThenInclude(e => e!.Lab)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrEmpty(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(r =>
                r.RecordNo.Contains(kw) ||
                (r.Equipment != null && r.Equipment.Name.Contains(kw)) ||
                (r.Applicant != null && r.Applicant.FullName.Contains(kw)));
        }

        var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<(bool Success, string Message)> SupervisorApproveAsync(Guid recordId, Guid approverId, bool approved, string? remark)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.PendingTeacherApproval)
            return (false, $"当前状态为「{record.Status}」，无法进行导师审批");

        record.SupervisorApprovalStatus = approved ? "同意" : "拒绝";
        record.SupervisorApprovalRemark = remark;
        record.SupervisorApprovalDate = DateTime.UtcNow;

        if (!approved)
        {
            record.Status = BorrowStatus.Rejected;
            if (record.Equipment != null)
            {
                record.Equipment.Status = "在库-可用";
            }
        }
        else
        {
            record.Status = BorrowStatus.PendingAdminApproval;
            record.ApproverId = approverId;
        }

        await _dbContext.SaveChangesAsync();
        return (true, approved ? "导师已审批通过，转交管理员" : "导师已拒绝");
    }

    public async Task<(bool Success, string Message)> AdminApproveAsync(Guid recordId, Guid approverId, bool approved, string? remark)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.PendingAdminApproval)
            return (false, $"当前状态为「{record.Status}」，无法进行管理员审批");

        record.AdminApprovalStatus = approved ? "同意" : "拒绝";
        record.AdminApprovalRemark = remark;
        record.AdminApprovalDate = DateTime.UtcNow;
        record.ApproverId = approverId;

        if (!approved)
        {
            record.Status = BorrowStatus.Rejected;
            if (record.Equipment != null)
            {
                record.Equipment.Status = "在库-可用";
            }
        }
        else
        {
            record.Status = BorrowStatus.AwaitingPickup;
        }

        await _dbContext.SaveChangesAsync();
        return (true, approved ? "管理员已审批通过，请在领取时间段内扫码确认借出" : "管理员已拒绝");
    }

    /// <summary>审批归还（归还审批中 -> 待归还/已借出）</summary>
    public async Task<(bool Success, string Message)> ApproveReturnAsync(Guid recordId, Guid checkerId, bool approved, string condition, string? remarks)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.ReturnPending)
            return (false, $"当前状态为「{record.Status}」，无法审批归还");

        if (approved)
        {
            record.Status = BorrowStatus.AwaitingReturn;
            record.ReturnCondition = condition;
            record.ReturnRemarks = remarks;
            record.ReturnCheckerId = checkerId;
            record.ReturnCheckDate = DateTime.UtcNow;
        }
        else
        {
            record.Status = BorrowStatus.Borrowed;
        }

        await _dbContext.SaveChangesAsync();
        return (true, approved ? "管理员已审批归还，请在领取时间段内扫码确认归还" : "归还申请已驳回");
    }

    public async Task<(bool Success, string Message)> ConfirmBorrowAsync(Guid recordId, Guid operatorId)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.AwaitingPickup && record.Status != BorrowStatus.Borrowed)
            return (false, $"当前状态为「{record.Status}」，无法确认借出");

        record.ActualBorrowDate = DateTime.UtcNow;
        record.ApproverId = operatorId;
        // 扫码核验时记录领取人信息
        record.RecipientId = record.ApplicantId;
        record.RecipientName = record.Applicant?.FullName;

        if (record.Equipment != null)
        {
            record.Equipment.Status = "借出";
        }

        if (record.Status == BorrowStatus.AwaitingPickup)
            record.Status = BorrowStatus.Borrowed;

        await _dbContext.SaveChangesAsync();
        return (true, "借出确认成功，设备已领取");
    }

    /// <summary>审批续借（续借审批中 -> 续借成功/拒绝）</summary>
    public async Task<(bool Success, string Message)> ApproveRenewAsync(Guid recordId, Guid approverId, bool approved, string? remark)
    {
        var record = await _dbContext.EquipmentBorrowRecords.FindAsync(recordId);
        if (record == null) return (false, "借还记录不存在");
        if (record.IsDeleted) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.RenewPending)
            return (false, $"当前状态为「{record.Status}」，无法审批续借");

        if (approved)
        {
            record.Status = BorrowStatus.Borrowed;
        }
        else
        {
            record.IsRenewed = false;
            record.Status = BorrowStatus.Borrowed;
        }

        await _dbContext.SaveChangesAsync();
        return (true, approved ? "续借已批准" : "续借已拒绝");
    }

    public async Task<(bool Success, string Message)> SubmitReturnAsync(Guid recordId, Guid userId)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.ApplicantId != userId)
            return (false, "只有申请人可以提交归还");
        if (record.Status != BorrowStatus.Borrowed && record.Status != BorrowStatus.Overdue)
            return (false, $"当前状态为「{record.Status}」，无法提交归还");

        record.Status = BorrowStatus.ReturnPending;
        await _dbContext.SaveChangesAsync();
        return (true, "归还申请已提交");
    }

    public async Task<(bool Success, string Message)> ConfirmReturnAsync(Guid recordId, Guid checkerId, string condition, string? remarks)
    {
        var record = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == recordId && !r.IsDeleted);

        if (record == null) return (false, "借还记录不存在");
        if (record.Status != BorrowStatus.AwaitingReturn)
            return (false, $"当前状态为「{record.Status}」，无法确认归还");

        record.Status = BorrowStatus.Returned;
        record.ActualReturnDate = DateTime.UtcNow;
        record.ReturnCondition = condition;
        record.ReturnRemarks = remarks;
        record.ReturnCheckerId = checkerId;
        record.ReturnCheckDate = DateTime.UtcNow;

        if (record.Equipment != null)
        {
            record.Equipment.Status = condition == "损坏" ? "在库-待维修" : "在库-可用";
        }

        await _dbContext.SaveChangesAsync();
        return (true, $"归还验收完成，设备状态已更新");
    }

    public async Task<(bool Success, string Message)> RenewAsync(Guid recordId, Guid applicantId, DateTime newReturnDate)
    {
        var record = await _dbContext.EquipmentBorrowRecords.FindAsync(recordId);

        if (record == null) return (false, "借还记录不存在");
        if (record.IsDeleted) return (false, "借还记录不存在");
        if (record.ApplicantId != applicantId)
            return (false, "只有申请人可以申请续借");
        if (record.Status != BorrowStatus.Borrowed && record.Status != BorrowStatus.Overdue)
            return (false, $"当前状态为「{record.Status}」，无法续借");
        if (record.IsRenewed)
            return (false, "该记录已续借过一次");
        if (newReturnDate <= record.ExpectedReturnDate)
            return (false, "续借后的归还时间必须晚于原计划归还时间");

        record.IsRenewed = true;
        record.RenewedReturnDate = newReturnDate;
        record.Status = BorrowStatus.RenewPending;

        await _dbContext.SaveChangesAsync();
        return (true, "续借申请已提交");
    }

    public async Task<List<OverdueRecordDto>> GetOverdueRecordsAsync()
    {
        var now = DateTime.UtcNow;
        var list = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted && (r.Status == BorrowStatus.Borrowed || r.Status == BorrowStatus.Overdue) && r.ExpectedReturnDate < now)
            .OrderBy(r => r.ExpectedReturnDate)
            .ToListAsync();

        foreach (var r in list.Where(r => r.Status == BorrowStatus.Borrowed))
            r.Status = BorrowStatus.Overdue;
        if (list.Any(r => r.Status == BorrowStatus.Overdue))
            await _dbContext.SaveChangesAsync();

        return list.Select(r => new OverdueRecordDto
        {
            RecordId = r.Id,
            RecordNo = r.RecordNo,
            EquipmentName = r.Equipment?.Name ?? "",
            EquipmentCode = r.Equipment?.Code ?? "",
            ApplicantName = r.Applicant?.FullName ?? "",
            BorrowDate = r.BorrowDate,
            ExpectedReturnDate = r.ExpectedReturnDate,
            DaysOverdue = (int)(now - r.ExpectedReturnDate).TotalDays
        }).ToList();
    }

    public async Task<List<ExpiringRecordDto>> GetExpiringRecordsAsync(int daysBefore = 1)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(daysBefore);

        var list = await _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted && r.Status == BorrowStatus.Borrowed && r.ExpectedReturnDate >= now && r.ExpectedReturnDate <= threshold)
            .OrderBy(r => r.ExpectedReturnDate)
            .ToListAsync();

        return list.Select(r => new ExpiringRecordDto
        {
            RecordId = r.Id,
            RecordNo = r.RecordNo,
            EquipmentName = r.Equipment?.Name ?? "",
            EquipmentCode = r.Equipment?.Code ?? "",
            ApplicantName = r.Applicant?.FullName ?? "",
            BorrowDate = r.BorrowDate,
            ExpectedReturnDate = r.ExpectedReturnDate,
            DaysUntilDue = (int)(r.ExpectedReturnDate - now).TotalDays
        }).ToList();
    }

    public async Task<List<BorrowFlowDto>> GetBorrowFlowAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbContext.EquipmentBorrowRecords
            .Include(r => r.Equipment)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(r => r.CreatedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(r => r.CreatedAt <= endDate.Value);

        var list = await query.OrderByDescending(r => r.CreatedAt).Take(500).ToListAsync();

        return list.Select(r => new BorrowFlowDto
        {
            RecordNo = r.RecordNo,
            EquipmentName = r.Equipment?.Name ?? "",
            EquipmentCode = r.Equipment?.Code ?? "",
            ApplicantName = r.Applicant?.FullName ?? "",
            Status = r.Status,
            BorrowDate = r.BorrowDate,
            ExpectedReturnDate = r.ExpectedReturnDate,
            ActualReturnDate = r.ActualReturnDate,
            Purpose = r.Purpose,
            ReturnCondition = r.ReturnCondition
        }).ToList();
    }

    private static BorrowRecordDto MapToDto(EquipmentBorrowRecord r) => new()
    {
        Id = r.Id,
        RecordNo = r.RecordNo,
        EquipmentId = r.EquipmentId,
        EquipmentCode = r.Equipment?.Code ?? "",
        EquipmentName = r.Equipment?.Name ?? "",
        EquipmentModel = r.Equipment?.Model,
        LabName = r.Equipment?.Lab?.Name,
        ApplicantId = r.ApplicantId,
        ApplicantName = r.Applicant?.FullName ?? "",
        ApproverId = r.ApproverId,
        ApproverName = r.Approver?.FullName,
        Status = r.Status,
        BorrowDate = r.BorrowDate,
        ExpectedReturnDate = r.ExpectedReturnDate,
        ActualBorrowDate = r.ActualBorrowDate,
        ActualReturnDate = r.ActualReturnDate,
        Purpose = r.Purpose,
        Phone = r.Phone,
        UsageLocation = r.UsageLocation,
        Remarks = r.Remarks,
        SupervisorApprovalStatus = r.SupervisorApprovalStatus,
        SupervisorApprovalRemark = r.SupervisorApprovalRemark,
        SupervisorApprovalDate = r.SupervisorApprovalDate,
        AdminApprovalStatus = r.AdminApprovalStatus,
        AdminApprovalRemark = r.AdminApprovalRemark,
        AdminApprovalDate = r.AdminApprovalDate,
        ReturnCondition = r.ReturnCondition,
        ReturnRemarks = r.ReturnRemarks,
        ReturnCheckerName = r.ReturnChecker?.FullName,
        ReturnCheckDate = r.ReturnCheckDate,
        IsRenewed = r.IsRenewed,
        RenewedReturnDate = r.RenewedReturnDate,
        DaysOverdue = r.Status == BorrowStatus.Borrowed || r.Status == BorrowStatus.Overdue
            ? (int)(DateTime.UtcNow - r.ExpectedReturnDate).TotalDays : 0,
        CreatedAt = r.CreatedAt,
        RecipientId = r.RecipientId,
        RecipientName = r.RecipientName,
        IsDeleted = r.IsDeleted
    };
}

public class CreateBorrowRequestDto
{
    public Guid EquipmentId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? UsageLocation { get; set; }
    public string? Remarks { get; set; }
}

public class BorrowRecordDto
{
    public Guid Id { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public Guid EquipmentId { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string? EquipmentModel { get; set; }
    public string? LabName { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public Guid? ApproverId { get; set; }
    public string? ApproverName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualBorrowDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? UsageLocation { get; set; }
    public string? Remarks { get; set; }
    public string? SupervisorApprovalStatus { get; set; }
    public string? SupervisorApprovalRemark { get; set; }
    public DateTime? SupervisorApprovalDate { get; set; }
    public string? AdminApprovalStatus { get; set; }
    public string? AdminApprovalRemark { get; set; }
    public DateTime? AdminApprovalDate { get; set; }
    public string? ReturnCondition { get; set; }
    public string? ReturnRemarks { get; set; }
    public string? ReturnCheckerName { get; set; }
    public DateTime? ReturnCheckDate { get; set; }
    public bool IsRenewed { get; set; }
    public DateTime? RenewedReturnDate { get; set; }
    public int DaysOverdue { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>领取人ID（扫码核验后记录）</summary>
    public Guid? RecipientId { get; set; }
    /// <summary>领取人姓名</summary>
    public string? RecipientName { get; set; }
    public bool IsDeleted { get; set; }
}

public class OverdueRecordDto
{
    public Guid RecordId { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public int DaysOverdue { get; set; }
}

public class ExpiringRecordDto
{
    public Guid RecordId { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public int DaysUntilDue { get; set; }
}

public class BorrowFlowDto
{
    public string RecordNo { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ReturnCondition { get; set; }
}
