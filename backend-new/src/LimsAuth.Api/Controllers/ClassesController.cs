using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:class:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _classService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:class:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _classService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:class:create")]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request)
    {
        var result = await _classService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:class:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request)
    {
        var result = await _classService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:class:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _classService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Permission:class:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _classService.ToggleStatusAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}/students")]
    [Authorize(Policy = "Permission:class:read")]
    public async Task<IActionResult> GetStudents(Guid id)
    {
        var result = await _classService.GetStudentsAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}/students")]
    [Authorize(Policy = "Permission:class:update")]
    public async Task<IActionResult> UpdateStudents(Guid id, [FromBody] ClassStudentsRequest request)
    {
        var result = await _classService.UpdateStudentsAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
