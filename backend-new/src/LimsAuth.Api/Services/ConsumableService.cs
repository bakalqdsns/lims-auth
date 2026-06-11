using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

// Consumable
public interface IConsumableService
{
    Task<ApiResponse<PagedResponse<ConsumableDto>>> GetListAsync(ConsumableQueryRequest query);
    Task<ApiResponse<ConsumableDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ConsumableDto>> CreateAsync(CreateConsumableRequest request);
    Task<ApiResponse<ConsumableDto>> UpdateAsync(Guid id, UpdateConsumableRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<ConsumableStatisticsDto>> GetStatisticsAsync();
}

public class ConsumableService : IConsumableService
{
    private readonly AppDbContext _db;
    public ConsumableService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<ConsumableDto>>> GetListAsync(ConsumableQueryRequest query)
    {
        var q = _db.MatConsumables.Where(c => c.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(c => c.Name.Contains(query.Keyword) || c.Code.Contains(query.Keyword));
        if (!string.IsNullOrEmpty(query.Category)) q = q.Where(c => c.Category == query.Category);
        if (query.IsLowStock == true) q = q.Where(c => c.CurrentStock <= c.MinStockThreshold);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(c => c.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(c => new ConsumableDto
            {
                Id = c.Id, Code = c.Code, Name = c.Name, Category = c.Category,
                SpecModel = c.SpecModel, Unit = c.Unit,
                CurrentStock = c.CurrentStock, AvailableStock = c.AvailableStock,
                LockedStock = c.LockedStock, MinStockThreshold = c.MinStockThreshold,
                StorageLocation = c.StorageLocation, Supplier = c.Supplier,
                UnitPrice = c.UnitPrice, Status = c.Status,
                IsLowStock = c.CurrentStock <= c.MinStockThreshold,
                CreatedAt = c.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<ConsumableDto>>.Success(new PagedResponse<ConsumableDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<ConsumableDto>> GetByIdAsync(Guid id)
    {
        var c = await _db.MatConsumables.FindAsync(id);
        if (c == null) return ApiResponse<ConsumableDto>.Error(404, "耗材不存在");
        return ApiResponse<ConsumableDto>.Success(new ConsumableDto { Id = c.Id, Code = c.Code, Name = c.Name, CurrentStock = c.CurrentStock });
    }

    public async Task<ApiResponse<ConsumableDto>> CreateAsync(CreateConsumableRequest request)
    {
        var c = new MatConsumable
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, Category = request.Category,
            SpecModel = request.SpecModel, Unit = request.Unit,
            CurrentStock = request.CurrentStock, AvailableStock = request.CurrentStock,
            MinStockThreshold = request.MinStockThreshold, StorageLocation = request.StorageLocation,
            Supplier = request.Supplier, UnitPrice = request.UnitPrice,
            Status = request.Status, CreatedAt = DateTime.UtcNow
        };
        _db.MatConsumables.Add(c);
        await _db.SaveChangesAsync();
        return ApiResponse<ConsumableDto>.Success(new ConsumableDto { Id = c.Id, Code = c.Code, Name = c.Name }, "耗材创建成功");
    }

    public async Task<ApiResponse<ConsumableDto>> UpdateAsync(Guid id, UpdateConsumableRequest request)
    {
        var c = await _db.MatConsumables.FindAsync(id);
        if (c == null) return ApiResponse<ConsumableDto>.Error(404, "耗材不存在");
        if (!string.IsNullOrEmpty(request.Name)) c.Name = request.Name;
        if (request.MinStockThreshold.HasValue) c.MinStockThreshold = request.MinStockThreshold.Value;
        if (request.Status.HasValue) c.Status = request.Status.Value;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<ConsumableDto>.Success(new ConsumableDto { Id = c.Id, Code = c.Code, Name = c.Name }, "耗材更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var c = await _db.MatConsumables.FindAsync(id);
        if (c == null) return ApiResponse.Error(404, "耗材不存在");
        c.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("耗材删除成功");
    }

    public async Task<ApiResponse<ConsumableStatisticsDto>> GetStatisticsAsync()
    {
        var consumables = await _db.MatConsumables.Where(c => c.IsDeleted == 0).ToListAsync();
        var stats = new ConsumableStatisticsDto
        {
            TotalTypes = consumables.Count,
            LowStockTypes = consumables.Count(c => c.CurrentStock <= c.MinStockThreshold),
            OutOfStockTypes = consumables.Count(c => c.CurrentStock == 0),
            TotalStockValue = consumables.Sum(c => c.CurrentStock * (c.UnitPrice ?? 0)),
            ByCategory = consumables.GroupBy(c => c.Category).ToDictionary(g => g.Key, g => g.Count),
            LowStockItems = consumables.Where(c => c.CurrentStock <= c.MinStockThreshold)
                .Select(c => new LowStockItemDto { Id = c.Id, Code = c.Code, Name = c.Name, Category = c.Category,
                    CurrentStock = c.CurrentStock, MinStockThreshold = c.MinStockThreshold, Unit = c.Unit }).ToList()
        };
        return ApiResponse<ConsumableStatisticsDto>.Success(stats);
    }
}

// Inbound
public interface IInboundService
{
    Task<ApiResponse<PagedResponse<InboundDto>>> GetListAsync(InboundQueryRequest query);
    Task<ApiResponse<InboundDto>> CreateAsync(CreateInboundRequest request, Guid handlerId);
    Task<ApiResponse> ApproveAsync(Guid id, ApprovalRequest request, Guid auditorId);
}

public class InboundService : IInboundService
{
    private readonly AppDbContext _db;
    public InboundService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<InboundDto>>> GetListAsync(InboundQueryRequest query)
    {
        var q = _db.MatInboundOrders.Include(i => i.Consumable).Include(i => i.Handler).Include(i => i.Auditor)
            .Where(i => i.IsDeleted == 0).AsQueryable();
        if (query.ConsumableId.HasValue) q = q.Where(i => i.ConsumableId == query.ConsumableId);
        if (!string.IsNullOrEmpty(query.Status)) q = q.Where(i => i.Status == query.Status);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(i => i.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(i => new InboundDto
            {
                Id = i.Id, Code = i.Code, ConsumableId = i.ConsumableId,
                ConsumableName = i.Consumable != null ? i.Consumable.Name : null,
                ConsumableCode = i.Consumable != null ? i.Consumable.Code : null,
                Quantity = i.Quantity, UnitPrice = i.UnitPrice,
                TotalAmount = i.UnitPrice * i.Quantity,
                Supplier = i.Supplier, InboundTime = i.InboundTime,
                HandlerId = i.HandlerId, HandlerName = i.Handler != null ? i.Handler.RealName : null,
                Remark = i.Remark, Status = i.Status,
                AuditorId = i.AuditorId, AuditorName = i.Auditor != null ? i.Auditor.RealName : null,
                AuditTime = i.AuditTime, AuditRemark = i.AuditRemark, CreatedAt = i.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<InboundDto>>.Success(new PagedResponse<InboundDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<InboundDto>> CreateAsync(CreateInboundRequest request, Guid handlerId)
    {
        var inbound = new MatInboundOrder
        {
            Id = Guid.NewGuid(), Code = request.Code, ConsumableId = request.ConsumableId,
            Quantity = request.Quantity, UnitPrice = request.UnitPrice,
            Supplier = request.Supplier, InboundTime = request.InboundTime ?? DateTime.UtcNow,
            HandlerId = handlerId, Remark = request.Remark,
            CreatedAt = DateTime.UtcNow
        };
        _db.MatInboundOrders.Add(inbound);
        await _db.SaveChangesAsync();
        return ApiResponse<InboundDto>.Success(new InboundDto { Id = inbound.Id, Code = inbound.Code }, "入库单创建成功");
    }

    public async Task<ApiResponse> ApproveAsync(Guid id, ApprovalRequest request, Guid auditorId)
    {
        var inbound = await _db.MatInboundOrders.FindAsync(id);
        if (inbound == null) return ApiResponse.Error(404, "入库单不存在");

        inbound.AuditorId = auditorId;
        inbound.AuditTime = DateTime.UtcNow;
        inbound.AuditRemark = request.Comment;

        if (request.Approved)
        {
            inbound.Status = "Approved";
            var consumable = await _db.MatConsumables.FindAsync(inbound.ConsumableId);
            if (consumable != null)
            {
                consumable.CurrentStock += inbound.Quantity;
                consumable.AvailableStock += inbound.Quantity;
                consumable.UpdatedAt = DateTime.UtcNow;

                _db.MatStockLogs.Add(new MatStockLog
                {
                    Id = Guid.NewGuid(), ConsumableId = consumable.Id,
                    ChangeType = "Inbound", BeforeQuantity = consumable.CurrentStock - inbound.Quantity,
                    ChangeQuantity = inbound.Quantity, AfterQuantity = consumable.CurrentStock,
                    ReferenceId = inbound.Id, ReferenceNo = inbound.Code,
                    OperatorId = auditorId
                });
            }
        }
        else
        {
            inbound.Status = "Rejected";
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success(request.Approved ? "审核通过" : "审核驳回");
    }
}

// Outbound
public interface IOutboundService
{
    Task<ApiResponse<PagedResponse<OutboundDto>>> GetListAsync(OutboundQueryRequest query);
    Task<ApiResponse<OutboundDto>> CreateAsync(CreateOutboundRequest request, Guid applicantId);
    Task<ApiResponse> ApproveAsync(Guid id, ApprovalRequest request, Guid auditorId);
}

public class OutboundService : IOutboundService
{
    private readonly AppDbContext _db;
    public OutboundService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<OutboundDto>>> GetListAsync(OutboundQueryRequest query)
    {
        var q = _db.MatOutboundOrders.Include(o => o.Consumable).Include(o => o.TargetRoom)
            .Include(o => o.Applicant).Include(o => o.Auditor)
            .Where(o => o.IsDeleted == 0).AsQueryable();
        if (query.ConsumableId.HasValue) q = q.Where(o => o.ConsumableId == query.ConsumableId);
        if (!string.IsNullOrEmpty(query.Status)) q = q.Where(o => o.Status == query.Status);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(o => new OutboundDto
            {
                Id = o.Id, Code = o.Code, ConsumableId = o.ConsumableId,
                ConsumableName = o.Consumable != null ? o.Consumable.Name : null,
                ConsumableCode = o.Consumable != null ? o.Consumable.Code : null,
                Category = o.Consumable != null ? o.Consumable.Category : null,
                Quantity = o.Quantity, Purpose = o.Purpose,
                TargetRoomId = o.TargetRoomId, TargetRoomName = o.TargetRoom != null ? o.TargetRoom.Name : null,
                OutTime = o.OutTime, ApplicantId = o.ApplicantId,
                ApplicantName = o.Applicant != null ? o.Applicant.RealName : null,
                Remark = o.Remark, Status = o.Status,
                AuditorId = o.AuditorId, AuditorName = o.Auditor != null ? o.Auditor.RealName : null,
                AuditTime = o.AuditTime, CreatedAt = o.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<OutboundDto>>.Success(new PagedResponse<OutboundDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<OutboundDto>> CreateAsync(CreateOutboundRequest request, Guid applicantId)
    {
        var outbound = new MatOutboundOrder
        {
            Id = Guid.NewGuid(), Code = request.Code, ConsumableId = request.ConsumableId,
            Quantity = request.Quantity, Purpose = request.Purpose,
            TargetRoomId = request.TargetRoomId, ApplicantId = applicantId,
            OutTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow
        };
        _db.MatOutboundOrders.Add(outbound);
        await _db.SaveChangesAsync();
        return ApiResponse<OutboundDto>.Success(new OutboundDto { Id = outbound.Id, Code = outbound.Code }, "出库单创建成功");
    }

    public async Task<ApiResponse> ApproveAsync(Guid id, ApprovalRequest request, Guid auditorId)
    {
        var outbound = await _db.MatOutboundOrders.FindAsync(id);
        if (outbound == null) return ApiResponse.Error(404, "出库单不存在");

        outbound.AuditorId = auditorId;
        outbound.AuditTime = DateTime.UtcNow;
        outbound.AuditRemark = request.Comment;

        if (request.Approved)
        {
            outbound.Status = "Approved";
            var consumable = await _db.MatConsumables.FindAsync(outbound.ConsumableId);
            if (consumable != null && consumable.AvailableStock >= outbound.Quantity)
            {
                consumable.AvailableStock -= outbound.Quantity;
                consumable.CurrentStock -= outbound.Quantity;
                consumable.UpdatedAt = DateTime.UtcNow;

                _db.MatStockLogs.Add(new MatStockLog
                {
                    Id = Guid.NewGuid(), ConsumableId = consumable.Id,
                    ChangeType = "Outbound", BeforeQuantity = consumable.CurrentStock + outbound.Quantity,
                    ChangeQuantity = -outbound.Quantity, AfterQuantity = consumable.CurrentStock,
                    ReferenceId = outbound.Id, ReferenceNo = outbound.Code,
                    OperatorId = auditorId
                });
            }
        }
        else
        {
            outbound.Status = "Rejected";
        }
        await _db.SaveChangesAsync();
        return ApiResponse.Success(request.Approved ? "审核通过" : "审核驳回");
    }
}

// StockLog
public interface IStockLogService
{
    Task<ApiResponse<PagedResponse<StockLogDto>>> GetListAsync(StockLogQueryRequest query);
    Task<ApiResponse<StockLogDto>> AdjustStockAsync(CreateStockAdjustmentRequest request, Guid operatorId);
}

public class StockLogService : IStockLogService
{
    private readonly AppDbContext _db;
    public StockLogService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<StockLogDto>>> GetListAsync(StockLogQueryRequest query)
    {
        var q = _db.MatStockLogs.Include(l => l.Consumable).Include(l => l.Operator)
            .Where(l => l.Consumable.IsDeleted == 0).AsQueryable();
        if (query.ConsumableId.HasValue) q = q.Where(l => l.ConsumableId == query.ConsumableId);
        if (!string.IsNullOrEmpty(query.ChangeType)) q = q.Where(l => l.ChangeType == query.ChangeType);
        if (query.StartDate.HasValue) q = q.Where(l => l.CreatedAt >= query.StartDate);
        if (query.EndDate.HasValue) q = q.Where(l => l.CreatedAt <= query.EndDate);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(l => l.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(l => new StockLogDto
            {
                Id = l.Id, ConsumableId = l.ConsumableId,
                ConsumableName = l.Consumable != null ? l.Consumable.Name : null,
                ChangeType = l.ChangeType, ChangeQuantity = l.ChangeQuantity,
                BeforeStock = l.BeforeQuantity, AfterStock = l.AfterQuantity,
                ReferenceId = l.ReferenceId, ReferenceNo = l.ReferenceNo,
                OperatorId = l.OperatorId, OperatorName = l.Operator != null ? l.Operator.RealName : null,
                Remark = l.Remark, CreatedAt = l.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<StockLogDto>>.Success(new PagedResponse<StockLogDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<StockLogDto>> AdjustStockAsync(CreateStockAdjustmentRequest request, Guid operatorId)
    {
        var consumable = await _db.MatConsumables.FindAsync(request.ConsumableId);
        if (consumable == null) return ApiResponse<StockLogDto>.Error(404, "耗材不存在");

        var before = consumable.CurrentStock;
        var change = request.AdjustmentQuantity;
        consumable.CurrentStock += change;
        consumable.AvailableStock += change;
        consumable.UpdatedAt = DateTime.UtcNow;

        var log = new MatStockLog
        {
            Id = Guid.NewGuid(), ConsumableId = consumable.Id,
            ChangeType = request.AdjustmentType, BeforeQuantity = before,
            ChangeQuantity = change, AfterQuantity = consumable.CurrentStock,
            OperatorId = operatorId, Remark = request.Reason,
            CreatedAt = DateTime.UtcNow
        };
        _db.MatStockLogs.Add(log);
        await _db.SaveChangesAsync();
        return ApiResponse<StockLogDto>.Success(new StockLogDto { Id = log.Id, ChangeType = log.ChangeType }, "库存调整成功");
    }
}
