using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MajorsController : ControllerBase
{
    private readonly IMajorService _majorService;

    public MajorsController(IMajorService majorService)
    {
        _majorService = majorService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:major:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _majorService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:major:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _majorService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:major:create")]
    public async Task<IActionResult> Create([FromBody] CreateMajorRequest request)
    {
        var result = await _majorService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:major:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMajorRequest request)
    {
        var result = await _majorService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:major:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _majorService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Permission:major:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _majorService.ToggleStatusAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
