using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // System
    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<SysRole> SysRoles => Set<SysRole>();
    public DbSet<SysPermission> SysPermissions => Set<SysPermission>();
    public DbSet<SysUserRole> SysUserRoles => Set<SysUserRole>();
    public DbSet<SysRolePermission> SysRolePermissions => Set<SysRolePermission>();
    public DbSet<SysInstitution> SysInstitutions => Set<SysInstitution>();
    public DbSet<SysDepartment> SysDepartments => Set<SysDepartment>();

    // Teaching
    public DbSet<EduSemester> EduSemesters => Set<EduSemester>();
    public DbSet<EduCourse> EduCourses => Set<EduCourse>();
    public DbSet<EduMajor> EduMajors => Set<EduMajor>();
    public DbSet<EduClass> EduClasses => Set<EduClass>();
    public DbSet<EduClassStudent> EduClassStudents => Set<EduClassStudent>();
    public DbSet<EduTeachingTask> EduTeachingTasks => Set<EduTeachingTask>();
    public DbSet<EduTeachingTaskTeacher> EduTeachingTaskTeachers => Set<EduTeachingTaskTeacher>();

    // Venue
    public DbSet<VenBuilding> VenBuildings => Set<VenBuilding>();
    public DbSet<LabRoom> LabRooms => Set<LabRoom>();

    // Schedule
    public DbSet<LabSchedule> LabSchedules => Set<LabSchedule>();
    public DbSet<LabExperimentItem> LabExperimentItems => Set<LabExperimentItem>();
    public DbSet<LabBookingApply> LabBookingApplies => Set<LabBookingApply>();
    public DbSet<LabUsageRegister> LabUsageRegisters => Set<LabUsageRegister>();

    // Device
    public DbSet<DevAsset> DevAssets => Set<DevAsset>();
    public DbSet<DevLoanApply> DevLoanApplies => Set<DevLoanApply>();

    // Consumable
    public DbSet<MatConsumable> MatConsumables => Set<MatConsumable>();
    public DbSet<MatInboundOrder> MatInboundOrders => Set<MatInboundOrder>();
    public DbSet<MatOutboundOrder> MatOutboundOrders => Set<MatOutboundOrder>();
    public DbSet<MatStockLog> MatStockLogs => Set<MatStockLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
