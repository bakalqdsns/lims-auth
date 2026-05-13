using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/schedules")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleEntryDto>>> GetSchedules([FromQuery] ScheduleQuery query)
    {
        var list = await _scheduleService.GetScheduleEntriesAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduleEntryDto>> GetSchedule(Guid id)
    {
        var entry = await _scheduleService.GetScheduleByIdAsync(id);
        if (entry == null) return NotFound(new { code = 404, message = "排课记录不存在" });
        return Ok(new { code = 200, data = entry });
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleEntryDto>> CreateSchedule([FromBody] CreateScheduleEntryRequest request)
    {
        var createdBy = User.Identity?.Name;
        var entry = await _scheduleService.CreateCentralScheduleAsync(request, createdBy);
        return Ok(new { code = 200, data = entry, message = "排课创建成功" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] UpdateScheduleEntryRequest request)
    {
        var updatedBy = User.Identity?.Name;
        var ok = await _scheduleService.UpdateCentralScheduleAsync(id, request, updatedBy);
        if (!ok) return NotFound(new { code = 404, message = "排课记录不存在" });
        return Ok(new { code = 200, message = "排课更新成功" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(Guid id)
    {
        var deletedBy = User.Identity?.Name;
        var ok = await _scheduleService.DeleteCentralScheduleAsync(id, deletedBy);
        if (!ok) return NotFound(new { code = 404, message = "排课记录不存在" });
        return Ok(new { code = 200, message = "排课删除成功" });
    }

    [HttpGet("table-view")]
    public async Task<ActionResult<IEnumerable<ScheduleTableRow>>> GetTableView([FromQuery] ScheduleQuery query)
    {
        var rows = await _scheduleService.GetScheduleTableAsync(query);
        return Ok(new { code = 200, data = rows });
    }

    [HttpGet("available-labs")]
    public async Task<ActionResult<IEnumerable<Lab>>> GetAvailableLabs([FromQuery] AvailabilityQuery query)
    {
        var labs = await _scheduleService.GetAvailableLabsAsync(query);
        return Ok(new { code = 200, data = labs });
    }

    [HttpPost("check-conflicts")]
    public async Task<ActionResult<ConflictCheckResult>> CheckConflicts([FromBody] ScheduleEntry entry)
    {
        var result = await _scheduleService.CheckConflictsAsync(entry);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("by-lab/{labId}")]
    public async Task<ActionResult<IEnumerable<ScheduleEntryDto>>> GetByLab(Guid labId, [FromQuery] ScheduleQuery query)
    {
        query.LabId = labId;
        var list = await _scheduleService.GetScheduleEntriesAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-teacher/{teacherId}")]
    public async Task<ActionResult<IEnumerable<ScheduleEntryDto>>> GetByTeacher(Guid teacherId, [FromQuery] ScheduleQuery query)
    {
        query.TeacherId = teacherId;
        var list = await _scheduleService.GetScheduleEntriesAsync(query);
        return Ok(new { code = 200, data = list });
    }

    [HttpGet("by-class/{classId}")]
    public async Task<ActionResult<IEnumerable<ScheduleEntryDto>>> GetByClass(Guid classId, [FromQuery] ScheduleQuery query)
    {
        query.ClassId = classId;
        var list = await _scheduleService.GetScheduleEntriesAsync(query);
        return Ok(new { code = 200, data = list });
    }
}
