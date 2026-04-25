using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExperimentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ExperimentsController> _logger;

    public ExperimentsController(AppDbContext context, ILogger<ExperimentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region 实验教学任务

    /// <summary>
    /// 获取实验教学任务列表
    /// </summary>
    [HttpGet("tasks")]
    public async Task<ActionResult<IEnumerable<ExperimentTeachingTask>>> GetExperimentTasks(
        [FromQuery] Guid? semesterId,
        [FromQuery] Guid? majorId,
        [FromQuery] Guid? classId)
    {
        var query = _context.ExperimentTeachingTasks
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .Include(e => e.Department)
            .AsQueryable();

        if (semesterId.HasValue)
            query = query.Where(e => e.SemesterId == semesterId.ToString());
        
        if (majorId.HasValue)
            query = query.Where(e => e.MajorId == majorId.ToString());
        
        if (classId.HasValue)
            query = query.Where(e => e.ClassId == classId.ToString());

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    /// <summary>
    /// 获取单个实验教学任务
    /// </summary>
    [HttpGet("tasks/{id}")]
    public async Task<ActionResult<ExperimentTeachingTask>> GetExperimentTask(string id)
    {
        var task = await _context.ExperimentTeachingTasks
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .Include(e => e.Department)
            .Include(e => e.Schedules)
            .ThenInclude(s => s.ExperimentItem)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (task == null)
            return NotFound();

        return task;
    }

    /// <summary>
    /// 创建实验教学任务
    /// </summary>
    [HttpPost("tasks")]
    public async Task<ActionResult<ExperimentTeachingTask>> CreateExperimentTask(ExperimentTeachingTask task)
    {
        task.Id = Guid.NewGuid().ToString();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentTeachingTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentTask), new { id = task.Id }, task);
    }

    /// <summary>
    /// 更新实验教学任务
    /// </summary>
    [HttpPut("tasks/{id}")]
    public async Task<IActionResult> UpdateExperimentTask(string id, ExperimentTeachingTask task)
    {
        if (id != task.Id)
            return BadRequest();

        task.UpdatedAt = DateTime.UtcNow;
        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ExperimentTeachingTasks.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// 删除实验教学任务
    /// </summary>
    [HttpDelete("tasks/{id}")]
    public async Task<IActionResult> DeleteExperimentTask(string id)
    {
        var task = await _context.ExperimentTeachingTasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.ExperimentTeachingTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 实验项目

    /// <summary>
    /// 获取实验项目列表
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<ExperimentItem>>> GetExperimentItems(
        [FromQuery] string? courseCode,
        [FromQuery] string? experimentType)
    {
        var query = _context.ExperimentItems.AsQueryable();

        if (!string.IsNullOrEmpty(courseCode))
            query = query.Where(e => e.CourseCode == courseCode);
        
        if (!string.IsNullOrEmpty(experimentType))
            query = query.Where(e => e.ExperimentType == experimentType);

        return await query.OrderBy(e => e.CourseCode).ThenBy(e => e.SortOrder).ToListAsync();
    }

    /// <summary>
    /// 获取单个实验项目
    /// </summary>
    [HttpGet("items/{id}")]
    public async Task<ActionResult<ExperimentItem>> GetExperimentItem(string id)
    {
        var item = await _context.ExperimentItems
            .Include(e => e.Schedules)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (item == null)
            return NotFound();

        return item;
    }

    /// <summary>
    /// 创建实验项目
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ExperimentItem>> CreateExperimentItem(ExperimentItem item)
    {
        item.Id = Guid.NewGuid().ToString();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentItem), new { id = item.Id }, item);
    }

    /// <summary>
    /// 更新实验项目
    /// </summary>
    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateExperimentItem(string id, ExperimentItem item)
    {
        if (id != item.Id)
            return BadRequest();

        item.UpdatedAt = DateTime.UtcNow;
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ExperimentItems.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// 删除实验项目
    /// </summary>
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteExperimentItem(string id)
    {
        var item = await _context.ExperimentItems.FindAsync(id);
        if (item == null)
            return NotFound();

        _context.ExperimentItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 实验项目开出安排

    /// <summary>
    /// 获取实验安排列表
    /// </summary>
    [HttpGet("schedules")]
    public async Task<ActionResult<IEnumerable<ExperimentItemSchedule>>> GetExperimentSchedules(
        [FromQuery] string? taskId,
        [FromQuery] int? weekNumber)
    {
        var query = _context.ExperimentItemSchedules
            .Include(e => e.ExperimentTask)
            .Include(e => e.ExperimentItem)
            .AsQueryable();

        if (!string.IsNullOrEmpty(taskId))
            query = query.Where(e => e.ExperimentTaskId == taskId);
        
        if (weekNumber.HasValue)
            query = query.Where(e => e.WeekNumber == weekNumber);

        return await query.OrderBy(e => e.WeekNumber).ThenBy(e => e.DayOfWeek).ToListAsync();
    }

    /// <summary>
    /// 获取单个实验安排
    /// </summary>
    [HttpGet("schedules/{id}")]
    public async Task<ActionResult<ExperimentItemSchedule>> GetExperimentSchedule(string id)
    {
        var schedule = await _context.ExperimentItemSchedules
            .Include(e => e.ExperimentTask)
            .Include(e => e.ExperimentItem)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (schedule == null)
            return NotFound();

        return schedule;
    }

    /// <summary>
    /// 创建实验安排
    /// </summary>
    [HttpPost("schedules")]
    public async Task<ActionResult<ExperimentItemSchedule>> CreateExperimentSchedule(ExperimentItemSchedule schedule)
    {
        schedule.Id = Guid.NewGuid().ToString();
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentItemSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentSchedule), new { id = schedule.Id }, schedule);
    }

    /// <summary>
    /// 更新实验安排
    /// </summary>
    [HttpPut("schedules/{id}")]
    public async Task<IActionResult> UpdateExperimentSchedule(string id, ExperimentItemSchedule schedule)
    {
        if (id != schedule.Id)
            return BadRequest();

        schedule.UpdatedAt = DateTime.UtcNow;
        _context.Entry(schedule).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ExperimentItemSchedules.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// 删除实验安排
    /// </summary>
    [HttpDelete("schedules/{id}")]
    public async Task<IActionResult> DeleteExperimentSchedule(string id)
    {
        var schedule = await _context.ExperimentItemSchedules.FindAsync(id);
        if (schedule == null)
            return NotFound();

        _context.ExperimentItemSchedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 实验质量评估

    /// <summary>
    /// 获取质量评估列表
    /// </summary>
    [HttpGet("quality")]
    public async Task<ActionResult<IEnumerable<ExperimentQualityAssessment>>> GetQualityAssessments()
    {
        return await _context.ExperimentQualityAssessments
            .Include(e => e.ExperimentTask)
            .Include(e => e.Institution)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 获取单个质量评估
    /// </summary>
    [HttpGet("quality/{id}")]
    public async Task<ActionResult<ExperimentQualityAssessment>> GetQualityAssessment(string id)
    {
        var assessment = await _context.ExperimentQualityAssessments
            .Include(e => e.ExperimentTask)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (assessment == null)
            return NotFound();

        return assessment;
    }

    /// <summary>
    /// 创建质量评估
    /// </summary>
    [HttpPost("quality")]
    public async Task<ActionResult<ExperimentQualityAssessment>> CreateQualityAssessment(ExperimentQualityAssessment assessment)
    {
        assessment.Id = Guid.NewGuid().ToString();
        assessment.CreatedAt = DateTime.UtcNow;
        assessment.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentQualityAssessments.Add(assessment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQualityAssessment), new { id = assessment.Id }, assessment);
    }

    /// <summary>
    /// 更新质量评估
    /// </summary>
    [HttpPut("quality/{id}")]
    public async Task<IActionResult> UpdateQualityAssessment(string id, ExperimentQualityAssessment assessment)
    {
        if (id != assessment.Id)
            return BadRequest();

        assessment.UpdatedAt = DateTime.UtcNow;
        _context.Entry(assessment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ExperimentQualityAssessments.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    #endregion

    #region 实训教学计划

    /// <summary>
    /// 获取实训计划列表
    /// </summary>
    [HttpGet("training-plans")]
    public async Task<ActionResult<IEnumerable<TrainingTeachingPlan>>> GetTrainingPlans(
        [FromQuery] Guid? courseId)
    {
        var query = _context.TrainingTeachingPlans
            .Include(e => e.Course)
            .AsQueryable();

        if (courseId.HasValue)
            query = query.Where(e => e.CourseId == courseId.ToString());

        return await query.OrderBy(e => e.Course!.Code).ToListAsync();
    }

    /// <summary>
    /// 获取单个实训计划
    /// </summary>
    [HttpGet("training-plans/{id}")]
    public async Task<ActionResult<TrainingTeachingPlan>> GetTrainingPlan(string id)
    {
        var plan = await _context.TrainingTeachingPlans
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (plan == null)
            return NotFound();

        return plan;
    }

    /// <summary>
    /// 创建实训计划
    /// </summary>
    [HttpPost("training-plans")]
    public async Task<ActionResult<TrainingTeachingPlan>> CreateTrainingPlan(TrainingTeachingPlan plan)
    {
        plan.Id = Guid.NewGuid().ToString();
        plan.CreatedAt = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;

        _context.TrainingTeachingPlans.Add(plan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTrainingPlan), new { id = plan.Id }, plan);
    }

    /// <summary>
    /// 更新实训计划
    /// </summary>
    [HttpPut("training-plans/{id}")]
    public async Task<IActionResult> UpdateTrainingPlan(string id, TrainingTeachingPlan plan)
    {
        if (id != plan.Id)
            return BadRequest();

        plan.UpdatedAt = DateTime.UtcNow;
        _context.Entry(plan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.TrainingTeachingPlans.AnyAsync(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    #endregion

    #region 楼宇和场地管理

    /// <summary>
    /// 获取楼宇列表
    /// </summary>
    [HttpGet("buildings")]
    public async Task<ActionResult<IEnumerable<VenBuilding>>> GetBuildings()
    {
        return await _context.VenBuildings
            .Include(b => b.Rooms)
            .OrderBy(b => b.Code)
            .ToListAsync();
    }

    /// <summary>
    /// 获取场地/实验室列表
    /// </summary>
    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<VenRoom>>> GetRooms(
        [FromQuery] string? buildingId,
        [FromQuery] string? roomType)
    {
        var query = _context.VenRooms
            .Include(r => r.Building)
            .AsQueryable();

        if (!string.IsNullOrEmpty(buildingId))
            query = query.Where(r => r.BuildingId == buildingId);
        
        if (!string.IsNullOrEmpty(roomType))
            query = query.Where(r => r.RoomType == roomType);

        return await query.OrderBy(r => r.Building!.Name).ThenBy(r => r.RoomNumber).ToListAsync();
    }

    #endregion
}
