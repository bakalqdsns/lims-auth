using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:course:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _courseService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:course:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _courseService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:course:create")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var result = await _courseService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:course:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var result = await _courseService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:course:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _courseService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Permission:course:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _courseService.ToggleStatusAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
