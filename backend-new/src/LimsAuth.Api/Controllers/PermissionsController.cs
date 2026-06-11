using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:permission:read")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _permissionService.GetAllAsync();
        return StatusCode(result.Code, result);
    }

    [HttpGet("by-module")]
    [Authorize(Policy = "Permission:permission:read")]
    public async Task<IActionResult> GetByModule()
    {
        var result = await _permissionService.GetByModuleAsync();
        return StatusCode(result.Code, result);
    }
}
