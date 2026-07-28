using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using OfficeOpenXml;

namespace LimsAuth.Api.Services;

public interface IEquipmentService
{
    Task<(List<EquipmentDto> Items, int Total)> GetListAsync(string? keyword = null, string? labId = null, string? category = null, string? status = null, int page = 1, int pageSize = 20);
    Task<EquipmentDto?> GetByIdAsync(Guid id);
    Task<Equipment> CreateAsync(CreateEquipmentRequest request);
    Task<Equipment?> UpdateAsync(Guid id, UpdateEquipmentRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
    Task<bool> UpdateStatusAsync(Guid id, string status);
    Task<EquipmentStatisticsDto> GetStatisticsAsync();
    Task<byte[]> ExportToExcelAsync(string? keyword = null, string? category = null, string? status = null);
    Task<ImportEquipmentResult> ImportExcelAsync(Stream fileStream, string fileName);
    Task<byte[]> GenerateImportTemplateAsync();
}

public class EquipmentStatisticsDto
{
    public int Total { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public int NormalCount { get; set; }
    public int MaintenanceCount { get; set; }
    public int BorrowedCount { get; set; }
    public int ScrappedCount { get; set; }
    public int RequiresBookingCount { get; set; }
    public decimal TotalValue { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByLab { get; set; } = new();
}

public class ImportEquipmentResult
{
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Total { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> DuplicateCodes { get; set; } = new();
}

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _dbContext;

    public EquipmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<EquipmentDto> Items, int Total)> GetListAsync(
        string? keyword = null, string? labId = null, string? category = null,
        string? status = null, int page = 1, int pageSize = 20)
    {
        var query = _dbContext.Equipments
            .Include(e => e.Lab)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(e =>
                e.Name.Contains(keyword) ||
                e.Code.Contains(keyword) ||
                (e.Model != null && e.Model.Contains(keyword)));
        }

        if (!string.IsNullOrEmpty(labId) && Guid.TryParse(labId, out var labGuid))
        {
            query = query.Where(e => e.LabId == labGuid);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(e => e.Category == category);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

        var total = await query.CountAsync();

        var equipments = await query
            .OrderBy(e => e.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = equipments.Select(e => new EquipmentDto
        {
            Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            Model = e.Model,
            Manufacturer = e.Manufacturer,
            SerialNumber = e.SerialNumber,
            LabId = e.LabId,
            LabName = e.Lab?.Name,
            Category = e.Category,
            Unit = e.Unit,
            Brand = e.Brand,
            Supplier = e.Supplier,
            Status = e.Status,
            PurchaseDate = e.PurchaseDate,
            WarrantyMonths = e.WarrantyMonths,
            Price = e.Price,
            Location = e.Location,
            ImageUrl = e.ImageUrl,
            Instructions = e.Instructions,
            RequiresBooking = e.RequiresBooking,
            MaxBookingHours = e.MaxBookingHours,
            TotalQuantity = e.TotalQuantity,
            AvailableQuantity = e.AvailableQuantity,
            Description = e.Description,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        }).ToList();

        return (items, total);
    }

    public async Task<EquipmentDto?> GetByIdAsync(Guid id)
    {
        var equipment = await _dbContext.Equipments
            .Include(e => e.Lab)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (equipment == null) return null;

        return new EquipmentDto
        {
            Id = equipment.Id,
            Code = equipment.Code,
            Name = equipment.Name,
            Model = equipment.Model,
            Manufacturer = equipment.Manufacturer,
            SerialNumber = equipment.SerialNumber,
            LabId = equipment.LabId,
            LabName = equipment.Lab?.Name,
            Category = equipment.Category,
            Unit = equipment.Unit,
            Brand = equipment.Brand,
            Supplier = equipment.Supplier,
            Status = equipment.Status,
            PurchaseDate = equipment.PurchaseDate,
            WarrantyMonths = equipment.WarrantyMonths,
            Price = equipment.Price,
            Location = equipment.Location,
            ImageUrl = equipment.ImageUrl,
            Instructions = equipment.Instructions,
            RequiresBooking = equipment.RequiresBooking,
            MaxBookingHours = equipment.MaxBookingHours,
            TotalQuantity = equipment.TotalQuantity,
            AvailableQuantity = equipment.AvailableQuantity,
            Description = equipment.Description,
            IsActive = equipment.IsActive,
            CreatedAt = equipment.CreatedAt
        };
    }

    public async Task<Equipment> CreateAsync(CreateEquipmentRequest request)
    {
        var exists = await _dbContext.Equipments.AnyAsync(e => e.Code == request.Code);
        if (exists)
            throw new InvalidOperationException($"设备代码 {request.Code} 已存在");

        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Model = request.Model,
            Manufacturer = request.Manufacturer,
            SerialNumber = request.SerialNumber,
            LabId = request.LabId,
            Category = request.Category,
            Unit = request.Unit ?? "台",
            Status = request.Status,
            PurchaseDate = request.PurchaseDate,
            WarrantyMonths = request.WarrantyMonths,
            Price = request.Price,
            Location = request.Location,
            ImageUrl = request.ImageUrl,
            Instructions = request.Instructions,
            RequiresBooking = request.RequiresBooking,
            MaxBookingHours = request.MaxBookingHours,
            TotalQuantity = request.TotalQuantity,
            AvailableQuantity = request.AvailableQuantity,
            Brand = request.Brand,
            Supplier = request.Supplier,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Equipments.Add(equipment);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"数据库保存失败: {inner}");
        }
        return equipment;
    }

    public async Task<Equipment?> UpdateAsync(Guid id, UpdateEquipmentRequest request)
    {
        var equipment = await _dbContext.Equipments.FindAsync(id);
        if (equipment == null) return null;

        if (request.Name != null) equipment.Name = request.Name;
        if (request.Model != null) equipment.Model = request.Model;
        if (request.Manufacturer != null) equipment.Manufacturer = request.Manufacturer;
        if (request.SerialNumber != null) equipment.SerialNumber = request.SerialNumber;
        if (request.LabId.HasValue) equipment.LabId = request.LabId.Value;
        if (request.Category != null) equipment.Category = request.Category;
        if (request.Unit != null) equipment.Unit = request.Unit;
        if (request.Brand != null) equipment.Brand = request.Brand;
        if (request.Supplier != null) equipment.Supplier = request.Supplier;
        if (request.Status != null) equipment.Status = request.Status;
        if (request.PurchaseDate.HasValue) equipment.PurchaseDate = request.PurchaseDate.Value;
        if (request.WarrantyMonths.HasValue) equipment.WarrantyMonths = request.WarrantyMonths.Value;
        if (request.Price.HasValue) equipment.Price = request.Price.Value;
        if (request.Location != null) equipment.Location = request.Location;
        if (request.ImageUrl != null) equipment.ImageUrl = request.ImageUrl;
        if (request.Instructions != null) equipment.Instructions = request.Instructions;
        if (request.RequiresBooking.HasValue) equipment.RequiresBooking = request.RequiresBooking.Value;
        if (request.MaxBookingHours.HasValue) equipment.MaxBookingHours = request.MaxBookingHours.Value;
        if (request.TotalQuantity.HasValue) equipment.TotalQuantity = request.TotalQuantity.Value;
        if (request.AvailableQuantity.HasValue) equipment.AvailableQuantity = request.AvailableQuantity.Value;
        if (request.Description != null) equipment.Description = request.Description;
        if (request.IsActive.HasValue) equipment.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();
        return equipment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var equipment = await _dbContext.Equipments.FindAsync(id);
        if (equipment == null) return false;

        _dbContext.Equipments.Remove(equipment);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(Guid id, bool isActive)
    {
        var equipment = await _dbContext.Equipments.FindAsync(id);
        if (equipment == null) return false;

        equipment.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        var equipment = await _dbContext.Equipments.FindAsync(id);
        if (equipment == null) return false;

        equipment.Status = status;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<EquipmentStatisticsDto> GetStatisticsAsync()
    {
        var allList = await _dbContext.Equipments.ToListAsync();
        var stats = new EquipmentStatisticsDto
        {
            Total = allList.Count,
            ActiveCount = allList.Count(e => e.IsActive),
            InactiveCount = allList.Count(e => !e.IsActive),
            NormalCount = allList.Count(e => e.Status == "在库-可用"),
            MaintenanceCount = allList.Count(e => e.Status == "在库-待维修"),
            BorrowedCount = allList.Count(e => e.Status == "借出"),
            ScrappedCount = allList.Count(e => e.Status == "报废"),
            RequiresBookingCount = allList.Count(e => e.RequiresBooking),
            TotalValue = allList.Where(e => e.Price.HasValue).Sum(e => e.Price!.Value)
        };

        stats.ByCategory = allList
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        stats.ByStatus = allList
            .GroupBy(e => e.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        stats.ByLab = allList
            .Where(e => e.Lab != null)
            .GroupBy(e => e.Lab!.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        return stats;
    }

    public async Task<byte[]> ExportToExcelAsync(string? keyword = null, string? category = null, string? status = null)
    {
        var query = _dbContext.Equipments.Include(e => e.Lab).AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(e => e.Category == category);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(e => e.Status == status);

        var data = await query.OrderBy(e => e.Code).ToListAsync();

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("设备台账");

        var headers = new[] { "设备代码", "设备名称", "型号", "制造商", "序列号", "分类", "状态",
            "所属实验室", "存放位置", "购买日期", "保修期(月)", "价格(元)", "需预约", "最大预约时长(h)", "描述", "是否启用" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[1, i + 1].Value = headers[i];
            ws.Cells[1, i + 1].Style.Font.Bold = true;
            ws.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        for (int i = 0; i < data.Count; i++)
        {
            var e = data[i];
            ws.Cells[i + 2, 1].Value = e.Code;
            ws.Cells[i + 2, 2].Value = e.Name;
            ws.Cells[i + 2, 3].Value = e.Model ?? "";
            ws.Cells[i + 2, 4].Value = e.Manufacturer ?? "";
            ws.Cells[i + 2, 5].Value = e.SerialNumber ?? "";
            ws.Cells[i + 2, 6].Value = e.Category;
            ws.Cells[i + 2, 7].Value = e.Status;
            ws.Cells[i + 2, 8].Value = e.Lab?.Name ?? "未分配";
            ws.Cells[i + 2, 9].Value = e.Location ?? "";
            ws.Cells[i + 2, 10].Value = e.PurchaseDate?.ToString("yyyy-MM-dd") ?? "";
            ws.Cells[i + 2, 11].Value = e.WarrantyMonths ?? 0;
            ws.Cells[i + 2, 12].Value = e.Price.HasValue ? e.Price.Value.ToString("F2") : "";
            ws.Cells[i + 2, 13].Value = e.RequiresBooking ? "是" : "否";
            ws.Cells[i + 2, 14].Value = e.MaxBookingHours ?? 0;
            ws.Cells[i + 2, 15].Value = e.Description ?? "";
            ws.Cells[i + 2, 16].Value = e.IsActive ? "启用" : "停用";
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }

    public async Task<ImportEquipmentResult> ImportExcelAsync(Stream fileStream, string fileName)
    {
        var result = new ImportEquipmentResult();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        package.Load(fileStream);

        var ws = package.Workbook.Worksheets[0];
        var wsDimension = ws.Dimension;
        if (wsDimension == null)
        {
            result.Errors.Add("Excel 工作表无效或为空");
            return result;
        }
        var rowCount = wsDimension.Rows;

        if (rowCount < 2)
        {
            result.Errors.Add("Excel 文件为空或没有数据行");
            return result;
        }

        var codeColMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int c = 1; c <= wsDimension.Columns; c++)
        {
            var header = ws.Cells[1, c].Text?.Trim().TrimEnd('*').Trim();
            if (!string.IsNullOrEmpty(header))
            {
                var normalized = NormalizeHeader(header);
                codeColMap[header] = c;
                if (!codeColMap.ContainsKey(normalized))
                    codeColMap[normalized] = c;
            }
        }

        string[] requiredHeaders = { "资产编号", "设备名称" };
        foreach (var rh in requiredHeaders)
        {
            if (!codeColMap.ContainsKey(rh))
            {
                result.Errors.Add($"缺少必需列：「{rh}」");
                return result;
            }
        }

        var existingCodes = await _dbContext.Equipments
            .Select(e => e.Code)
            .ToListAsync();
        var existingCodeSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);

        var labNames = await _dbContext.Labs
            .Select(l => l.Name)
            .ToListAsync();
        var labNameSet = new HashSet<string>(labNames, StringComparer.OrdinalIgnoreCase);

        var validStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "在库-可用", "在库-待维修", "在库-已预约", "借出", "送修", "报废", "丢失"
        };
        var validCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "通用设备", "计算机设备", "实验仪器", "测量设备", "办公设备", "安全设备", "其他"
        };

        int codeCol = codeColMap["资产编号"];
        int nameCol = codeColMap.ContainsKey("设备名称") ? codeColMap["设备名称"] : -1;
        int modelCol = codeColMap.ContainsKey("型号") ? codeColMap["型号"] : -1;
        int manufacturerCol = codeColMap.ContainsKey("制造商") ? codeColMap["制造商"] : -1;
        int brandCol = codeColMap.ContainsKey("品牌") ? codeColMap["品牌"] : -1;
        int snCol = codeColMap.ContainsKey("序列号") ? codeColMap["序列号"] : -1;
        int catCol = codeColMap.ContainsKey("类别") ? codeColMap["类别"] : -1;
        int unitCol = codeColMap.ContainsKey("单位") ? codeColMap["单位"] : -1;
        int statusCol = codeColMap.ContainsKey("状态") ? codeColMap["状态"] : -1;
        int priceCol = codeColMap.ContainsKey("价格") ? codeColMap["价格"] : -1;
        int dateCol = codeColMap.ContainsKey("购入日期") ? codeColMap["购入日期"] : -1;
        int warrantyCol = codeColMap.ContainsKey("保修期(月)") ? codeColMap["保修期(月)"] : -1;
        int supplierCol = codeColMap.ContainsKey("供应商") ? codeColMap["供应商"] : -1;
        int locationCol = codeColMap.ContainsKey("存放位置") ? codeColMap["存放位置"] : -1;
        int labNameCol = codeColMap.ContainsKey("所属实验室名称") ? codeColMap["所属实验室名称"] : -1;
        int bookingCol = codeColMap.ContainsKey("是否预约(是/否)") ? codeColMap["是否预约(是/否)"] : -1;
        int maxBookingCol = codeColMap.ContainsKey("最大预约时长(h)") ? codeColMap["最大预约时长(h)"] : -1;
        int totalQuantityCol = codeColMap.ContainsKey("总数量") ? codeColMap["总数量"] : -1;
        int availableQuantityCol = codeColMap.ContainsKey("可用数量") ? codeColMap["可用数量"] : -1;
        int descriptionCol = codeColMap.ContainsKey("描述") ? codeColMap["描述"] : -1;

        var toInsert = new List<Equipment>();

        for (int r = 2; r <= rowCount; r++)
        {
            var firstCellText = ws.Cells[r, 1].Text?.Trim() ?? "";
            if (firstCellText.StartsWith("状态可选值", StringComparison.OrdinalIgnoreCase)
             || firstCellText.StartsWith("类别可选值", StringComparison.OrdinalIgnoreCase)
             || firstCellText.StartsWith("提示", StringComparison.OrdinalIgnoreCase))
                continue;

            var code = ws.Cells[r, codeCol].Text?.Trim() ?? "";
            var name = nameCol > 0 ? (ws.Cells[r, nameCol].Text?.Trim() ?? "") : "";

            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name))
                continue;

            var rowErrors = new List<string>();

            if (string.IsNullOrEmpty(code))
                rowErrors.Add("资产编号为空");
            if (string.IsNullOrEmpty(name))
                rowErrors.Add("设备名称为空");

            if (existingCodeSet.Contains(code) || toInsert.Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                rowErrors.Add($"资产编号「{code}」已存在");
                result.DuplicateCodes.Add(code);
            }

            Guid? labId = null;
            if (labNameCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, labNameCol].Text?.Trim()))
            {
                var labName = ws.Cells[r, labNameCol].Text!.Trim();
                var matchedLab = labNames
                    .Where(n => n.Equals(labName, StringComparison.OrdinalIgnoreCase)
                             || labName.Contains(n, StringComparison.OrdinalIgnoreCase)
                             || n.Contains(labName, StringComparison.OrdinalIgnoreCase))
                    .Select(n => _dbContext.Labs.FirstOrDefault(l => l.Name == n))
                    .FirstOrDefault(m => m != null);
                if (matchedLab != null)
                    labId = matchedLab.Id;
            }

            string category = "通用设备";
            if (catCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, catCol].Text?.Trim()))
            {
                var rawCat = ws.Cells[r, catCol].Text!.Trim();
                category = validCategories.Contains(rawCat) ? rawCat : "通用设备";
            }

            string status = "在库-可用";
            if (statusCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, statusCol].Text?.Trim()))
            {
                var rawStatus = ws.Cells[r, statusCol].Text!.Trim();
                status = validStatuses.Contains(rawStatus) ? rawStatus : "在库-可用";
            }

            DateTime? purchaseDate = null;
            if (dateCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, dateCol].Text?.Trim()))
            {
                if (DateTime.TryParse(ws.Cells[r, dateCol].Text!.Trim(), out var pd))
                    purchaseDate = pd;
                else
                    rowErrors.Add("购入日期格式错误（应为 yyyy-MM-dd）");
            }

            decimal? price = null;
            if (priceCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, priceCol].Text?.Trim()))
            {
                if (decimal.TryParse(ws.Cells[r, priceCol].Text!.Trim(), out var p))
                    price = p;
            }

            int warrantyMonths = 0;
            if (warrantyCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, warrantyCol].Text?.Trim()))
            {
                int.TryParse(ws.Cells[r, warrantyCol].Text!.Trim(), out warrantyMonths);
            }

            bool requiresBooking = false;
            if (bookingCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, bookingCol].Text?.Trim()))
            {
                var rawBooking = ws.Cells[r, bookingCol].Text!.Trim();
                requiresBooking = rawBooking == "是" || rawBooking == "1" || rawBooking.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            int maxBookingHours = 0;
            if (maxBookingCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, maxBookingCol].Text?.Trim()))
            {
                int.TryParse(ws.Cells[r, maxBookingCol].Text!.Trim(), out maxBookingHours);
            }

            int totalQty = 1;
            if (totalQuantityCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, totalQuantityCol].Text?.Trim()))
            {
                int.TryParse(ws.Cells[r, totalQuantityCol].Text!.Trim(), out totalQty);
                if (totalQty < 1) totalQty = 1;
            }

            int availQty = totalQty;
            if (availableQuantityCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, availableQuantityCol].Text?.Trim()))
            {
                int.TryParse(ws.Cells[r, availableQuantityCol].Text!.Trim(), out availQty);
                if (availQty < 0) availQty = 0;
            }

            toInsert.Add(new Equipment
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                Model = modelCol > 0 ? ws.Cells[r, modelCol].Text?.Trim() : null,
                Manufacturer = manufacturerCol > 0 ? ws.Cells[r, manufacturerCol].Text?.Trim() : null,
                Brand = brandCol > 0 ? ws.Cells[r, brandCol].Text?.Trim() : null,
                SerialNumber = snCol > 0 ? ws.Cells[r, snCol].Text?.Trim() : null,
                Category = category,
                Unit = unitCol > 0 && !string.IsNullOrEmpty(ws.Cells[r, unitCol].Text?.Trim())
                    ? ws.Cells[r, unitCol].Text!.Trim() : "台",
                Status = status,
                Price = price,
                PurchaseDate = purchaseDate,
                WarrantyMonths = warrantyMonths > 0 ? warrantyMonths : null,
                Supplier = supplierCol > 0 ? ws.Cells[r, supplierCol].Text?.Trim() : null,
                Location = locationCol > 0 ? ws.Cells[r, locationCol].Text?.Trim() : null,
                LabId = labId,
                RequiresBooking = requiresBooking,
                MaxBookingHours = maxBookingHours > 0 ? maxBookingHours : null,
                TotalQuantity = totalQty,
                AvailableQuantity = availQty,
                Description = descriptionCol > 0 ? ws.Cells[r, descriptionCol].Text?.Trim() : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            existingCodeSet.Add(code);
            result.Total++;
        }

        if (toInsert.Count > 0)
        {
            _dbContext.Equipments.AddRange(toInsert);
            try
            {
                await _dbContext.SaveChangesAsync();
                result.Success = toInsert.Count;
            }
            catch (DbUpdateException ex)
            {
                result.Errors.Add("批量保存失败: " + (ex.InnerException?.Message ?? ex.Message));
                result.Failed = toInsert.Count;
                result.Success = 0;
            }
        }

        return result;
    }

    public Task<byte[]> GenerateImportTemplateAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("设备导入模板");

        var headers = new[]
        {
            "资产编号*", "设备名称*", "型号", "制造商", "品牌", "序列号",
            "类别", "单位", "状态", "价格", "购入日期",
            "保修期(月)", "供应商", "存放位置", "所属实验室名称",
            "是否预约(是/否)", "最大预约时长(h)", "总数量", "可用数量", "描述"
        };
        var requiredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "资产编号*", "设备名称*" };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[1, i + 1].Value = headers[i];
            ws.Cells[1, i + 1].Style.Font.Bold = true;
            ws.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            ws.Column(i + 1).Width = 18;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }

    private static string NormalizeHeader(string header)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["购置日期"] = "购入日期",
            ["购入日期"] = "购入日期",
            ["购买日期"] = "购入日期",
            ["单价(元)"] = "价格",
            ["单价"] = "价格",
            ["金额"] = "价格",
            ["保修期"] = "保修期(月)",
            ["保修期(月)"] = "保修期(月)",
            ["保修"] = "保修期(月)",
            ["资产编号"] = "资产编号",
            ["设备名称"] = "设备名称",
            ["型号"] = "型号",
            ["制造商"] = "制造商",
            ["品牌"] = "品牌",
            ["序列号"] = "序列号",
            ["类别"] = "类别",
            ["单位"] = "单位",
            ["状态"] = "状态",
            ["供应商"] = "供应商",
            ["存放位置"] = "存放位置",
            ["所属实验室名称"] = "所属实验室名称",
            ["实验室"] = "所属实验室名称",
            ["是否预约(是/否)"] = "是否预约(是/否)",
            ["最大预约时长(h)"] = "最大预约时长(h)",
            ["总数量"] = "总数量",
            ["可用数量"] = "可用数量",
            ["描述"] = "描述",
            ["备注"] = "描述",
        };
        return map.TryGetValue(header, out var normalized) ? normalized : header;
    }
}

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? LabId { get; set; }
    public string? LabName { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? Brand { get; set; }
    public string? Supplier { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool RequiresBooking { get; set; }
    public int? MaxBookingHours { get; set; }
    public int TotalQuantity { get; set; } = 1;
    public int AvailableQuantity { get; set; } = 1;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEquipmentRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? LabId { get; set; }
    public string Category { get; set; } = "通用设备";
    public string? Unit { get; set; }
    public string Status { get; set; } = "在库-可用";
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool RequiresBooking { get; set; } = false;
    public int? MaxBookingHours { get; set; }
    public int TotalQuantity { get; set; } = 1;
    public int AvailableQuantity { get; set; } = 1;
    public string? Brand { get; set; }
    public string? Supplier { get; set; }
    public string? Description { get; set; }
}

public class UpdateEquipmentRequest
{
    public string? Name { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? LabId { get; set; }
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public string? Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool? RequiresBooking { get; set; }
    public int? MaxBookingHours { get; set; }
    public int? TotalQuantity { get; set; }
    public int? AvailableQuantity { get; set; }
    public string? Brand { get; set; }
    public string? Supplier { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
