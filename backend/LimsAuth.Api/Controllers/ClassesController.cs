using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/classes")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? keyword, [FromQuery] string? departmentId, [FromQuery] string? majorId, [FromQuery] string? grade)
    {
        var classes = await _classService.GetListAsync(keyword, departmentId, majorId, grade);
        return Ok(new { code = 200, data = classes });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var classEntity = await _classService.GetByIdAsync(id);
        if (classEntity == null)
            return NotFound(new { code = 404, message = "班级不存在" });
        return Ok(new { code = 200, data = classEntity });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request)
    {
        var classEntity = await _classService.CreateAsync(request);
        return Ok(new { code = 200, data = classEntity, message = "创建成功" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request)
    {
        var classEntity = await _classService.UpdateAsync(id, request);
        if (classEntity == null)
            return NotFound(new { code = 404, message = "班级不存在" });
        return Ok(new { code = 200, data = classEntity, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _classService.DeleteAsync(id);
        if (!result)
            return NotFound(new { code = 404, message = "班级不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _classService.ToggleStatusAsync(id, request.IsActive);
        if (!result)
            return NotFound(new { code = 404, message = "班级不存在" });
        return Ok(new { code = 200, message = "状态更新成功" });
    }

    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetStudents(Guid id)
    {
        var students = await _classService.GetStudentsAsync(id);
        return Ok(new { code = 200, data = students });
    }

    [HttpPost("{id}/students")]
    public async Task<IActionResult> AddStudents(Guid id, [FromBody] AddStudentsRequest request)
    {
        var result = await _classService.AddStudentsAsync(id, request.StudentIds);
        if (!result)
            return NotFound(new { code = 404, message = "班级不存在" });
        return Ok(new { code = 200, message = "添加成功" });
    }

    [HttpDelete("{id}/students/{studentId}")]
    public async Task<IActionResult> RemoveStudent(Guid id, Guid studentId)
    {
        var result = await _classService.RemoveStudentAsync(id, studentId);
        if (!result)
            return NotFound(new { code = 404, message = "学生不在班级中" });
        return Ok(new { code = 200, message = "移除成功" });
    }
}

public class AddStudentsRequest
{
    public List<Guid> StudentIds { get; set; } = new();
}
