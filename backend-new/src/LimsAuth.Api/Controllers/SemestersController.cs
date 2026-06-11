using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:semester:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _semesterService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var result = await _semesterService.GetCurrentAsync();
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:semester:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _semesterService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:semester:create")]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
    {
        var result = await _semesterService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:semester:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSemesterRequest request)
    {
        var result = await _semesterService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:semester:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _semesterService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost("{id:guid}/set-current")]
    [Authorize(Policy = "Permission:semester:update")]
    public async Task<IActionResult> SetCurrent(Guid id)
    {
        var result = await _semesterService.SetCurrentAsync(id);
        return StatusCode(result.Code, result);
    }
}
