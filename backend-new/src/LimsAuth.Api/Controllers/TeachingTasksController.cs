using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TeachingTasksController : ControllerBase
{
    private readonly ITeachingTaskService _teachingTaskService;

    public TeachingTasksController(ITeachingTaskService teachingTaskService)
    {
        _teachingTaskService = teachingTaskService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:teachingtask:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _teachingTaskService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:teachingtask:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _teachingTaskService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:teachingtask:create")]
    public async Task<IActionResult> Create([FromBody] CreateTeachingTaskRequest request)
    {
        var result = await _teachingTaskService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:teachingtask:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeachingTaskRequest request)
    {
        var result = await _teachingTaskService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:teachingtask:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _teachingTaskService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}/teachers")]
    [Authorize(Policy = "Permission:teachingtask:update")]
    public async Task<IActionResult> UpdateTeachers(Guid id, [FromBody] TeachingTaskTeachersRequest request)
    {
        var result = await _teachingTaskService.UpdateTeachersAsync(id, request);
        return StatusCode(result.Code, result);
    }
}
