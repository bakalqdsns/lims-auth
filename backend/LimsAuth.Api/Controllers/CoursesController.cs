using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] string? departmentId, [FromQuery] string? courseType)
    {
        var courses = await _courseService.GetListAsync(keyword, departmentId, courseType);
        return Ok(new { code = 200, data = courses });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound(new { code = 404, message = "课程不存在" });
        return Ok(new { code = 200, data = course });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:course:create")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var course = await _courseService.CreateAsync(request);
        return Ok(new { code = 200, data = course, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:course:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var course = await _courseService.UpdateAsync(id, request);
        if (course == null)
            return NotFound(new { code = 404, message = "课程不存在" });
        return Ok(new { code = 200, data = course, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:course:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _courseService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "课程不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:course:update")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _courseService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "课程不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }
}

public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}
