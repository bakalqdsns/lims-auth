using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LimsAuth.Api.Authorization;
using LimsAuth.Api.Models.DTOs;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetUsers([FromQuery] UserListQuery query)
    {
        var result = await _userService.GetUsersAsync(query);
        return Ok(ApiResponse<PagedResponse<UserListItemDto>>.Success(result));
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDetailDto>.Error(404, "用户不存在"));
        }
        return Ok(ApiResponse<UserDetailDto>.Success(user));
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Permission:user:create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = await _userService.CreateUserAsync(request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:user:update")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var response = await _userService.UpdateUserAsync(id, request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:user:delete")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _userService.DeleteUserAsync(id);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 更新用户状态
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Permission:user:update")]
    public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] bool isActive)
    {
        var response = await _userService.UpdateUserStatusAsync(id, isActive);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 分配角色
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    [Authorize(Policy = "Permission:role:assign")]
    public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UpdateUserRolesRequest request)
    {
        var response = await _userService.UpdateUserRolesAsync(id, request);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    [HttpPut("{id:guid}/password")]
    [Authorize(Policy = "Permission:user:reset_password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
    {
        var response = await _userService.ResetPasswordAsync(id, request.NewPassword);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 修改自己的密码
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<bool>.Error(401, "未授权"));
        }

        var response = await _userService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);
        return StatusCode(response.Code, response);
    }

    /// <summary>
    /// 获取当前用户权限
    /// </summary>
    [HttpGet("permissions/my")]
    [Authorize]
    public async Task<IActionResult> GetMyPermissions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse<List<string>>.Error(401, "未授权"));
        }

        var permissions = await _userService.GetUserPermissionsAsync(userId);
        return Ok(ApiResponse<List<string>>.Success(permissions));
    }
}
