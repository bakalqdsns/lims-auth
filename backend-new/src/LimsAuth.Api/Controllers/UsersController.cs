using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetList([FromQuery] UserQueryRequest query)
    {
        var result = await _userService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:user:create")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:user:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:user:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}/roles")]
    [Authorize(Policy = "Permission:user:update")]
    public async Task<IActionResult> UpdateRoles(Guid id, [FromBody] UpdateUserRolesRequest request)
    {
        var result = await _userService.UpdateRolesAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}/password")]
    [Authorize(Policy = "Permission:user:update")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
    {
        var result = await _userService.ResetPasswordAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}/roles")]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetRoles(Guid id)
    {
        var result = await _userService.GetRolesAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}/permissions")]
    [Authorize(Policy = "Permission:user:read")]
    public async Task<IActionResult> GetPermissions(Guid id)
    {
        var result = await _userService.GetPermissionsAsync(id);
        return StatusCode(result.Code, result);
    }
}
