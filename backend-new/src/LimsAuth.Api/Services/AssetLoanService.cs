using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

// Asset
public interface IAssetService
{
    Task<ApiResponse<PagedResponse<AssetDto>>> GetListAsync(AssetQueryRequest query);
    Task<ApiResponse<AssetDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<AssetDto>> CreateAsync(CreateAssetRequest request);
    Task<ApiResponse<AssetDto>> UpdateAsync(Guid id, UpdateAssetRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<AssetStatisticsDto>> GetStatisticsAsync();
}

public class AssetService : IAssetService
{
    private readonly AppDbContext _db;
    public AssetService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<AssetDto>>> GetListAsync(AssetQueryRequest query)
    {
        var q = _db.DevAssets.Include(a => a.ResponsibleUser).Include(a => a.Department)
            .Where(a => a.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(a => a.Name.Contains(query.Keyword) || a.Code.Contains(query.Keyword));
        if (query.DepartmentId.HasValue) q = q.Where(a => a.DepartmentId == query.DepartmentId);
        if (!string.IsNullOrEmpty(query.Category)) q = q.Where(a => a.Category == query.Category);
        if (!string.IsNullOrEmpty(query.DeviceStatus)) q = q.Where(a => a.DeviceStatus == query.DeviceStatus);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(a => new AssetDto
            {
                Id = a.Id, Code = a.Code, Name = a.Name, ModelNumber = a.ModelNumber, Specification = a.Specification,
                Category = a.Category, Unit = a.Unit, PurchaseDate = a.PurchaseDate, Brand = a.Brand,
                SerialNumber = a.SerialNumber, Price = a.Price, FundingSource = a.FundingSource,
                ServiceLife = a.ServiceLife, Supplier = a.Supplier, WarrantyPeriod = a.WarrantyPeriod,
                StorageLocation = a.StorageLocation,
                ResponsibleUserId = a.ResponsibleUserId, ResponsibleUserName = a.ResponsibleUser != null ? a.ResponsibleUser.RealName : null,
                DepartmentId = a.DepartmentId, DepartmentName = a.Department != null ? a.Department.Name : null,
                IsImportant = a.IsImportant, LabelInfo = a.LabelInfo, PhotoPath = a.PhotoPath,
                DeviceStatus = a.DeviceStatus, TotalQuantity = a.TotalQuantity,
                AvailableQuantity = a.AvailableQuantity, Status = a.Status, Description = a.Description,
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<AssetDto>>.Success(new PagedResponse<AssetDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<AssetDto>> GetByIdAsync(Guid id)
    {
        var a = await _db.DevAssets.Include(x => x.ResponsibleUser).Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return ApiResponse<AssetDto>.Error(404, "设备不存在");
        return ApiResponse<AssetDto>.Success(new AssetDto { Id = a.Id, Code = a.Code, Name = a.Name, DeviceStatus = a.DeviceStatus });
    }

    public async Task<ApiResponse<AssetDto>> CreateAsync(CreateAssetRequest request)
    {
        var a = new DevAsset
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, ModelNumber = request.ModelNumber,
            Specification = request.Specification, Category = request.Category, Unit = request.Unit,
            PurchaseDate = request.PurchaseDate, Brand = request.Brand, SerialNumber = request.SerialNumber,
            Price = request.Price, FundingSource = request.FundingSource, ServiceLife = request.ServiceLife,
            Supplier = request.Supplier, WarrantyPeriod = request.WarrantyPeriod,
            StorageLocation = request.StorageLocation, ResponsibleUserId = request.ResponsibleUserId,
            DepartmentId = request.DepartmentId, IsImportant = request.IsImportant,
            LabelInfo = request.LabelInfo, PhotoPath = request.PhotoPath,
            DeviceStatus = request.DeviceStatus, TotalQuantity = request.TotalQuantity,
            AvailableQuantity = request.AvailableQuantity, Status = request.Status,
            Description = request.Description, CreatedAt = DateTime.UtcNow
        };
        _db.DevAssets.Add(a);
        await _db.SaveChangesAsync();
        return ApiResponse<AssetDto>.Success(new AssetDto { Id = a.Id, Code = a.Code, Name = a.Name }, "设备创建成功");
    }

    public async Task<ApiResponse<AssetDto>> UpdateAsync(Guid id, UpdateAssetRequest request)
    {
        var a = await _db.DevAssets.FindAsync(id);
        if (a == null) return ApiResponse<AssetDto>.Error(404, "设备不存在");
        if (!string.IsNullOrEmpty(request.Name)) a.Name = request.Name;
        if (!string.IsNullOrEmpty(request.DeviceStatus)) a.DeviceStatus = request.DeviceStatus;
        if (request.Status.HasValue) a.Status = request.Status.Value;
        a.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<AssetDto>.Success(new AssetDto { Id = a.Id, Code = a.Code, Name = a.Name }, "设备更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var a = await _db.DevAssets.FindAsync(id);
        if (a == null) return ApiResponse.Error(404, "设备不存在");
        a.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("设备删除成功");
    }

    public async Task<ApiResponse<AssetStatisticsDto>> GetStatisticsAsync()
    {
        var assets = await _db.DevAssets.Where(a => a.IsDeleted == 0).ToListAsync();
        var stats = new AssetStatisticsDto
        {
            TotalAssets = assets.Count,
            TotalQuantity = assets.Sum(a => a.TotalQuantity),
            AvailableQuantity = assets.Sum(a => a.AvailableQuantity),
            ByCategory = assets.GroupBy(a => a.Category ?? "未分类").ToDictionary(g => g.Key, g => g.Sum(a => a.TotalQuantity)),
            ByStatus = assets.GroupBy(a => a.DeviceStatus).ToDictionary(g => g.Key, g => g.Count())
        };
        return ApiResponse<AssetStatisticsDto>.Success(stats);
    }
}

// LoanApply
public interface ILoanApplyService
{
    Task<ApiResponse<PagedResponse<LoanApplyDto>>> GetListAsync(LoanApplyQueryRequest query);
    Task<ApiResponse<LoanApplyDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<LoanApplyDto>> CreateAsync(CreateLoanApplyRequest request);
    Task<ApiResponse> AuditAsync(Guid id, AuditLoanRequest request);
    Task<ApiResponse> ConfirmReturnAsync(Guid id, ConfirmReturnRequest request);
    Task<ApiResponse> RenewAsync(Guid id, RenewLoanRequest request);
}

public class LoanApplyService : ILoanApplyService
{
    private readonly AppDbContext _db;
    public LoanApplyService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<LoanApplyDto>>> GetListAsync(LoanApplyQueryRequest query)
    {
        var q = _db.DevLoanApplies
            .Include(l => l.Asset).Include(l => l.Borrower).Include(l => l.Auditor).Include(l => l.ReturnConfirmUser)
            .Where(l => l.IsDeleted == 0).AsQueryable();
        if (query.AssetId.HasValue) q = q.Where(l => l.AssetId == query.AssetId);
        if (query.BorrowerId.HasValue) q = q.Where(l => l.BorrowerId == query.BorrowerId);
        if (!string.IsNullOrEmpty(query.AuditStatus)) q = q.Where(l => l.AuditStatus == query.AuditStatus);
        if (query.StartDate.HasValue) q = q.Where(l => l.CreatedAt >= query.StartDate);
        if (query.EndDate.HasValue) q = q.Where(l => l.CreatedAt <= query.EndDate);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(l => l.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(l => new LoanApplyDto
            {
                Id = l.Id, AssetId = l.AssetId, AssetCode = l.Asset != null ? l.Asset.Code : null,
                AssetName = l.Asset != null ? l.Asset.Name : null,
                BorrowerId = l.BorrowerId, BorrowerName = l.Borrower != null ? l.Borrower.RealName : null,
                BorrowerType = l.BorrowerType, LoanPurpose = l.LoanPurpose,
                LoanQuantity = l.LoanQuantity, ExpectedReturnDate = l.ExpectedReturnDate,
                ActualLoanTime = l.ActualLoanTime, ActualReturnTime = l.ActualReturnTime,
                AuditStatus = l.AuditStatus, AuditorId = l.AuditorId,
                AuditorName = l.Auditor != null ? l.Auditor.RealName : null,
                AuditTime = l.AuditTime, AuditOpinion = l.AuditOpinion,
                ReturnConfirmUserId = l.ReturnConfirmUserId,
                ReturnConfirmUserName = l.ReturnConfirmUser != null ? l.ReturnConfirmUser.RealName : null,
                DeviceCondition = l.DeviceCondition, CreatedAt = l.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<LoanApplyDto>>.Success(new PagedResponse<LoanApplyDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<LoanApplyDto>> GetByIdAsync(Guid id)
    {
        var l = await _db.DevLoanApplies.Include(x => x.Asset).Include(x => x.Borrower).FirstOrDefaultAsync(x => x.Id == id);
        if (l == null) return ApiResponse<LoanApplyDto>.Error(404, "借还记录不存在");
        return ApiResponse<LoanApplyDto>.Success(new LoanApplyDto { Id = l.Id, AssetId = l.AssetId, AuditStatus = l.AuditStatus });
    }

    public async Task<ApiResponse<LoanApplyDto>> CreateAsync(CreateLoanApplyRequest request)
    {
        var l = new DevLoanApply
        {
            Id = Guid.NewGuid(), AssetId = request.AssetId, BorrowerId = request.BorrowerId,
            BorrowerType = request.BorrowerType, LoanPurpose = request.LoanPurpose,
            LoanQuantity = request.LoanQuantity, ExpectedReturnDate = request.ExpectedReturnDate,
            CreatedAt = DateTime.UtcNow
        };
        _db.DevLoanApplies.Add(l);
        await _db.SaveChangesAsync();
        return ApiResponse<LoanApplyDto>.Success(new LoanApplyDto { Id = l.Id }, "借还申请创建成功");
    }

    public async Task<ApiResponse> AuditAsync(Guid id, AuditLoanRequest request)
    {
        var l = await _db.DevLoanApplies.FindAsync(id);
        if (l == null) return ApiResponse.Error(404, "借还记录不存在");
        l.AuditStatus = request.Approved ? "Approved" : "Rejected";
        l.AuditOpinion = request.Opinion;
        if (request.Approved)
        {
            l.ActualLoanTime = DateTime.UtcNow;
            var asset = await _db.DevAssets.FindAsync(l.AssetId);
            if (asset != null) asset.AvailableQuantity -= l.LoanQuantity;
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success(request.Approved ? "审批通过" : "审批驳回");
    }

    public async Task<ApiResponse> ConfirmReturnAsync(Guid id, ConfirmReturnRequest request)
    {
        var l = await _db.DevLoanApplies.FindAsync(id);
        if (l == null) return ApiResponse.Error(404, "借还记录不存在");
        l.ActualReturnTime = DateTime.UtcNow;
        l.AuditStatus = "Returned";
        l.DeviceCondition = request.DeviceCondition;
        var asset = await _db.DevAssets.FindAsync(l.AssetId);
        if (asset != null) asset.AvailableQuantity += l.LoanQuantity;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("归还确认成功");
    }

    public async Task<ApiResponse> RenewAsync(Guid id, RenewLoanRequest request)
    {
        var l = await _db.DevLoanApplies.FindAsync(id);
        if (l == null) return ApiResponse.Error(404, "借还记录不存在");
        l.ExpectedReturnDate = request.NewReturnDate;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("续借成功");
    }
}
