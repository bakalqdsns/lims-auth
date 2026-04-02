using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/semesters")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] bool? isCurrent)
    {
        var semesters = await _semesterService.GetListAsync(keyword, isCurrent);
        return Ok(new { code = 200, data = semesters });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var semester = await _semesterService.GetByIdAsync(id);
        if (semester == null)
            return NotFound(new { code = 404, message = "学期不存在" });
        return Ok(new { code = 200, data = semester });
    }

    [HttpPost]
    [Authorize(Policy = "Permission")]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
    {
        var semester = await _semesterService.CreateAsync(request);
        return Ok(new { code = 200, data = semester, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSemesterRequest request)
    {
        var semester = await _semesterService.UpdateAsync(id, request);
        if (semester == null)
            return NotFound(new { code = 404, message = "学期不存在" });
        return Ok(new { code = 200, data = semester, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _semesterService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "学期不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPost("{id}/set-current")]
    [Authorize(Policy = "Permission")]
    public async Task<IActionResult> SetCurrent(Guid id)
    {
        var result = await _semesterService.SetCurrentAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "学期不存在" });
        return Ok(new { code = 200, message = "设置成功" });
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var semester = await _semesterService.GetCurrentAsync();
        if (semester == null)
            return NotFound(new { code = 404, message = "没有当前学期" });
        return Ok(new { code = 200, data = semester });
    }

    [HttpPost("{id}/generate-calendar")]
    [Authorize(Policy = "Permission")]
    public async Task<IActionResult> GenerateCalendar(Guid id)
    {
        try
        {
            var calendars = await _semesterService.GenerateCalendarAsync(id);
            return Ok(new { code = 200, data = calendars, message = "校历生成成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }
}
