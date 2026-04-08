using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Services;
using LimsAuth.Api.Models;

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

    #region 基础 CRUD

    /// <summary>
    /// 获取学期列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? keyword,
        [FromQuery] SemesterStatus? status,
        [FromQuery] SemesterType? type,
        [FromQuery] bool? isCurrent)
    {
        var semesters = await _semesterService.GetListAsync(keyword, status, type, isCurrent);
        return Ok(new { code = 200, data = semesters });
    }

    /// <summary>
    /// 获取学期详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var semester = await _semesterService.GetByIdAsync(id);
        if (semester == null)
            return NotFound(new { code = 404, message = "学期不存在" });
        return Ok(new { code = 200, data = semester });
    }

    /// <summary>
    /// 获取当前学期
    /// </summary>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var semester = await _semesterService.GetCurrentAsync();
        if (semester == null)
            return NotFound(new { code = 404, message = "没有当前学期" });
        return Ok(new { code = 200, data = semester });
    }

    /// <summary>
    /// 创建学期
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
    {
        try
        {
            // 先校验
            var validation = await _semesterService.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return BadRequest(new { code = 400, message = string.Join("; ", validation.Errors) });
            }

            var userId = GetCurrentUserId();
            var semester = await _semesterService.CreateAsync(request, userId);
            return Ok(new { code = 200, data = semester, message = "创建成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 更新学期
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSemesterRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var semester = await _semesterService.UpdateAsync(id, request, userId);
            if (semester == null)
                return NotFound(new { code = 404, message = "学期不存在" });
            return Ok(new { code = 200, data = semester, message = "更新成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 删除学期
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _semesterService.DeleteAsync(id);
            if (!result)
                return NotFound(new { code = 404, message = "学期不存在" });
            return Ok(new { code = 200, message = "删除成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    #endregion

    #region 状态管理

    /// <summary>
    /// 设为当前学期
    /// </summary>
    [HttpPost("{id}/set-current")]
    [Authorize]
    public async Task<IActionResult> SetCurrent(Guid id)
    {
        try
        {
            var result = await _semesterService.SetCurrentAsync(id);
            if (!result)
                return NotFound(new { code = 404, message = "学期不存在" });
            return Ok(new { code = 200, message = "设置成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 更新学期状态
    /// </summary>
    [HttpPost("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _semesterService.UpdateStatusAsync(id, request.Status, userId);
            if (!result)
                return NotFound(new { code = 404, message = "学期不存在" });
            return Ok(new { code = 200, message = "状态更新成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 归档学期
    /// </summary>
    [HttpPost("{id}/archive")]
    [Authorize]
    public async Task<IActionResult> Archive(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _semesterService.ArchiveAsync(id, userId);
            if (!result)
                return NotFound(new { code = 404, message = "学期不存在" });
            return Ok(new { code = 200, message = "归档成功" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 自动状态流转（定时任务调用）
    /// </summary>
    [HttpPost("auto-transition")]
    [Authorize]
    public async Task<IActionResult> AutoTransition()
    {
        await _semesterService.AutoTransitionStatusAsync();
        return Ok(new { code = 200, message = "状态流转完成" });
    }

    #endregion

    #region 校验与验证

    /// <summary>
    /// 校验学期数据
    /// </summary>
    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> Validate([FromBody] CreateSemesterRequest request)
    {
        var result = await _semesterService.ValidateAsync(request);
        return Ok(new { code = 200, data = result });
    }

    /// <summary>
    /// 检查时间重叠
    /// </summary>
    [HttpGet("check-overlap")]
    [Authorize]
    public async Task<IActionResult> CheckOverlap(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] Guid? excludeId)
    {
        var overlap = await _semesterService.CheckTimeOverlapAsync(startDate, endDate, excludeId);
        return Ok(new { code = 200, data = new { hasOverlap = overlap } });
    }

    #endregion

    #region 校历管理

    /// <summary>
    /// 生成校历
    /// </summary>
    [HttpPost("{id}/generate-calendar")]
    [Authorize]
    public async Task<IActionResult> GenerateCalendar(Guid id, [FromBody] GenerateCalendarRequest? request)
    {
        try
        {
            var calendars = await _semesterService.GenerateCalendarAsync(id, request);
            return Ok(new { code = 200, data = calendars, message = "校历生成成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 获取学期校历
    /// </summary>
    [HttpGet("{id}/calendar")]
    public async Task<IActionResult> GetCalendar(Guid id)
    {
        var calendars = await _semesterService.GetCalendarAsync(id);
        return Ok(new { code = 200, data = calendars });
    }

    /// <summary>
    /// 获取周次信息
    /// </summary>
    [HttpGet("{id}/week-info")]
    public async Task<IActionResult> GetWeekInfo(Guid id, [FromQuery] int? weekNumber)
    {
        var calendars = await _semesterService.GetCalendarAsync(id);
        
        if (weekNumber.HasValue)
        {
            var weekDays = calendars.Where(c => c.WeekNumber == weekNumber.Value).ToList();
            if (!weekDays.Any())
                return NotFound(new { code = 404, message = "周次不存在" });
            
            return Ok(new { 
                code = 200, 
                data = new 
                { 
                    weekNumber = weekNumber.Value,
                    startDate = weekDays.Min(c => c.Date),
                    endDate = weekDays.Max(c => c.Date),
                    days = weekDays
                } 
            });
        }
        else
        {
            // 返回所有周次信息
            var weeks = calendars.GroupBy(c => c.WeekNumber)
                .Select(g => new 
                {
                    weekNumber = g.Key,
                    startDate = g.Min(c => c.Date),
                    endDate = g.Max(c => c.Date),
                    dayCount = g.Count()
                })
                .OrderBy(w => w.weekNumber)
                .ToList();
            
            return Ok(new { code = 200, data = weeks });
        }
    }

    /// <summary>
    /// 获取今日校历
    /// </summary>
    [HttpGet("current/today")]
    public async Task<IActionResult> GetTodayCalendar()
    {
        var semester = await _semesterService.GetCurrentAsync();
        if (semester == null)
            return NotFound(new { code = 404, message = "没有当前学期" });

        var calendars = await _semesterService.GetCalendarAsync(semester.Id);
        var today = calendars.FirstOrDefault(c => c.Date.Date == DateTime.UtcNow.Date);
        
        if (today == null)
            return NotFound(new { code = 404, message = "今日不在当前学期范围内" });

        return Ok(new { code = 200, data = today });
    }

    #endregion

    #region 数据复制

    /// <summary>
    /// 从模板复制学期
    /// </summary>
    [HttpPost("copy-from-template")]
    [Authorize]
    public async Task<IActionResult> CopyFromTemplate([FromBody] CopyFromTemplateRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var copyRequest = new CopySemesterRequest
            {
                Name = request.Name,
                Code = request.Code,
                AcademicYear = request.AcademicYear,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
            
            var semester = await _semesterService.CopyFromTemplateAsync(request.TemplateId, copyRequest, userId);
            return Ok(new { code = 200, data = semester, message = "复制成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 从学期复制
    /// </summary>
    [HttpPost("copy-from-semester")]
    [Authorize]
    public async Task<IActionResult> CopyFromSemester([FromBody] CopyFromSemesterRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var copyRequest = new CopySemesterRequest
            {
                Name = request.Name,
                Code = request.Code,
                AcademicYear = request.AcademicYear,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CopyOptions = request.CopyOptions
            };
            
            var semester = await _semesterService.CopyFromSemesterAsync(request.SourceSemesterId, copyRequest, userId);
            return Ok(new { code = 200, data = semester, message = "复制成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    #endregion

    #region 沙箱管理

    /// <summary>
    /// 创建沙箱
    /// </summary>
    [HttpPost("{id}/sandbox")]
    [Authorize]
    public async Task<IActionResult> CreateSandbox(Guid id, [FromBody] CreateSandboxRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var sandbox = await _semesterService.CreateSandboxAsync(id, request.Name, userId);
            return Ok(new { code = 200, data = sandbox, message = "沙箱创建成功" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 应用沙箱
    /// </summary>
    [HttpPost("sandbox/{sandboxId}/apply")]
    [Authorize]
    public async Task<IActionResult> ApplySandbox(Guid sandboxId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _semesterService.ApplySandboxAsync(sandboxId, userId);
            if (!result)
                return NotFound(new { code = 404, message = "沙箱不存在" });
            return Ok(new { code = 200, message = "沙箱已应用到正式环境" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// 废弃沙箱
    /// </summary>
    [HttpDelete("sandbox/{sandboxId}")]
    [Authorize]
    public async Task<IActionResult> DiscardSandbox(Guid sandboxId)
    {
        var result = await _semesterService.DiscardSandboxAsync(sandboxId);
        if (!result)
            return NotFound(new { code = 404, message = "沙箱不存在" });
        return Ok(new { code = 200, message = "沙箱已废弃" });
    }

    #endregion

    #region 日志

    /// <summary>
    /// 获取学期操作日志
    /// </summary>
    [HttpGet("{id}/logs")]
    [Authorize]
    public async Task<IActionResult> GetLogs(Guid id)
    {
        var logs = await _semesterService.GetLogsAsync(id);
        return Ok(new { code = 200, data = logs });
    }

    #endregion

    #region 辅助方法

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }

    #endregion
}

#region Request DTOs

public class UpdateStatusRequest
{
    public SemesterStatus Status { get; set; }
}

public class CopyFromTemplateRequest
{
    public Guid TemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class CopyFromSemesterRequest
{
    public Guid SourceSemesterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CopyOptions? CopyOptions { get; set; }
}

public class CreateSandboxRequest
{
    public string Name { get; set; } = string.Empty;
}

#endregion
