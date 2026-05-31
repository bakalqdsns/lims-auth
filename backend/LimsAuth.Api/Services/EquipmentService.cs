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
            Status = e.Status,
            PurchaseDate = e.PurchaseDate,
            WarrantyMonths = e.WarrantyMonths,
            Price = e.Price,
            Location = e.Location,
            ImageUrl = e.ImageUrl,
            Instructions = e.Instructions,
            RequiresBooking = e.RequiresBooking,
            MaxBookingHours = e.MaxBookingHours,
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
            Status = equipment.Status,
            PurchaseDate = equipment.PurchaseDate,
            WarrantyMonths = equipment.WarrantyMonths,
            Price = equipment.Price,
            Location = equipment.Location,
            ImageUrl = equipment.ImageUrl,
            Instructions = equipment.Instructions,
            RequiresBooking = equipment.RequiresBooking,
            MaxBookingHours = equipment.MaxBookingHours,
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
            IsActive = true
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
        if (request.Status != null) equipment.Status = request.Status;
        if (request.PurchaseDate.HasValue) equipment.PurchaseDate = request.PurchaseDate.Value;
        if (request.WarrantyMonths.HasValue) equipment.WarrantyMonths = request.WarrantyMonths.Value;
        if (request.Price.HasValue) equipment.Price = request.Price.Value;
        if (request.Location != null) equipment.Location = request.Location;
        if (request.ImageUrl != null) equipment.ImageUrl = request.ImageUrl;
        if (request.Instructions != null) equipment.Instructions = request.Instructions;
        if (request.RequiresBooking.HasValue) equipment.RequiresBooking = request.RequiresBooking.Value;
        if (request.MaxBookingHours.HasValue) equipment.MaxBookingHours = request.MaxBookingHours.Value;
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
    public string Status { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool RequiresBooking { get; set; }
    public int? MaxBookingHours { get; set; }
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
