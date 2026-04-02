using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Services;

/// <summary>
/// 部门服务
/// </summary>
public class DepartmentService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(AppDbContext dbContext, ILogger<DepartmentService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    public async Task<List<DepartmentDto>> GetDepartmentTreeAsync()
    {
        var departments = await _dbContext.Departments
            .Include(d => d.Manager)
            .Include(d => d.Parent)
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .ToListAsync();

        return BuildTree(departments, null);
    }

    /// <summary>
    /// 获取所有部门(扁平列表)
    /// </summary>
    public async Task<List<DepartmentBriefDto>> GetAllDepartmentsAsync()
    {
        return await _dbContext.Departments
            .Where(d => d.IsActive)
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .Select(d => new DepartmentBriefDto
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name
            })
            .ToListAsync();
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id)
    {
        var department = await _dbContext.Departments
            .Include(d => d.Manager)
            .Include(d => d.Parent)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null) return null;

        return MapToDto(department);
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        // 检查编码
        if (await _dbContext.Departments.AnyAsync(d => d.Code == request.Code))
        {
            return ApiResponse<DepartmentDto>.Error(400, "部门编码已存在");
        }

        // 验证父部门
        if (request.ParentId.HasValue)
        {
            var parentExists = await _dbContext.Departments.AnyAsync(d => d.Id == request.ParentId);
            if (!parentExists)
            {
                return ApiResponse<DepartmentDto>.Error(400, "父部门不存在");
            }
        }

        // 验证负责人
        if (request.ManagerId.HasValue)
        {
            var managerExists = await _dbContext.Users.AnyAsync(u => u.Id == request.ManagerId && u.IsActive);
            if (!managerExists)
            {
                return ApiResponse<DepartmentDto>.Error(400, "负责人不存在或已禁用");
            }
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            ParentId = request.ParentId,
            ManagerId = request.ManagerId,
            Description = request.Description,
            IsActive = true
        };

        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("创建部门成功: {Code}", department.Code);

        var result = await GetDepartmentByIdAsync(department.Id);
        return ApiResponse<DepartmentDto>.Success(result!, "部门创建成功");
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequest request)
    {
        var department = await _dbContext.Departments.FindAsync(id);
        if (department == null)
        {
            return ApiResponse<DepartmentDto>.Error(404, "部门不存在");
        }

        // 验证负责人
        if (request.ManagerId.HasValue)
        {
            var managerExists = await _dbContext.Users.AnyAsync(u => u.Id == request.ManagerId && u.IsActive);
            if (!managerExists)
            {
                return ApiResponse<DepartmentDto>.Error(400, "负责人不存在或已禁用");
            }
        }

        if (request.Name != null)
            department.Name = request.Name;
        if (request.ManagerId.HasValue)
            department.ManagerId = request.ManagerId;
        if (request.Description != null)
            department.Description = request.Description;
        if (request.IsActive.HasValue)
            department.IsActive = request.IsActive.Value;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("更新部门成功: {Code}", department.Code);

        var result = await GetDepartmentByIdAsync(id);
        return ApiResponse<DepartmentDto>.Success(result!, "部门更新成功");
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteDepartmentAsync(Guid id)
    {
        var department = await _dbContext.Departments
            .Include(d => d.Children)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return ApiResponse<bool>.Error(404, "部门不存在");
        }

        // 检查是否有子部门
        if (department.Children.Any())
        {
            return ApiResponse<bool>.Error(400, "该部门下存在子部门，不能删除");
        }

        // 检查是否有用户
        var hasUsers = await _dbContext.Users.AnyAsync(u => u.DepartmentId == id);
        if (hasUsers)
        {
            return ApiResponse<bool>.Error(400, "该部门下存在用户，不能删除");
        }

        _dbContext.Departments.Remove(department);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("删除部门成功: {Code}", department.Code);
        return ApiResponse<bool>.Success(true, "部门删除成功");
    }

    /// <summary>
    /// 构建部门树
    /// </summary>
    private List<DepartmentDto> BuildTree(List<Department> departments, Guid? parentId)
    {
        return departments
            .Where(d => d.ParentId == parentId)
            .Select(d =>
            {
                var dto = MapToDto(d);
                dto.Children = BuildTree(departments, d.Id);
                return dto;
            })
            .ToList();
    }

    /// <summary>
    /// 映射为DTO
    /// </summary>
    private DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Code = department.Code,
            Name = department.Name,
            ParentId = department.ParentId,
            ParentName = department.Parent?.Name,
            ManagerId = department.ManagerId,
            ManagerName = department.Manager?.FullName ?? department.Manager?.Username,
            Description = department.Description,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt,
            Children = new List<DepartmentDto>()
        };
    }
}
