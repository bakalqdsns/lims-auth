using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Authorization;
using LimsAuth.Api.Models.DTOs;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly RoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:role:read")]
    public async Task<IActionResult> GetRoles([FromQuery] RoleListQuery query)
    {
        var result = await _roleService.GetRolesAsync(query);
        return Ok(ApiResponse<PagedResponse<RoleDto>>.Success(result));
    }

    /// <summary>
    /// 获取所有角色(用于下拉选择)
    /// </summary>
    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _roleService.GetAllRolesAsync();
        return Ok(ApiResponse<List<RoleBriefDto>>.Success(result));
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:role:read")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(ApiResponse<RoleDetailDto>.Error(404, "角色不存在"));
        }
        return Ok(ApiResponse<RoleDetailDto>.Success(role));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Permission:role:create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var response = await _roleService.CreateRoleAsync(request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:role:update")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var response = await _roleService.UpdateRoleAsync(id, request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:role:delete")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 分配权限
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    [Authorize(Policy = "Permission:permission:assign")]
    public async Task<IActionResult> UpdateRolePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request)
    {
        var response = await _roleService.UpdateRolePermissionsAsync(id, request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 获取角色下的用户
    /// </summary>
    [HttpGet("{id:guid}/users")]
    [Authorize(Policy = "Permission:role:read")]
    public async Task<IActionResult> GetRoleUsers(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(ApiResponse<List<UserListItemDto>>.Error(404, "角色不存在"));
        }

        var users = await _roleService.GetRoleUsersAsync(id);
        return Ok(ApiResponse<List<UserListItemDto>>.Success(users));
    }
}
