using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Authorization;
using LimsAuth.Api.Models.DTOs;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly DepartmentService _departmentService;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(DepartmentService departmentService, ILogger<DepartmentsController> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:department:read")]
    public async Task<IActionResult> GetDepartmentTree()
    {
        var result = await _departmentService.GetDepartmentTreeAsync();
        return Ok(ApiResponse<List<DepartmentDto>>.Success(result));
    }

    /// <summary>
    /// 获取所有部门(扁平列表)
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllDepartments()
    {
        var result = await _departmentService.GetAllDepartmentsAsync();
        return Ok(ApiResponse<List<DepartmentBriefDto>>.Success(result));
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:department:read")]
    public async Task<IActionResult> GetDepartment(Guid id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
        {
            return NotFound(ApiResponse<DepartmentDto>.Error(404, "部门不存在"));
        }
        return Ok(ApiResponse<DepartmentDto>.Success(department));
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Permission:department:create")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        var response = await _departmentService.CreateDepartmentAsync(request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:department:update")]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var response = await _departmentService.UpdateDepartmentAsync(id, request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:department:delete")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        var response = await _departmentService.DeleteDepartmentAsync(id);
        return StatusCode(response.Code, response);
    }
}
