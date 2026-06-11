using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface IBuildingService
{
    Task<ApiResponse<PagedResponse<BuildingDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<BuildingDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<BuildingDto>> CreateAsync(CreateBuildingRequest request);
    Task<ApiResponse<BuildingDto>> UpdateAsync(Guid id, UpdateBuildingRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class BuildingService : IBuildingService
{
    private readonly AppDbContext _db;
    public BuildingService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<BuildingDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.VenBuildings.Include(b => b.Institution).Where(b => b.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(b => b.Name.Contains(query.Keyword) || b.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(b => b.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(b => new BuildingDto
            {
                Id = b.Id, Code = b.Code, Name = b.Name, NameEn = b.NameEn,
                InstitutionId = b.InstitutionId, InstitutionName = b.Institution != null ? b.Institution.Name : null,
                Address = b.Address, TotalFloors = b.TotalFloors, Area = b.Area,
                BuildYear = b.BuildYear, UseType = b.UseType, Status = b.Status,
                Description = b.Description, CreatedAt = b.CreatedAt, RoomCount = 0
            }).ToListAsync();
        return ApiResponse<PagedResponse<BuildingDto>>.Success(new PagedResponse<BuildingDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<BuildingDto>> GetByIdAsync(Guid id)
    {
        var b = await _db.VenBuildings.Include(x => x.Institution).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return ApiResponse<BuildingDto>.Error(404, "楼宇不存在");
        return ApiResponse<BuildingDto>.Success(new BuildingDto { Id = b.Id, Code = b.Code, Name = b.Name, Status = b.Status });
    }

    public async Task<ApiResponse<BuildingDto>> CreateAsync(CreateBuildingRequest request)
    {
        var b = new VenBuilding
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, NameEn = request.NameEn,
            InstitutionId = request.InstitutionId, Address = request.Address,
            TotalFloors = request.TotalFloors, Area = request.Area, BuildYear = request.BuildYear,
            UseType = request.UseType, SortOrder = request.SortOrder,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.VenBuildings.Add(b);
        await _db.SaveChangesAsync();
        return ApiResponse<BuildingDto>.Success(new BuildingDto { Id = b.Id, Code = b.Code, Name = b.Name }, "楼宇创建成功");
    }

    public async Task<ApiResponse<BuildingDto>> UpdateAsync(Guid id, UpdateBuildingRequest request)
    {
        var b = await _db.VenBuildings.FindAsync(id);
        if (b == null) return ApiResponse<BuildingDto>.Error(404, "楼宇不存在");
        if (!string.IsNullOrEmpty(request.Name)) b.Name = request.Name;
        if (request.Status.HasValue) b.Status = request.Status.Value;
        b.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<BuildingDto>.Success(new BuildingDto { Id = b.Id, Code = b.Code, Name = b.Name }, "楼宇更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var b = await _db.VenBuildings.FindAsync(id);
        if (b == null) return ApiResponse.Error(404, "楼宇不存在");
        b.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("楼宇删除成功");
    }
}

public interface IRoomService
{
    Task<ApiResponse<PagedResponse<RoomDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<RoomDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<RoomDto>> CreateAsync(CreateRoomRequest request);
    Task<ApiResponse<RoomDto>> UpdateAsync(Guid id, UpdateRoomRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
}

public class RoomService : IRoomService
{
    private readonly AppDbContext _db;
    public RoomService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<RoomDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.LabRooms.Include(r => r.Building).Where(r => r.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(r => r.Name.Contains(query.Keyword) || r.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(r => new RoomDto
            {
                Id = r.Id, Code = r.Code, Name = r.Name, BuildingId = r.BuildingId,
                BuildingName = r.Building != null ? r.Building.Name : null,
                FloorNo = r.FloorNo, RoomNumber = r.RoomNumber, SeatCount = r.SeatCount, Area = r.Area,
                RoomType = r.RoomType, IsAvailable = r.IsAvailable, Status = r.Status,
                Description = r.Description, CreatedAt = r.CreatedAt
            }).ToListAsync();
        return ApiResponse<PagedResponse<RoomDto>>.Success(new PagedResponse<RoomDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<RoomDto>> GetByIdAsync(Guid id)
    {
        var r = await _db.LabRooms.Include(x => x.Building).FirstOrDefaultAsync(x => x.Id == id);
        if (r == null) return ApiResponse<RoomDto>.Error(404, "实验室不存在");
        return ApiResponse<RoomDto>.Success(new RoomDto { Id = r.Id, Code = r.Code, Name = r.Name, Status = r.Status });
    }

    public async Task<ApiResponse<RoomDto>> CreateAsync(CreateRoomRequest request)
    {
        var r = new LabRoom
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, BuildingId = request.BuildingId,
            FloorNo = request.FloorNo, RoomNumber = request.RoomNumber, SeatCount = request.SeatCount,
            Area = request.Area, RoomType = request.RoomType, Photo = request.Photo,
            IsAvailable = request.IsAvailable, SortOrder = request.SortOrder,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.LabRooms.Add(r);
        await _db.SaveChangesAsync();
        return ApiResponse<RoomDto>.Success(new RoomDto { Id = r.Id, Code = r.Code, Name = r.Name }, "实验室创建成功");
    }

    public async Task<ApiResponse<RoomDto>> UpdateAsync(Guid id, UpdateRoomRequest request)
    {
        var r = await _db.LabRooms.FindAsync(id);
        if (r == null) return ApiResponse<RoomDto>.Error(404, "实验室不存在");
        if (!string.IsNullOrEmpty(request.Name)) r.Name = request.Name;
        if (request.Status.HasValue) r.Status = request.Status.Value;
        r.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<RoomDto>.Success(new RoomDto { Id = r.Id, Code = r.Code, Name = r.Name }, "实验室更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var r = await _db.LabRooms.FindAsync(id);
        if (r == null) return ApiResponse.Error(404, "实验室不存在");
        r.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("实验室删除成功");
    }
}
