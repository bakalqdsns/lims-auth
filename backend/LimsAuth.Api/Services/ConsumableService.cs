using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

public interface IConsumableService
{
    // 耗材分类
    Task<List<ConsumableCategoryDto>> GetCategoriesAsync();
    Task<ConsumableCategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<ConsumableCategory> CreateCategoryAsync(CreateConsumableCategoryRequest request);
    Task<ConsumableCategory?> UpdateCategoryAsync(Guid id, UpdateConsumableCategoryRequest request);
    Task<bool> DeleteCategoryAsync(Guid id);

    // 耗材
    Task<(List<ConsumableDto> Items, int Total)> GetConsumablesAsync(ConsumableQuery query);
    Task<ConsumableDto?> GetConsumableByIdAsync(Guid id);
    Task<Consumable> CreateConsumableAsync(CreateConsumableRequest request);
    Task<Consumable?> UpdateConsumableAsync(Guid id, UpdateConsumableRequest request);
    Task<bool> DeleteConsumableAsync(Guid id);
    Task<ImportConsumableResult> ImportConsumablesAsync(Stream fileStream);
    Task<byte[]> GenerateConsumableImportTemplateAsync();

    // 入库
    Task<(List<ConsumableInRecordDto> Items, int Total)> GetInRecordsAsync(ConsumableInRecordQuery query);
    Task<ConsumableInRecord> CreateInRecordAsync(CreateConsumableInRecordRequest request, Guid operatorId, string operatorName);
    Task<int> BatchCreateInRecordsAsync(BatchCreateConsumableInRecordRequest request, Guid operatorId, string operatorName);
    Task<bool> ApproveInRecordAsync(Guid id, bool approved, string? comment, Guid approverId, string approverName);
    Task<bool> DeleteInRecordAsync(Guid id);

    // 出库
    Task<(List<ConsumableOutRecordDto> Items, int Total)> GetOutRecordsAsync(ConsumableOutRecordQuery query);
    Task<(List<ConsumableOutRecordDto> Items, int Total)> GetMyOutRecordsAsync(Guid applicantId, ConsumableOutRecordQuery query);
    Task<ConsumableOutRecord> CreateOutRecordAsync(CreateConsumableOutRecordRequest request, Guid applicantId, string applicantName);
    Task<int> BatchCreateOutRecordsAsync(BatchCreateConsumableOutRecordRequest request, Guid applicantId, string applicantName);
    Task<bool> ApproveOutRecordAsync(Guid id, bool approved, string? comment, Guid approverId, string approverName);
    Task<bool> DeleteOutRecordAsync(Guid id);

    // 库存
    Task<ConsumableStockAdjustment> AdjustStockAsync(CreateConsumableStockAdjustmentRequest request, Guid operatorId, string operatorName);
    Task<int> StockCheckAsync(StockCheckRequest request, Guid operatorId, string operatorName);
    Task<(List<ConsumableStockLogDto> Items, int Total)> GetStockLogsAsync(ConsumableStockLogQuery query);
    Task<(List<ConsumableStockAdjustmentDto> Items, int Total)> GetStockAdjustmentsAsync(ConsumableStockAdjustmentQuery query);

    // 统计
    Task<ConsumableStatisticsDto> GetStatisticsAsync(ConsumableStatisticsQuery? query);

    // 通知
    Task<List<ConsumableNotificationDto>> GetNotificationsAsync(Guid userId, ConsumableNotificationQuery query);
    Task<int> GetUnreadNotificationCountAsync(Guid userId);
    Task<bool> MarkNotificationReadAsync(Guid id, Guid userId);

    // 低库存检查并发送通知
    Task CheckLowStockAndNotifyAsync();
}

public class ConsumableService : IConsumableService
{
    private readonly AppDbContext _dbContext;

    public ConsumableService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region 耗材分类

    public async Task<List<ConsumableCategoryDto>> GetCategoriesAsync()
    {
        var categories = await _dbContext.ConsumableCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(c => new ConsumableCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Remark = c.Remark,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<ConsumableCategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var c = await _dbContext.ConsumableCategories.FindAsync(id);
        if (c == null) return null;

        return new ConsumableCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Remark = c.Remark,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }

    public async Task<ConsumableCategory> CreateCategoryAsync(CreateConsumableCategoryRequest request)
    {
        var category = new ConsumableCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Remark = request.Remark,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.ConsumableCategories.Add(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<ConsumableCategory?> UpdateCategoryAsync(Guid id, UpdateConsumableCategoryRequest request)
    {
        var category = await _dbContext.ConsumableCategories.FindAsync(id);
        if (category == null) return null;

        if (request.Name != null) category.Name = request.Name;
        if (request.Remark != null) category.Remark = request.Remark;
        if (request.IsActive.HasValue) category.IsActive = request.IsActive.Value;
        category.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _dbContext.ConsumableCategories.FindAsync(id);
        if (category == null) return false;

        var hasConsumables = await _dbContext.Consumables
            .AnyAsync(c => c.CategoryId == id && !c.IsDeleted);
        if (hasConsumables)
            throw new InvalidOperationException("该分类下存在耗材，无法删除");

        _dbContext.ConsumableCategories.Remove(category);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 耗材

    public async Task<(List<ConsumableDto> Items, int Total)> GetConsumablesAsync(ConsumableQuery query)
    {
        var q = _dbContext.Consumables
            .Include(c => c.Category)
            .Where(c => !c.IsDeleted && c.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            q = q.Where(c =>
                c.Name.Contains(query.Keyword) ||
                c.Code.Contains(query.Keyword) ||
                (c.Specification != null && c.Specification.Contains(query.Keyword)));
        }

        if (query.CategoryId.HasValue)
            q = q.Where(c => c.CategoryId == query.CategoryId);

        if (!string.IsNullOrEmpty(query.Supplier))
            q = q.Where(c => c.Supplier != null && c.Supplier.Contains(query.Supplier));

        if (query.IsLowStock == true)
            q = q.Where(c => c.CurrentStock <= c.MinStock);

        if (query.IsActive.HasValue)
            q = q.Where(c => c.IsActive == query.IsActive.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderBy(c => c.Code)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var result = items.Select(c => new ConsumableDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            CategoryId = c.CategoryId,
            CategoryName = c.Category?.Name,
            Specification = c.Specification,
            Unit = c.Unit,
            CurrentStock = c.CurrentStock,
            AvailableStock = c.AvailableStock,
            LockedStock = c.LockedStock,
            MinStock = c.MinStock,
            Location = c.Location,
            Supplier = c.Supplier,
            UnitPrice = c.UnitPrice,
            MaxSingleRequest = c.MaxSingleRequest,
            MonthlyLimit = c.MonthlyLimit,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            IsLowStock = c.CurrentStock <= c.MinStock
        }).ToList();

        return (result, total);
    }

    public async Task<ConsumableDto?> GetConsumableByIdAsync(Guid id)
    {
        var c = await _dbContext.Consumables
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (c == null) return null;

        return new ConsumableDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            CategoryId = c.CategoryId,
            CategoryName = c.Category?.Name,
            Specification = c.Specification,
            Unit = c.Unit,
            CurrentStock = c.CurrentStock,
            AvailableStock = c.AvailableStock,
            LockedStock = c.LockedStock,
            MinStock = c.MinStock,
            Location = c.Location,
            Supplier = c.Supplier,
            UnitPrice = c.UnitPrice,
            MaxSingleRequest = c.MaxSingleRequest,
            MonthlyLimit = c.MonthlyLimit,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            IsLowStock = c.CurrentStock <= c.MinStock
        };
    }

    public async Task<Consumable> CreateConsumableAsync(CreateConsumableRequest request)
    {
        var exists = await _dbContext.Consumables
            .AnyAsync(c => c.Code == request.Code && !c.IsDeleted);
        if (exists)
            throw new InvalidOperationException($"耗材编号 {request.Code} 已存在");

        var consumable = new Consumable
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            CategoryId = request.CategoryId,
            Specification = request.Specification,
            Unit = request.Unit,
            CurrentStock = request.CurrentStock,
            AvailableStock = request.CurrentStock,
            MinStock = request.MinStock,
            Location = request.Location,
            Supplier = request.Supplier,
            UnitPrice = request.UnitPrice,
            MaxSingleRequest = request.MaxSingleRequest,
            MonthlyLimit = request.MonthlyLimit,
            Description = request.Description,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Consumables.Add(consumable);
        await _dbContext.SaveChangesAsync();
        return consumable;
    }

    public async Task<ImportConsumableResult> ImportConsumablesAsync(Stream fileStream)
    {
        var result = new ImportConsumableResult { Total = 0, Success = 0, Failed = 0 };

        using var package = new OfficeOpenXml.ExcelPackage();
        await package.LoadAsync(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new InvalidOperationException("无法读取 Excel 工作表");

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
            throw new InvalidOperationException("Excel 文件为空或格式不正确");

        var categoryCache = (await _dbContext.ConsumableCategories.Where(c => c.IsActive).ToListAsync())
            .ToDictionary(c => c.Name, c => c.Id);

        for (int row = 2; row <= rowCount; row++)
        {
            result.Total++;
            try
            {
                var code = worksheet.Cells[row, 1].Text?.Trim();
                var name = worksheet.Cells[row, 2].Text?.Trim();
                var categoryName = worksheet.Cells[row, 3].Text?.Trim();
                var specification = worksheet.Cells[row, 4].Text?.Trim();
                var unit = worksheet.Cells[row, 5].Text?.Trim();
                var currentStockStr = worksheet.Cells[row, 6].Text?.Trim();
                var minStockStr = worksheet.Cells[row, 7].Text?.Trim();
                var location = worksheet.Cells[row, 8].Text?.Trim();
                var supplier = worksheet.Cells[row, 9].Text?.Trim();
                var unitPriceStr = worksheet.Cells[row, 10].Text?.Trim();

                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                {
                    result.Errors.Add($"第 {row} 行：编号和名称不能为空");
                    result.Failed++;
                    continue;
                }

                if (await _dbContext.Consumables.AnyAsync(c => c.Code == code && !c.IsDeleted))
                {
                    result.Errors.Add($"第 {row} 行：编号 {code} 已存在");
                    result.Failed++;
                    continue;
                }

                Guid? categoryId = null;
                if (!string.IsNullOrEmpty(categoryName) && categoryCache.TryGetValue(categoryName, out var cat))
                    categoryId = cat;

                decimal currentStock = 0;
                if (!string.IsNullOrEmpty(currentStockStr) && decimal.TryParse(currentStockStr, out var cs))
                    currentStock = cs;

                decimal minStock = 0;
                if (!string.IsNullOrEmpty(minStockStr) && decimal.TryParse(minStockStr, out var ms))
                    minStock = ms;

                decimal? unitPrice = null;
                if (!string.IsNullOrEmpty(unitPriceStr) && decimal.TryParse(unitPriceStr, out var up))
                    unitPrice = up;

                var consumable = new Consumable
                {
                    Id = Guid.NewGuid(),
                    Code = code,
                    Name = name,
                    CategoryId = categoryId,
                    Specification = specification,
                    Unit = string.IsNullOrEmpty(unit) ? "个" : unit,
                    CurrentStock = currentStock,
                    AvailableStock = currentStock,
                    MinStock = minStock,
                    Location = location,
                    Supplier = supplier,
                    UnitPrice = unitPrice,
                    MaxSingleRequest = 999999,
                    MonthlyLimit = 999999,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Consumables.Add(consumable);
                result.Success++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"第 {row} 行：{ex.Message}");
                result.Failed++;
            }
        }

        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<byte[]> GenerateConsumableImportTemplateAsync()
    {
        using var package = new OfficeOpenXml.ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("耗材列表");
        ws.Cells[1, 1].Value = "耗材编号 *";
        ws.Cells[1, 2].Value = "耗材名称 *";
        ws.Cells[1, 3].Value = "分类名称";
        ws.Cells[1, 4].Value = "规格型号";
        ws.Cells[1, 5].Value = "单位";
        ws.Cells[1, 6].Value = "当前库存";
        ws.Cells[1, 7].Value = "最低库存";
        ws.Cells[1, 8].Value = "存放位置";
        ws.Cells[1, 9].Value = "供应商";
        ws.Cells[1, 10].Value = "单价(元)";

        ws.Column(1).Width = 15;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 15;
        ws.Column(4).Width = 20;
        ws.Column(5).Width = 10;
        ws.Column(6).Width = 12;
        ws.Column(7).Width = 12;
        ws.Column(8).Width = 20;
        ws.Column(9).Width = 20;
        ws.Column(10).Width = 12;

        // 示例数据行
        ws.Cells[2, 1].Value = "HC-0001";
        ws.Cells[2, 2].Value = "一次性离心管";
        ws.Cells[2, 3].Value = "实验耗材";
        ws.Cells[2, 4].Value = "50mL";
        ws.Cells[2, 5].Value = "支";
        ws.Cells[2, 6].Value = 0;
        ws.Cells[2, 7].Value = 100;
        ws.Cells[2, 8].Value = "A柜-01-03";
        ws.Cells[2, 9].Value = "上海科汇";
        ws.Cells[2, 10].Value = 5.5;

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<Consumable?> UpdateConsumableAsync(Guid id, UpdateConsumableRequest request)
    {
        var consumable = await _dbContext.Consumables.FindAsync(id);
        if (consumable == null || consumable.IsDeleted) return null;

        if (request.Name != null) consumable.Name = request.Name;
        if (request.CategoryId.HasValue) consumable.CategoryId = request.CategoryId;
        if (request.Specification != null) consumable.Specification = request.Specification;
        if (request.Unit != null) consumable.Unit = request.Unit;
        if (request.MinStock.HasValue) consumable.MinStock = request.MinStock.Value;
        if (request.Location != null) consumable.Location = request.Location;
        if (request.Supplier != null) consumable.Supplier = request.Supplier;
        if (request.UnitPrice.HasValue) consumable.UnitPrice = request.UnitPrice.Value;
        if (request.MaxSingleRequest.HasValue) consumable.MaxSingleRequest = request.MaxSingleRequest.Value;
        if (request.MonthlyLimit.HasValue) consumable.MonthlyLimit = request.MonthlyLimit.Value;
        if (request.Description != null) consumable.Description = request.Description;
        if (request.IsActive.HasValue) consumable.IsActive = request.IsActive.Value;

        consumable.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return consumable;
    }

    public async Task<bool> DeleteConsumableAsync(Guid id)
    {
        var consumable = await _dbContext.Consumables.FindAsync(id);
        if (consumable == null || consumable.IsDeleted) return false;

        consumable.IsDeleted = true;
        consumable.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 入库

    public async Task<(List<ConsumableInRecordDto> Items, int Total)> GetInRecordsAsync(ConsumableInRecordQuery query)
    {
        var q = _dbContext.ConsumableInRecords
            .Include(r => r.Consumable)
            .Include(r => r.Handler)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            q = q.Where(r =>
                r.RecordNo.Contains(query.Keyword) ||
                (r.Consumable != null && r.Consumable.Name.Contains(query.Keyword)));
        }

        if (query.ConsumableId.HasValue)
            q = q.Where(r => r.ConsumableId == query.ConsumableId);

        if (query.HandlerId.HasValue)
            q = q.Where(r => r.HandlerId == query.HandlerId);

        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(r => r.Status == query.Status);

        if (query.StartDate.HasValue)
            q = q.Where(r => r.InTime >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(r => r.InTime <= query.EndDate.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items.Select(MapInRecord).ToList(), total);
    }

    public async Task<ConsumableInRecord> CreateInRecordAsync(CreateConsumableInRecordRequest request, Guid operatorId, string operatorName)
    {
        var consumable = await _dbContext.Consumables.FindAsync(request.ConsumableId);
        if (consumable == null)
            throw new InvalidOperationException("耗材不存在");

        var recordNo = GenerateRecordNo("IN");

        var record = new ConsumableInRecord
        {
            Id = Guid.NewGuid(),
            RecordNo = recordNo,
            ConsumableId = request.ConsumableId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Supplier = request.Supplier ?? consumable.Supplier,
            InTime = request.InTime ?? DateTime.UtcNow,
            HandlerId = operatorId,
            HandlerName = operatorName,
            Remark = request.Remark,
            Status = "Pending",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ConsumableInRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record;
    }

    public async Task<int> BatchCreateInRecordsAsync(BatchCreateConsumableInRecordRequest request, Guid operatorId, string operatorName)
    {
        int successCount = 0;
        foreach (var item in request.Items)
        {
            try
            {
                await CreateInRecordAsync(item, operatorId, operatorName);
                successCount++;
            }
            catch { }
        }
        return successCount;
    }

    public async Task<bool> ApproveInRecordAsync(Guid id, bool approved, string? comment, Guid approverId, string approverName)
    {
        var record = await _dbContext.ConsumableInRecords
            .Include(r => r.Consumable)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (record == null) return false;
        if (record.Status != "Pending") return false;

        record.ApprovedBy = approverId;
        record.ApproverName = approverName;
        record.ApprovedAt = DateTime.UtcNow;
        record.ApprovalRemark = comment;
        record.Status = approved ? "Approved" : "Rejected";

        if (approved && record.Consumable != null)
        {
            record.Consumable.CurrentStock += record.Quantity;
            record.Consumable.AvailableStock += record.Quantity;
            record.Consumable.UpdatedAt = DateTime.UtcNow;

            _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
            {
                Id = Guid.NewGuid(),
                ConsumableId = record.Consumable.Id,
                ChangeType = "In",
                ChangeQuantity = record.Quantity,
                BeforeStock = record.Consumable.CurrentStock - record.Quantity,
                AfterStock = record.Consumable.CurrentStock,
                ReferenceId = record.Id,
                ReferenceNo = record.RecordNo,
                OperatorId = approverId,
                OperatorName = approverName,
                Remark = $"入库审批通过：{record.Quantity} {record.Consumable.Unit}",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInRecordAsync(Guid id)
    {
        var record = await _dbContext.ConsumableInRecords.FindAsync(id);
        if (record == null || record.IsDeleted) return false;

        record.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 出库

    public async Task<(List<ConsumableOutRecordDto> Items, int Total)> GetOutRecordsAsync(ConsumableOutRecordQuery query)
    {
        var q = _dbContext.ConsumableOutRecords
            .Include(r => r.Consumable).ThenInclude(c => c!.Category)
            .Include(r => r.Applicant)
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            q = q.Where(r =>
                r.RecordNo.Contains(query.Keyword) ||
                (r.Consumable != null && r.Consumable.Name.Contains(query.Keyword)));
        }

        if (query.ConsumableId.HasValue)
            q = q.Where(r => r.ConsumableId == query.ConsumableId);

        if (query.ApplicantId.HasValue)
            q = q.Where(r => r.ApplicantId == query.ApplicantId);

        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(r => r.Status == query.Status);

        if (query.StartDate.HasValue)
            q = q.Where(r => r.OutTime >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(r => r.OutTime <= query.EndDate.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items.Select(MapOutRecord).ToList(), total);
    }

    public async Task<(List<ConsumableOutRecordDto> Items, int Total)> GetMyOutRecordsAsync(Guid applicantId, ConsumableOutRecordQuery query)
    {
        query.ApplicantId = applicantId;
        return await GetOutRecordsAsync(query);
    }

    public async Task<ConsumableOutRecord> CreateOutRecordAsync(CreateConsumableOutRecordRequest request, Guid applicantId, string applicantName)
    {
        var consumable = await _dbContext.Consumables.FindAsync(request.ConsumableId);
        if (consumable == null)
            throw new InvalidOperationException("耗材不存在");

        if (consumable.IsDeleted || !consumable.IsActive)
            throw new InvalidOperationException("该耗材已停用");

        if (consumable.AvailableStock < request.Quantity)
            throw new InvalidOperationException($"可用库存不足，当前可用：{consumable.AvailableStock} {consumable.Unit}");

        if (request.Quantity > consumable.MaxSingleRequest)
            throw new InvalidOperationException($"超出单次最大领用数量 {consumable.MaxSingleRequest} {consumable.Unit}");

        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var monthlyUsed = (decimal)(await _dbContext.ConsumableOutRecords
            .Where(r => r.ConsumableId == request.ConsumableId &&
                        r.ApplicantId == applicantId &&
                        r.Status == "Approved" &&
                        r.OutTime >= thisMonth)
            .ToListAsync())
            .Sum(r => r.Quantity);

        if (monthlyUsed + request.Quantity > consumable.MonthlyLimit)
            throw new InvalidOperationException($"超出月度领用额度 {consumable.MonthlyLimit} {consumable.Unit}（本月已使用：{monthlyUsed}）");

        var recordNo = GenerateRecordNo("OUT");

        var record = new ConsumableOutRecord
        {
            Id = Guid.NewGuid(),
            RecordNo = recordNo,
            ConsumableId = request.ConsumableId,
            Quantity = request.Quantity,
            UsagePurpose = request.UsagePurpose,
            UsageLab = request.UsageLab,
            OutTime = DateTime.UtcNow,
            ApplicantId = applicantId,
            ApplicantName = applicantName,
            Remark = request.Remark,
            Status = "Pending",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ConsumableOutRecords.Add(record);

        consumable.AvailableStock -= request.Quantity;
        consumable.LockedStock += request.Quantity;
        consumable.UpdatedAt = DateTime.UtcNow;

        _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
        {
            Id = Guid.NewGuid(),
            ConsumableId = consumable.Id,
            ChangeType = "Lock",
            ChangeQuantity = -request.Quantity,
            BeforeStock = consumable.AvailableStock + request.Quantity,
            AfterStock = consumable.AvailableStock,
            ReferenceId = record.Id,
            ReferenceNo = recordNo,
            OperatorId = applicantId,
            OperatorName = applicantName,
            Remark = $"领用申请锁定：{request.Quantity} {consumable.Unit}",
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return record;
    }

    public async Task<int> BatchCreateOutRecordsAsync(BatchCreateConsumableOutRecordRequest request, Guid applicantId, string applicantName)
    {
        int successCount = 0;
        foreach (var item in request.Items)
        {
            try
            {
                await CreateOutRecordAsync(item, applicantId, applicantName);
                successCount++;
            }
            catch { }
        }
        return successCount;
    }

    public async Task<bool> ApproveOutRecordAsync(Guid id, bool approved, string? comment, Guid approverId, string approverName)
    {
        var record = await _dbContext.ConsumableOutRecords
            .Include(r => r.Consumable)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (record == null) return false;
        if (record.Status != "Pending") return false;

        record.ApprovedBy = approverId;
        record.ApproverName = approverName;
        record.ApprovedAt = DateTime.UtcNow;
        record.ApprovalRemark = comment;
        record.Status = approved ? "Approved" : "Rejected";

        if (record.Consumable != null)
        {
            if (approved)
            {
                record.Consumable.CurrentStock -= record.Quantity;
                record.Consumable.LockedStock -= record.Quantity;

                _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
                {
                    Id = Guid.NewGuid(),
                    ConsumableId = record.Consumable.Id,
                    ChangeType = "Out",
                    ChangeQuantity = -record.Quantity,
                    BeforeStock = record.Consumable.CurrentStock + record.Quantity,
                    AfterStock = record.Consumable.CurrentStock,
                    ReferenceId = record.Id,
                    ReferenceNo = record.RecordNo,
                    OperatorId = approverId,
                    OperatorName = approverName,
                    Remark = $"领用审批通过：{record.Quantity} {record.Consumable.Unit}",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                record.Consumable.AvailableStock += record.Quantity;
                record.Consumable.LockedStock -= record.Quantity;

                _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
                {
                    Id = Guid.NewGuid(),
                    ConsumableId = record.Consumable.Id,
                    ChangeType = "Unlock",
                    ChangeQuantity = record.Quantity,
                    BeforeStock = record.Consumable.AvailableStock - record.Quantity,
                    AfterStock = record.Consumable.AvailableStock,
                    ReferenceId = record.Id,
                    ReferenceNo = record.RecordNo,
                    OperatorId = approverId,
                    OperatorName = approverName,
                    Remark = $"领用驳回，库存解锁：{record.Quantity} {record.Consumable.Unit}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            record.Consumable.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOutRecordAsync(Guid id)
    {
        var record = await _dbContext.ConsumableOutRecords.FindAsync(id);
        if (record == null || record.IsDeleted) return false;

        if (record.Status == "Approved")
        {
            var consumable = await _dbContext.Consumables.FindAsync(record.ConsumableId);
            if (consumable != null)
            {
                consumable.CurrentStock += record.Quantity;
                consumable.UpdatedAt = DateTime.UtcNow;
            }
        }
        else if (record.Status == "Pending")
        {
            var consumable = await _dbContext.Consumables.FindAsync(record.ConsumableId);
            if (consumable != null)
            {
                consumable.AvailableStock += record.Quantity;
                consumable.LockedStock -= record.Quantity;
                consumable.UpdatedAt = DateTime.UtcNow;
            }
        }

        record.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 库存

    public async Task<ConsumableStockAdjustment> AdjustStockAsync(CreateConsumableStockAdjustmentRequest request, Guid operatorId, string operatorName)
    {
        var consumable = await _dbContext.Consumables.FindAsync(request.ConsumableId);
        if (consumable == null)
            throw new InvalidOperationException("耗材不存在");

        var beforeQty = consumable.CurrentStock;
        var afterQty = beforeQty + request.AdjustmentQuantity;

        if (afterQty < 0)
            throw new InvalidOperationException("调整后库存不能为负数");

        consumable.CurrentStock = afterQty;
        consumable.AvailableStock = afterQty;
        consumable.UpdatedAt = DateTime.UtcNow;

        var adjustment = new ConsumableStockAdjustment
        {
            Id = Guid.NewGuid(),
            ConsumableId = consumable.Id,
            AdjustmentType = request.AdjustmentType,
            BeforeQuantity = beforeQty,
            AdjustmentQuantity = request.AdjustmentQuantity,
            AfterQuantity = afterQty,
            Reason = request.Reason,
            OperatorId = operatorId,
            OperatorName = operatorName,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ConsumableStockAdjustments.Add(adjustment);

        _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
        {
            Id = Guid.NewGuid(),
            ConsumableId = consumable.Id,
            ChangeType = $"Adjust_{request.AdjustmentType}",
            ChangeQuantity = request.AdjustmentQuantity,
            BeforeStock = beforeQty,
            AfterStock = afterQty,
            ReferenceId = adjustment.Id,
            OperatorId = operatorId,
            OperatorName = operatorName,
            Remark = $"[{request.AdjustmentType}] {request.Reason}",
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return adjustment;
    }

    public async Task<int> StockCheckAsync(StockCheckRequest request, Guid operatorId, string operatorName)
    {
        int count = 0;
        foreach (var item in request.Items)
        {
            var consumable = await _dbContext.Consumables.FindAsync(item.ConsumableId);
            if (consumable == null) continue;

            var diff = item.ActualQuantity - consumable.CurrentStock;
            if (diff == 0) continue;

            consumable.CurrentStock = item.ActualQuantity;
            consumable.AvailableStock = item.ActualQuantity;
            consumable.UpdatedAt = DateTime.UtcNow;

            _dbContext.ConsumableStockAdjustments.Add(new ConsumableStockAdjustment
            {
                Id = Guid.NewGuid(),
                ConsumableId = consumable.Id,
                AdjustmentType = "盘点",
                BeforeQuantity = consumable.CurrentStock - diff,
                AdjustmentQuantity = diff,
                AfterQuantity = item.ActualQuantity,
                Reason = $"库存盘点，差异：{(diff > 0 ? "+" : "")}{diff}",
                OperatorId = operatorId,
                OperatorName = operatorName,
                CreatedAt = DateTime.UtcNow
            });

            _dbContext.ConsumableStockLogs.Add(new ConsumableStockLog
            {
                Id = Guid.NewGuid(),
                ConsumableId = consumable.Id,
                ChangeType = "Check",
                ChangeQuantity = diff,
                BeforeStock = consumable.CurrentStock - diff,
                AfterStock = item.ActualQuantity,
                OperatorId = operatorId,
                OperatorName = operatorName,
                Remark = $"库存盘点，实际：{item.ActualQuantity}",
                CreatedAt = DateTime.UtcNow
            });

            count++;
        }

        await _dbContext.SaveChangesAsync();
        return count;
    }

    public async Task<(List<ConsumableStockLogDto> Items, int Total)> GetStockLogsAsync(ConsumableStockLogQuery query)
    {
        var q = _dbContext.ConsumableStockLogs
            .Include(l => l.Consumable)
            .AsQueryable();

        if (query.ConsumableId.HasValue)
            q = q.Where(l => l.ConsumableId == query.ConsumableId);

        if (!string.IsNullOrEmpty(query.ChangeType))
            q = q.Where(l => l.ChangeType == query.ChangeType);

        if (query.StartDate.HasValue)
            q = q.Where(l => l.CreatedAt >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(l => l.CreatedAt <= query.EndDate.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(l => l.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items.Select(l => new ConsumableStockLogDto
        {
            Id = l.Id,
            ConsumableId = l.ConsumableId,
            ConsumableName = l.Consumable?.Name ?? "",
            ChangeType = l.ChangeType,
            ChangeQuantity = l.ChangeQuantity,
            BeforeStock = l.BeforeStock,
            AfterStock = l.AfterStock,
            ReferenceId = l.ReferenceId,
            ReferenceNo = l.ReferenceNo,
            OperatorId = l.OperatorId,
            OperatorName = l.OperatorName,
            Remark = l.Remark,
            CreatedAt = l.CreatedAt
        }).ToList(), total);
    }

    public async Task<(List<ConsumableStockAdjustmentDto> Items, int Total)> GetStockAdjustmentsAsync(ConsumableStockAdjustmentQuery query)
    {
        var q = _dbContext.ConsumableStockAdjustments
            .Include(a => a.Consumable)
            .AsQueryable();

        if (query.ConsumableId.HasValue)
            q = q.Where(a => a.ConsumableId == query.ConsumableId);

        if (!string.IsNullOrEmpty(query.AdjustmentType))
            q = q.Where(a => a.AdjustmentType == query.AdjustmentType);

        if (query.StartDate.HasValue)
            q = q.Where(a => a.CreatedAt >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(a => a.CreatedAt <= query.EndDate.Value);

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(a => a.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items.Select(a => new ConsumableStockAdjustmentDto
        {
            Id = a.Id,
            ConsumableId = a.ConsumableId,
            ConsumableName = a.Consumable?.Name ?? "",
            ConsumableCode = a.Consumable?.Code,
            AdjustmentType = a.AdjustmentType,
            BeforeQuantity = a.BeforeQuantity,
            AdjustmentQuantity = a.AdjustmentQuantity,
            AfterQuantity = a.AfterQuantity,
            Reason = a.Reason,
            OperatorId = a.OperatorId,
            OperatorName = a.OperatorName,
            CreatedAt = a.CreatedAt
        }).ToList(), total);
    }

    #endregion

    #region 统计

    public async Task<ConsumableStatisticsDto> GetStatisticsAsync(ConsumableStatisticsQuery? query)
    {
        var consumables = _dbContext.Consumables.Where(c => !c.IsDeleted).AsQueryable();

        if (query?.CategoryId.HasValue == true)
            consumables = consumables.Where(c => c.CategoryId == query.CategoryId);

        var allConsumables = await consumables.Include(c => c.Category).ToListAsync();

        var lowStockItems = allConsumables
            .Where(c => c.CurrentStock <= c.MinStock)
            .Select(c => new LowStockItem
            {
                ConsumableId = c.Id,
                ConsumableName = c.Name,
                CategoryName = c.Category?.Name,
                CurrentStock = c.CurrentStock,
                MinStock = c.MinStock,
                Unit = c.Unit
            }).ToList();

        var inQuery = _dbContext.ConsumableInRecords.Where(r => !r.IsDeleted && r.Status == "Approved").AsQueryable();
        var outQuery = _dbContext.ConsumableOutRecords.Where(r => !r.IsDeleted && r.Status == "Approved").AsQueryable();

        if (query?.StartDate.HasValue == true)
        {
            inQuery = inQuery.Where(r => r.InTime >= query.StartDate.Value);
            outQuery = outQuery.Where(r => r.OutTime >= query.StartDate.Value);
        }
        if (query?.EndDate.HasValue == true)
        {
            inQuery = inQuery.Where(r => r.InTime <= query.EndDate.Value);
            outQuery = outQuery.Where(r => r.OutTime <= query.EndDate.Value);
        }

        var inRecords = await inQuery.ToListAsync();
        var outRecords = await outQuery.ToListAsync();

        var monthlyOut = outRecords
            .GroupBy(r => r.OutTime.ToString("yyyy-MM"))
            .Select(g => new MonthlyConsumption
            {
                Month = g.Key,
                Quantity = g.Sum(r => r.Quantity),
                RecordCount = g.Count()
            })
            .OrderByDescending(m => m.Month)
            .Take(12)
            .ToList();

        return new ConsumableStatisticsDto
        {
            TotalTypes = allConsumables.Count,
            LowStockTypes = allConsumables.Count(c => c.CurrentStock <= c.MinStock),
            OutOfStockTypes = allConsumables.Count(c => c.CurrentStock == 0),
            ActiveTypes = allConsumables.Count(c => c.IsActive),
            TotalStockValue = allConsumables.AsEnumerable().Sum(c => c.CurrentStock * (c.UnitPrice ?? 0)),
            TotalInRecords = inRecords.Count,
            TotalOutRecords = outRecords.Count,
            TotalInAmount = inRecords.AsEnumerable().Sum(r => r.Quantity * (r.UnitPrice ?? 0)),
            TotalOutAmount = outRecords.AsEnumerable().Sum(r => r.Quantity),
            ByCategory = allConsumables
                .Where(c => c.Category != null)
                .GroupBy(c => c.Category!.Name)
                .ToDictionary(g => g.Key, g => g.Count()),
            LowStockItems = lowStockItems,
            MonthlyConsumptions = monthlyOut
        };
    }

    #endregion

    #region 通知

    public async Task<List<ConsumableNotificationDto>> GetNotificationsAsync(Guid userId, ConsumableNotificationQuery query)
    {
        var q = _dbContext.ConsumableNotifications
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (query.IsRead.HasValue)
            q = q.Where(n => n.IsRead == query.IsRead.Value);

        var items = await q
            .OrderByDescending(n => n.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return items.Select(n => new ConsumableNotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Type = n.Type,
            Title = n.Title,
            Content = n.Content,
            RelatedId = n.RelatedId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        return await _dbContext.ConsumableNotifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<bool> MarkNotificationReadAsync(Guid id, Guid userId)
    {
        var notification = await _dbContext.ConsumableNotifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null) return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task CheckLowStockAndNotifyAsync()
    {
        var lowStock = await _dbContext.Consumables
            .Where(c => !c.IsDeleted && c.IsActive && c.CurrentStock <= c.MinStock)
            .ToListAsync();

        if (!lowStock.Any()) return;

        var labAdmins = await _dbContext.UserRoles
            .Where(ur => ur.Role != null && ur.Role.Code == "lab_admin")
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync();

        foreach (var consumable in lowStock)
        {
            foreach (var adminId in labAdmins)
            {
                var exists = await _dbContext.ConsumableNotifications
                    .AnyAsync(n => n.UserId == adminId &&
                                   n.Type == "LowStock" &&
                                   n.RelatedId == consumable.Id &&
                                   n.CreatedAt > DateTime.UtcNow.AddHours(-24));

                if (!exists)
                {
                    _dbContext.ConsumableNotifications.Add(new ConsumableNotification
                    {
                        Id = Guid.NewGuid(),
                        UserId = adminId,
                        Type = "LowStock",
                        Title = $"耗材库存不足：{consumable.Name}",
                        Content = $"{consumable.Name}（{consumable.Code}）当前库存 {consumable.CurrentStock}{consumable.Unit}，低于最低库存 {consumable.MinStock}{consumable.Unit}",
                        RelatedId = consumable.Id,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Helpers

    private static string GenerateRecordNo(string prefix)
    {
        return $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    private static ConsumableInRecordDto MapInRecord(ConsumableInRecord r)
    {
        return new ConsumableInRecordDto
        {
            Id = r.Id,
            RecordNo = r.RecordNo,
            ConsumableId = r.ConsumableId,
            ConsumableName = r.Consumable?.Name ?? "",
            ConsumableCode = r.Consumable?.Code,
            Quantity = r.Quantity,
            UnitPrice = r.UnitPrice,
            Supplier = r.Supplier,
            InTime = r.InTime,
            HandlerId = r.HandlerId,
            HandlerName = r.HandlerName,
            Remark = r.Remark,
            Status = r.Status,
            ApprovedBy = r.ApprovedBy,
            ApproverName = r.ApproverName,
            ApprovedAt = r.ApprovedAt,
            ApprovalRemark = r.ApprovalRemark,
            CreatedAt = r.CreatedAt
        };
    }

    private static ConsumableOutRecordDto MapOutRecord(ConsumableOutRecord r)
    {
        return new ConsumableOutRecordDto
        {
            Id = r.Id,
            RecordNo = r.RecordNo,
            ConsumableId = r.ConsumableId,
            ConsumableName = r.Consumable?.Name ?? "",
            ConsumableCode = r.Consumable?.Code,
            CategoryName = r.Consumable?.Category?.Name,
            Quantity = r.Quantity,
            UsagePurpose = r.UsagePurpose,
            UsageLab = r.UsageLab,
            OutTime = r.OutTime,
            ApplicantId = r.ApplicantId,
            ApplicantName = r.ApplicantName,
            Remark = r.Remark,
            Status = r.Status,
            ApprovedBy = r.ApprovedBy,
            ApproverName = r.ApproverName,
            ApprovedAt = r.ApprovedAt,
            ApprovalRemark = r.ApprovalRemark,
            CreatedAt = r.CreatedAt
        };
    }

    #endregion
}
