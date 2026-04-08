using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models.DTOs;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/teaching-tasks")]
[Authorize]
public class TeachingTasksController : ControllerBase
{
    private readonly ITeachingTaskService _teachingTaskService;

    public TeachingTasksController(ITeachingTaskService teachingTaskService)
    {
        _teachingTaskService = teachingTaskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? semesterId, [FromQuery] string? courseId, [FromQuery] string? classId, [FromQuery] string? teacherId)
    {
        var tasks = await _teachingTaskService.GetListAsync(semesterId, courseId, classId, teacherId);
        return Ok(new { code = 200, data = tasks });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _teachingTaskService.GetByIdAsync(id);
        if (task == null)
            return NotFound(new { code = 404, message = "教学任务不存在" });
        return Ok(new { code = 200, data = task });
    }

    [HttpPost]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> Create([FromBody] CreateTeachingTaskRequest request)
    {
        var task = await _teachingTaskService.CreateAsync(request);
        return Ok(new { code = 200, data = task, message = "创建成功" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeachingTaskRequest request)
    {
        var task = await _teachingTaskService.UpdateAsync(id, request);
        if (task == null)
            return NotFound(new { code = 404, message = "教学任务不存在" });
        return Ok(new { code = 200, data = task, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _teachingTaskService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "教学任务不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _teachingTaskService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "教学任务不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }

    [HttpPost("{id}/teachers")]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> AddTeacher(Guid id, [FromBody] AddTeacherRequest request)
    {
        var result = await _teachingTaskService.AddTeacherAsync(id, request.TeacherId, request.IsMainTeacher);
        if (!result)
            return BadRequest(new { code = 400, message = "添加教师失败" });
        return Ok(new { code = 200, message = "添加成功" });
    }

    [HttpDelete("{id}/teachers/{teacherId}")]
    [Authorize(Policy = "Permission:course:schedule")]
    public async Task<IActionResult> RemoveTeacher(Guid id, Guid teacherId)
    {
        var result = await _teachingTaskService.RemoveTeacherAsync(id, teacherId);
        if (!result)
            return NotFound(new { code = 404, message = "教师不在任务中" });
        return Ok(new { code = 200, message = "移除成功" });
    }
}

public class AddTeacherRequest
{
    public Guid TeacherId { get; set; }
    public bool IsMainTeacher { get; set; }
}
