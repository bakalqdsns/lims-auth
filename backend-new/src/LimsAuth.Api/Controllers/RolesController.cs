using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:role:read")]
    public async Task<IActionResult> GetList([FromQuery] RoleQueryRequest query)
    {
        var result = await _roleService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:role:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:role:create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        var result = await _roleService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:role:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:role:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roleService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}/permissions")]
    [Authorize(Policy = "Permission:permission:assign")]
    public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request)
    {
        var result = await _roleService.UpdatePermissionsAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
