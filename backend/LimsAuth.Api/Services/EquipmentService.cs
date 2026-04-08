using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Services;

public interface IEquipmentService
{
    Task<List<EquipmentDto>> GetListAsync(string? keyword = null, string? labId = null, string? category = null, string? status = null);
    Task<EquipmentDto?> GetByIdAsync(Guid id);
    Task<Equipment> CreateAsync(CreateEquipmentRequest request);
    Task<Equipment?> UpdateAsync(Guid id, UpdateEquipmentRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ToggleStatusAsync(Guid id, bool isActive);
    Task<bool> UpdateStatusAsync(Guid id, string status);
}

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _dbContext;

    public EquipmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<EquipmentDto>> GetListAsync(string? keyword = null, string? labId = null, string? category = null, string? status = null)
    {
        var query = _dbContext.Equipments
            .Include(e => e.Lab)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword) || (e.Model != null && e.Model.Contains(keyword)));
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

        var equipments = await query.OrderBy(e => e.Code).ToListAsync();

        return equipments.Select(e => new EquipmentDto
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
        var equipment = new Equipment
        {
            Code = request.Code,
            Name = request.Name,
            Model = request.Model,
            Manufacturer = request.Manufacturer,
            SerialNumber = request.SerialNumber,
            LabId = request.LabId,
            Category = request.Category,
            Status = request.Status,
            PurchaseDate = request.PurchaseDate,
            WarrantyMonths = request.WarrantyMonths,
            Price = request.Price,
            Location = request.Location,
            ImageUrl = request.ImageUrl,
            Instructions = request.Instructions,
            RequiresBooking = request.RequiresBooking,
            MaxBookingHours = request.MaxBookingHours,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.Equipments.Add(equipment);
        await _dbContext.SaveChangesAsync();
        return equipment;
    }

    public async Task<Equipment?> UpdateAsync(Guid id, UpdateEquipmentRequest request)
    {
        var equipment = await _dbContext.Equipments.FindAsync(id);
        if (equipment == null) return null;

        if (request.Code != null) equipment.Code = request.Code;
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
    public string Status { get; set; } = "正常";
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool RequiresBooking { get; set; } = false;
    public int? MaxBookingHours { get; set; }
    public string? Description { get; set; }
}

public class UpdateEquipmentRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? LabId { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public int? WarrantyMonths { get; set; }
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public bool? RequiresBooking { get; set; }
    public int? MaxBookingHours { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
