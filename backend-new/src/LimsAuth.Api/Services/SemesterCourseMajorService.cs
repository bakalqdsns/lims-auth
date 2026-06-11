using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data;
using LimsAuth.Api.Models;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Services;

public interface ISemesterService
{
    Task<ApiResponse<PagedResponse<SemesterDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<SemesterDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<SemesterDto>> GetCurrentAsync();
    Task<ApiResponse<SemesterDto>> CreateAsync(CreateSemesterRequest request);
    Task<ApiResponse<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> SetCurrentAsync(Guid id);
}

public class SemesterService : ISemesterService
{
    private readonly AppDbContext _db;
    public SemesterService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<SemesterDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.EduSemesters.Where(s => s.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(s => s.Name.Contains(query.Keyword) || s.Code.Contains(query.Keyword));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(s => s.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(s => new SemesterDto
            {
                Id = s.Id, Code = s.Code, Name = s.Name, SchoolYear = s.SchoolYear,
                SemesterNo = s.SemesterNo, StartDate = s.StartDate, EndDate = s.EndDate,
                TotalWeeks = s.TotalWeeks, IsCurrent = s.IsCurrent, Status = s.Status,
                Description = s.Description, CreatedAt = s.CreatedAt, CreatedBy = s.CreatedBy
            }).ToListAsync();

        return ApiResponse<PagedResponse<SemesterDto>>.Success(new PagedResponse<SemesterDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<SemesterDto>> GetByIdAsync(Guid id)
    {
        var s = await _db.EduSemesters.FindAsync(id);
        if (s == null) return ApiResponse<SemesterDto>.Error(404, "学期不存在");
        return ApiResponse<SemesterDto>.Success(new SemesterDto
        {
            Id = s.Id, Code = s.Code, Name = s.Name, SchoolYear = s.SchoolYear,
            SemesterNo = s.SemesterNo, StartDate = s.StartDate, EndDate = s.EndDate,
            TotalWeeks = s.TotalWeeks, IsCurrent = s.IsCurrent, Status = s.Status,
            Description = s.Description, CreatedAt = s.CreatedAt
        });
    }

    public async Task<ApiResponse<SemesterDto>> GetCurrentAsync()
    {
        var s = await _db.EduSemesters.FirstOrDefaultAsync(x => x.IsCurrent == 1 && x.IsDeleted == 0);
        if (s == null) return ApiResponse<SemesterDto>.Error(404, "无当前学期");
        return ApiResponse<SemesterDto>.Success(new SemesterDto
        {
            Id = s.Id, Code = s.Code, Name = s.Name, SchoolYear = s.SchoolYear,
            SemesterNo = s.SemesterNo, StartDate = s.StartDate, EndDate = s.EndDate,
            TotalWeeks = s.TotalWeeks, IsCurrent = s.IsCurrent, Status = s.Status
        });
    }

    public async Task<ApiResponse<SemesterDto>> CreateAsync(CreateSemesterRequest request)
    {
        var s = new EduSemester
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name,
            SchoolYear = request.SchoolYear, SemesterNo = request.SemesterNo,
            StartDate = request.StartDate, EndDate = request.EndDate,
            TotalWeeks = request.TotalWeeks, IsCurrent = request.IsCurrent,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.EduSemesters.Add(s);
        await _db.SaveChangesAsync();
        return ApiResponse<SemesterDto>.Success(new SemesterDto { Id = s.Id, Code = s.Code, Name = s.Name }, "学期创建成功");
    }

    public async Task<ApiResponse<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest request)
    {
        var s = await _db.EduSemesters.FindAsync(id);
        if (s == null) return ApiResponse<SemesterDto>.Error(404, "学期不存在");
        if (!string.IsNullOrEmpty(request.Name)) s.Name = request.Name;
        if (!string.IsNullOrEmpty(request.SchoolYear)) s.SchoolYear = request.SchoolYear;
        if (request.SemesterNo.HasValue) s.SemesterNo = request.SemesterNo.Value;
        if (request.StartDate.HasValue) s.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) s.EndDate = request.EndDate.Value;
        if (request.TotalWeeks.HasValue) s.TotalWeeks = request.TotalWeeks.Value;
        if (request.Status.HasValue) s.Status = request.Status.Value;
        if (!string.IsNullOrEmpty(request.Description)) s.Description = request.Description;
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<SemesterDto>.Success(new SemesterDto { Id = s.Id, Code = s.Code, Name = s.Name }, "学期更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var s = await _db.EduSemesters.FindAsync(id);
        if (s == null) return ApiResponse.Error(404, "学期不存在");
        s.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("学期删除成功");
    }

    public async Task<ApiResponse> SetCurrentAsync(Guid id)
    {
        var semesters = await _db.EduSemesters.Where(s => s.IsDeleted == 0).ToListAsync();
        foreach (var s in semesters) s.IsCurrent = 0;
        var target = semesters.FirstOrDefault(s => s.Id == id);
        if (target == null) return ApiResponse.Error(404, "学期不存在");
        target.IsCurrent = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("已设为当前学期");
    }
}

public interface ICourseService
{
    Task<ApiResponse<PagedResponse<CourseDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<CourseDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<CourseDto>> CreateAsync(CreateCourseRequest request);
    Task<ApiResponse<CourseDto>> UpdateAsync(Guid id, UpdateCourseRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _db;
    public CourseService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<CourseDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.EduCourses.Where(c => c.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(c => c.Name.Contains(query.Keyword) || c.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(c => c.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(c => new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name, NameEn = c.NameEn, Nature = c.Nature,
                Credits = c.Credits, TotalHours = c.TotalHours, LectureHours = c.LectureHours, PracticeHours = c.PracticeHours,
                LabHours = c.LabHours, OnlineHours = c.OnlineHours, OpenSemesters = c.OpenSemesters, SortOrder = c.SortOrder,
                Status = c.Status, Description = c.Description, CreatedAt = c.CreatedAt }).ToListAsync();
        return ApiResponse<PagedResponse<CourseDto>>.Success(new PagedResponse<CourseDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<CourseDto>> GetByIdAsync(Guid id)
    {
        var c = await _db.EduCourses.FindAsync(id);
        if (c == null) return ApiResponse<CourseDto>.Error(404, "课程不存在");
        return ApiResponse<CourseDto>.Success(new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name, Credits = c.Credits, Status = c.Status });
    }

    public async Task<ApiResponse<CourseDto>> CreateAsync(CreateCourseRequest request)
    {
        var c = new EduCourse
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, NameEn = request.NameEn,
            Nature = request.Nature, Credits = request.Credits, TotalHours = request.TotalHours,
            LectureHours = request.LectureHours, PracticeHours = request.PracticeHours,
            LabHours = request.LabHours, OnlineHours = request.OnlineHours,
            OpenSemesters = request.OpenSemesters, SortOrder = request.SortOrder,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.EduCourses.Add(c);
        await _db.SaveChangesAsync();
        return ApiResponse<CourseDto>.Success(new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name }, "课程创建成功");
    }

    public async Task<ApiResponse<CourseDto>> UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var c = await _db.EduCourses.FindAsync(id);
        if (c == null) return ApiResponse<CourseDto>.Error(404, "课程不存在");
        if (!string.IsNullOrEmpty(request.Name)) c.Name = request.Name;
        if (!string.IsNullOrEmpty(request.NameEn)) c.NameEn = request.NameEn;
        if (request.Credits.HasValue) c.Credits = request.Credits;
        if (request.TotalHours.HasValue) c.TotalHours = request.TotalHours.Value;
        if (request.Status.HasValue) c.Status = request.Status.Value;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<CourseDto>.Success(new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name }, "课程更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var c = await _db.EduCourses.FindAsync(id);
        if (c == null) return ApiResponse.Error(404, "课程不存在");
        c.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("课程删除成功");
    }

    public async Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request)
    {
        var c = await _db.EduCourses.FindAsync(id);
        if (c == null) return ApiResponse.Error(404, "课程不存在");
        c.Status = request.IsActive ? 1 : 0;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("状态更新成功");
    }
}

public interface IMajorService
{
    Task<ApiResponse<PagedResponse<MajorDto>>> GetListAsync(PagedQueryRequest query);
    Task<ApiResponse<MajorDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<MajorDto>> CreateAsync(CreateMajorRequest request);
    Task<ApiResponse<MajorDto>> UpdateAsync(Guid id, UpdateMajorRequest request);
    Task<ApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request);
}

public class MajorService : IMajorService
{
    private readonly AppDbContext _db;
    public MajorService(AppDbContext db) { _db = db; }

    public async Task<ApiResponse<PagedResponse<MajorDto>>> GetListAsync(PagedQueryRequest query)
    {
        var q = _db.EduMajors.Include(m => m.Institution).Include(m => m.Department)
            .Where(m => m.IsDeleted == 0).AsQueryable();
        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(m => m.Name.Contains(query.Keyword) || m.Code.Contains(query.Keyword));
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(m => m.CreatedAt).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(m => new MajorDto { Id = m.Id, Code = m.Code, Name = m.Name, NameEn = m.NameEn,
                InstitutionId = m.InstitutionId, InstitutionName = m.Institution != null ? m.Institution.Name : null,
                DepartmentId = m.DepartmentId, DepartmentName = m.Department != null ? m.Department.Name : null,
                DegreeLevel = m.DegreeLevel, Duration = m.Duration, DegreeName = m.DegreeName,
                Status = m.Status, Description = m.Description, CreatedAt = m.CreatedAt }).ToListAsync();
        return ApiResponse<PagedResponse<MajorDto>>.Success(new PagedResponse<MajorDto> { Items = items, Total = total, Page = query.Page, PageSize = query.PageSize });
    }

    public async Task<ApiResponse<MajorDto>> GetByIdAsync(Guid id)
    {
        var m = await _db.EduMajors.Include(x => x.Institution).Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return ApiResponse<MajorDto>.Error(404, "专业不存在");
        return ApiResponse<MajorDto>.Success(new MajorDto { Id = m.Id, Code = m.Code, Name = m.Name, Status = m.Status });
    }

    public async Task<ApiResponse<MajorDto>> CreateAsync(CreateMajorRequest request)
    {
        var m = new EduMajor
        {
            Id = Guid.NewGuid(), Code = request.Code, Name = request.Name, NameEn = request.NameEn,
            InstitutionId = request.InstitutionId, DepartmentId = request.DepartmentId,
            DegreeLevel = request.DegreeLevel, Duration = request.Duration, DegreeName = request.DegreeName,
            Status = request.Status, Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.EduMajors.Add(m);
        await _db.SaveChangesAsync();
        return ApiResponse<MajorDto>.Success(new MajorDto { Id = m.Id, Code = m.Code, Name = m.Name }, "专业创建成功");
    }

    public async Task<ApiResponse<MajorDto>> UpdateAsync(Guid id, UpdateMajorRequest request)
    {
        var m = await _db.EduMajors.FindAsync(id);
        if (m == null) return ApiResponse<MajorDto>.Error(404, "专业不存在");
        if (!string.IsNullOrEmpty(request.Name)) m.Name = request.Name;
        if (request.Status.HasValue) m.Status = request.Status.Value;
        m.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ApiResponse<MajorDto>.Success(new MajorDto { Id = m.Id, Code = m.Code, Name = m.Name }, "专业更新成功");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var m = await _db.EduMajors.FindAsync(id);
        if (m == null) return ApiResponse.Error(404, "专业不存在");
        m.IsDeleted = 1;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("专业删除成功");
    }

    public async Task<ApiResponse> ToggleStatusAsync(Guid id, ToggleStatusRequest request)
    {
        var m = await _db.EduMajors.FindAsync(id);
        if (m == null) return ApiResponse.Error(404, "专业不存在");
        m.Status = request.IsActive ? 1 : 0;
        await _db.SaveChangesAsync();
        return ApiResponse.Success("状态更新成功");
    }
}
