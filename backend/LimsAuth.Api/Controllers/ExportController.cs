using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ExportService _exportService;

    public ExportController(AppDbContext context, ExportService exportService)
    {
        _context = context;
        _exportService = exportService;
    }

    /// <summary>
    /// 获取可用的导出模板列表
    /// </summary>
    [HttpGet("templates")]
    public ActionResult<IEnumerable<object>> GetTemplates()
    {
        var templates = _exportService.GetAvailableTemplates();
        return Ok(new
        {
            code = 200,
            data = templates.Select(t => new { key = t, name = _exportService.GetTemplateName(t) })
        });
    }

    /// <summary>
    /// 导出实验课程教学任务一览表（Word）
    /// </summary>
    [HttpGet("experiment/task-list")]
    public async Task<IActionResult> ExportExperimentTaskList(
        [FromQuery] Guid? semesterId,
        [FromQuery] Guid? majorId,
        [FromQuery] Guid? classId)
    {
        var query = _context.ExperimentTeachingTasks
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .AsQueryable();

        if (semesterId.HasValue)
            query = query.Where(e => e.SemesterId == semesterId.Value);
        if (majorId.HasValue)
            query = query.Where(e => e.MajorId == majorId.Value);
        if (classId.HasValue)
            query = query.Where(e => e.ClassId == classId.Value);

        var tasks = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();

        var semesterName = semesterId.HasValue
            ? tasks.FirstOrDefault()?.Semester?.Name ?? "全部"
            : "全部";

        var data = tasks.Select(t => new ExportDataContext
        {
            SemesterName = t.Semester?.Name,
            Task = t
        });

        var options = new ExportOptions { SemesterName = semesterName };
        var base64 = _exportService.Export("task_list", data, options);
        var bytes = Convert.FromBase64String(base64);

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"实验课程教学任务一览表_{DateTime.Now:yyyyMMddHHmmss}.docx");
    }

    /// <summary>
    /// 导出实验教学授课计划表（Word）
    /// </summary>
    [HttpGet("experiment/schedule-plan")]
    public async Task<IActionResult> ExportExperimentSchedulePlan(
        [FromQuery] Guid? semesterId,
        [FromQuery] Guid? majorId,
        [FromQuery] Guid? classId)
    {
        var taskQuery = _context.ExperimentTeachingTasks
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .AsQueryable();

        if (semesterId.HasValue)
            taskQuery = taskQuery.Where(e => e.SemesterId == semesterId.Value);
        if (majorId.HasValue)
            taskQuery = taskQuery.Where(e => e.MajorId == majorId.Value);
        if (classId.HasValue)
            taskQuery = taskQuery.Where(e => e.ClassId == classId.Value);

        var tasks = await taskQuery.ToListAsync();
        var taskIds = tasks.Select(t => t.Id).ToList();

        var schedules = await _context.ExperimentItemSchedules
            .Include(s => s.ExperimentTask)
            .Include(s => s.ExperimentItem)
            .Include(s => s.Lab)
                .ThenInclude(l => l!.Building)
                    .ThenInclude(b => b!.Campus)
            .Where(s => taskIds.Contains(s.ExperimentTaskId))
            .OrderBy(s => s.ExperimentTaskId)
            .ThenBy(s => s.WeekNumber)
            .ThenBy(s => s.DayOfWeek)
            .ToListAsync();

        var semesterName = semesterId.HasValue
            ? tasks.FirstOrDefault()?.Semester?.Name ?? "全部"
            : "全部";

        var taskDict = tasks.ToDictionary(t => t.Id);
        var data = new List<ExportDataContext>();

        foreach (var task in tasks)
        {
            data.Add(new ExportDataContext { SemesterName = task.Semester?.Name, Task = task });

            var taskSchedules = schedules.Where(s => s.ExperimentTaskId == task.Id);
            foreach (var schedule in taskSchedules)
            {
                data.Add(new ExportDataContext { SemesterName = task.Semester?.Name, Task = task, Schedule = schedule });
            }
        }

        var options = new ExportOptions { SemesterName = semesterName };
        var base64 = _exportService.Export("schedule_plan", data, options);
        var bytes = Convert.FromBase64String(base64);

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"实验教学授课计划表_{DateTime.Now:yyyyMMddHHmmss}.docx");
    }
}
