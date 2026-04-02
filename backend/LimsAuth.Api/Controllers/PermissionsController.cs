using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Authorization;
using LimsAuth.Api.Models.DTOs;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

/// <summary>
/// 权限管理控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly PermissionService _permissionService;

    public PermissionsController(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// 获取所有权限
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:permission:read")]
    public async Task<IActionResult> GetAllPermissions()
    {
        var result = await _permissionService.GetAllPermissionsAsync();
        return Ok(ApiResponse<List<PermissionDto>>.Success(result));
    }

    /// <summary>
    /// 按模块分组获取权限
    /// </summary>
    [HttpGet("by-module")]
    [Authorize(Policy = "Permission:permission:read")]
    public async Task<IActionResult> GetPermissionsByModule()
    {
        var result = await _permissionService.GetPermissionsByModuleAsync();
        return Ok(ApiResponse<List<PermissionModuleDto>>.Success(result));
    }

    /// <summary>
    /// 获取权限模块列表
    /// </summary>
    [HttpGet("modules")]
    [Authorize(Policy = "Permission:permission:read")]
    public IActionResult GetPermissionModules()
    {
        var result = _permissionService.GetPermissionModules();
        return Ok(ApiResponse<List<PermissionModuleDto>>.Success(result));
    }
}
