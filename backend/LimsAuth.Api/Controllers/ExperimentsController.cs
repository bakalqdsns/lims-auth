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
            .Include(e => e.Institution)
            .AsQueryable();

        if (semesterId.HasValue)
            query = query.Where(e => e.SemesterId == semesterId.Value);

        if (majorId.HasValue)
            query = query.Where(e => e.MajorId == majorId.Value);

        if (classId.HasValue)
            query = query.Where(e => e.ClassId == classId.Value);

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    [HttpGet("tasks/{id}")]
    public async Task<ActionResult<ExperimentTeachingTask>> GetExperimentTask(Guid id)
    {
        var task = await _context.ExperimentTeachingTasks
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .Include(e => e.Department)
            .Include(e => e.Institution)
            .Include(e => e.Schedules)
                .ThenInclude(s => s.ExperimentItem)
            .Include(e => e.QualityAssessment)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (task == null)
            return NotFound();

        return task;
    }

    [HttpPost("tasks")]
    public async Task<ActionResult<ExperimentTeachingTask>> CreateExperimentTask(ExperimentTeachingTask task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentTeachingTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentTask), new { id = task.Id }, task);
    }

    [HttpPut("tasks/{id}")]
    public async Task<IActionResult> UpdateExperimentTask(Guid id, ExperimentTeachingTask task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpDelete("tasks/{id}")]
    public async Task<IActionResult> DeleteExperimentTask(Guid id)
    {
        var task = await _context.ExperimentTeachingTasks
            .Include(t => t.Schedules)
            .Include(t => t.QualityAssessment)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound();

        if (task.Schedules?.Count > 0)
            _context.ExperimentItemSchedules.RemoveRange(task.Schedules);

        if (task.QualityAssessment != null)
            _context.ExperimentQualityAssessments.Remove(task.QualityAssessment);

        _context.ExperimentTeachingTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 实验项目

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

    [HttpGet("items/by-task/{taskId}")]
    public async Task<ActionResult<IEnumerable<ExperimentItem>>> GetExperimentItemsByTask(Guid taskId)
    {
        var itemIds = await _context.ExperimentItemSchedules
            .Where(s => s.ExperimentTaskId == taskId)
            .Select(s => s.ExperimentItemId)
            .Distinct()
            .ToListAsync();

        var items = await _context.ExperimentItems
            .Where(i => itemIds.Contains(i.Id))
            .OrderBy(e => e.SortOrder)
            .ToListAsync();

        return items;
    }

    [HttpGet("items/{id}")]
    public async Task<ActionResult<ExperimentItem>> GetExperimentItem(Guid id)
    {
        var item = await _context.ExperimentItems
            .Include(e => e.Schedules)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (item == null)
            return NotFound();

        return item;
    }

    [HttpPost("items")]
    public async Task<ActionResult<ExperimentItem>> CreateExperimentItem(ExperimentItem item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentItem), new { id = item.Id }, item);
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateExperimentItem(Guid id, ExperimentItem item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteExperimentItem(Guid id)
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

    [HttpGet("schedules")]
    public async Task<ActionResult<IEnumerable<ExperimentItemSchedule>>> GetExperimentSchedules(
        [FromQuery] Guid? taskId,
        [FromQuery] int? weekNumber)
    {
        var query = _context.ExperimentItemSchedules
            .Include(e => e.ExperimentTask)
            .Include(e => e.ExperimentItem)
            .Include(e => e.Lab)
                .ThenInclude(l => l!.Building)
                    .ThenInclude(b => b!.Campus)
            .AsQueryable();

        if (taskId.HasValue)
            query = query.Where(e => e.ExperimentTaskId == taskId.Value);

        if (weekNumber.HasValue)
            query = query.Where(e => e.WeekNumber == weekNumber);

        return await query.OrderBy(e => e.WeekNumber).ThenBy(e => e.DayOfWeek).ToListAsync();
    }

    [HttpGet("schedules/{id}")]
    public async Task<ActionResult<ExperimentItemSchedule>> GetExperimentSchedule(Guid id)
    {
        var schedule = await _context.ExperimentItemSchedules
            .Include(e => e.ExperimentTask)
            .Include(e => e.ExperimentItem)
            .Include(e => e.Lab)
                .ThenInclude(l => l!.Building)
                    .ThenInclude(b => b!.Campus)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (schedule == null)
            return NotFound();

        return schedule;
    }

    [HttpPost("schedules")]
    public async Task<ActionResult<ExperimentItemSchedule>> CreateExperimentSchedule(ExperimentItemSchedule schedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        schedule.Id = Guid.NewGuid();
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentItemSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperimentSchedule), new { id = schedule.Id }, schedule);
    }

    [HttpPut("schedules/{id}")]
    public async Task<IActionResult> UpdateExperimentSchedule(Guid id, ExperimentItemSchedule schedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpDelete("schedules/{id}")]
    public async Task<IActionResult> DeleteExperimentSchedule(Guid id)
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

    [HttpGet("quality")]
    public async Task<ActionResult<IEnumerable<ExperimentQualityAssessment>>> GetQualityAssessments()
    {
        return await _context.ExperimentQualityAssessments
            .Include(e => e.ExperimentTask)
            .Include(e => e.Institution)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("quality/{id}")]
    public async Task<ActionResult<ExperimentQualityAssessment>> GetQualityAssessment(Guid id)
    {
        var assessment = await _context.ExperimentQualityAssessments
            .Include(e => e.ExperimentTask)
            .Include(e => e.Institution)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (assessment == null)
            return NotFound();

        return assessment;
    }

    [HttpPost("quality")]
    public async Task<ActionResult<ExperimentQualityAssessment>> CreateQualityAssessment(ExperimentQualityAssessment assessment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        assessment.Id = Guid.NewGuid();
        assessment.CreatedAt = DateTime.UtcNow;
        assessment.UpdatedAt = DateTime.UtcNow;

        _context.ExperimentQualityAssessments.Add(assessment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQualityAssessment), new { id = assessment.Id }, assessment);
    }

    [HttpPut("quality/{id}")]
    public async Task<IActionResult> UpdateQualityAssessment(Guid id, ExperimentQualityAssessment assessment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpDelete("quality/{id}")]
    public async Task<IActionResult> DeleteQualityAssessment(Guid id)
    {
        var assessment = await _context.ExperimentQualityAssessments.FindAsync(id);
        if (assessment == null)
            return NotFound();

        _context.ExperimentQualityAssessments.Remove(assessment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 实训教学计划

    [HttpGet("training-plans")]
    public async Task<ActionResult<IEnumerable<TrainingTeachingPlan>>> GetTrainingPlans(
        [FromQuery] Guid? courseId,
        [FromQuery] string? status,
        [FromQuery] string? approvalStatus)
    {
        var query = _context.TrainingTeachingPlans
            .Include(e => e.Course)
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .AsQueryable();

        if (courseId.HasValue)
            query = query.Where(e => e.CourseId == courseId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(e => e.Status == status);

        if (!string.IsNullOrEmpty(approvalStatus))
        {
            if (approvalStatus == "PendingExperimentCenter")
                query = query.Where(e => e.ExperimentCenterOpinionStatus == "Pending" || e.ExperimentCenterOpinionStatus == null);
            else if (approvalStatus == "PendingDepartment")
                query = query.Where(e => e.DepartmentOpinionStatus == "Pending" || e.DepartmentOpinionStatus == null);
            else if (approvalStatus == "Approved")
                query = query.Where(e => e.ExperimentCenterOpinionStatus == "Approved" && e.DepartmentOpinionStatus == "Approved");
            else if (approvalStatus == "Rejected")
                query = query.Where(e => e.ExperimentCenterOpinionStatus == "Rejected" || e.DepartmentOpinionStatus == "Rejected");
        }

        return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
    }

    [HttpGet("training-plans/{id}")]
    public async Task<ActionResult<TrainingTeachingPlan>> GetTrainingPlan(Guid id)
    {
        var plan = await _context.TrainingTeachingPlans
            .Include(e => e.Course)
            .Include(e => e.Semester)
            .Include(e => e.Major)
            .Include(e => e.Class)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (plan == null)
            return NotFound();

        return plan;
    }

    [HttpPost("training-plans")]
    public async Task<ActionResult<TrainingTeachingPlan>> CreateTrainingPlan(TrainingTeachingPlan plan)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        plan.Id = Guid.NewGuid();
        plan.CreatedAt = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;

        _context.TrainingTeachingPlans.Add(plan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTrainingPlan), new { id = plan.Id }, plan);
    }

    [HttpPut("training-plans/{id}")]
    public async Task<IActionResult> UpdateTrainingPlan(Guid id, TrainingTeachingPlan plan)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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

    [HttpDelete("training-plans/{id}")]
    public async Task<IActionResult> DeleteTrainingPlan(Guid id)
    {
        var plan = await _context.TrainingTeachingPlans.FindAsync(id);
        if (plan == null)
            return NotFound();

        _context.TrainingTeachingPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("training-plans/{id}/approve")]
    public async Task<IActionResult> ApproveTrainingPlan(Guid id, [FromBody] ApproveTrainingPlanRequest request)
    {
        var plan = await _context.TrainingTeachingPlans.FindAsync(id);
        if (plan == null)
            return NotFound();

        if (request.ApprovalType == "ExperimentCenter")
        {
            plan.ExperimentCenterOpinion = request.Opinion;
            plan.ExperimentCenterOpinionStatus = request.Status;
            plan.ExperimentCenterApprovedBy = request.Approver;
            plan.ExperimentCenterApprovalDate = request.Status == "Approved" || request.Status == "Rejected" ? DateTime.UtcNow : null;
        }
        else if (request.ApprovalType == "Department")
        {
            plan.DepartmentOpinion = request.Opinion;
            plan.DepartmentOpinionStatus = request.Status;
            plan.DepartmentApprovedBy = request.Approver;
            plan.DepartmentApprovalDate = request.Status == "Approved" || request.Status == "Rejected" ? DateTime.UtcNow : null;
        }

        plan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region 楼宇和场地管理

    [HttpGet("buildings")]
    public async Task<ActionResult<IEnumerable<VenBuilding>>> GetBuildings()
    {
        return await _context.VenBuildings
            .Include(b => b.Rooms)
            .OrderBy(b => b.Code)
            .ToListAsync();
    }

    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<VenRoom>>> GetRooms(
        [FromQuery] Guid? buildingId,
        [FromQuery] string? roomType)
    {
        var query = _context.VenRooms
            .Include(r => r.Building)
            .AsQueryable();

        if (buildingId.HasValue)
            query = query.Where(r => r.BuildingId == buildingId.Value);

        if (!string.IsNullOrEmpty(roomType))
            query = query.Where(r => r.RoomType == roomType);

        return await query.OrderBy(r => r.Building!.Name).ThenBy(r => r.RoomNumber).ToListAsync();
    }

    #endregion
}

public class ApproveTrainingPlanRequest
{
    public string ApprovalType { get; set; } = string.Empty;
    public string Opinion { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Approver { get; set; }
}
