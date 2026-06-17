using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using LimsAuth.Api.Data.Configuration;

namespace LimsAuth.Api.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 身份认证与权限
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Department> Departments => Set<Department>();

    // 教学管理相关
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<AcademicCalendar> AcademicCalendars => Set<AcademicCalendar>();
    public DbSet<CalendarTemplate> CalendarTemplates => Set<CalendarTemplate>();
    public DbSet<SemesterLog> SemesterLogs => Set<SemesterLog>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
    public DbSet<TeachingTask> TeachingTasks => Set<TeachingTask>();
    public DbSet<TeachingTaskTeacher> TeachingTaskTeachers => Set<TeachingTaskTeacher>();
    public DbSet<PeriodTime> PeriodTimes => Set<PeriodTime>();

    // 实验室设备管理
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<EquipmentBorrowRecord> EquipmentBorrowRecords => Set<EquipmentBorrowRecord>();

    // 实验实训管理
    public DbSet<ExperimentTeachingTask> ExperimentTeachingTasks => Set<ExperimentTeachingTask>();
    public DbSet<ExperimentItem> ExperimentItems => Set<ExperimentItem>();
    public DbSet<ExperimentItemSchedule> ExperimentItemSchedules => Set<ExperimentItemSchedule>();
    public DbSet<ExperimentQualityAssessment> ExperimentQualityAssessments => Set<ExperimentQualityAssessment>();
    public DbSet<TrainingTeachingPlan> TrainingTeachingPlans => Set<TrainingTeachingPlan>();
    public DbSet<SysInstitution> SysInstitutions => Set<SysInstitution>();
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Building> Buildings => Set<Building>();

    // 排课预约管理
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<TeachingApplication> TeachingApplications => Set<TeachingApplication>();
    public DbSet<UsageRegistration> UsageRegistrations => Set<UsageRegistration>();
    public DbSet<ScheduleStatistics> ScheduleStatistics => Set<ScheduleStatistics>();

    // 耗材管理（SQLite 手工建表，参见 Program.cs）
    public DbSet<ConsumableCategory> ConsumableCategories => Set<ConsumableCategory>();
    public DbSet<Consumable> Consumables => Set<Consumable>();
    public DbSet<ConsumableInRecord> ConsumableInRecords => Set<ConsumableInRecord>();
    public DbSet<ConsumableOutRecord> ConsumableOutRecords => Set<ConsumableOutRecord>();
    public DbSet<ConsumableStockAdjustment> ConsumableStockAdjustments => Set<ConsumableStockAdjustment>();
    public DbSet<ConsumableStockLog> ConsumableStockLogs => Set<ConsumableStockLog>();
    public DbSet<ConsumableNotification> ConsumableNotifications => Set<ConsumableNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureIdentity();
        modelBuilder.ConfigureTeaching();
        modelBuilder.ConfigureLab();
        modelBuilder.ConfigureExperiment();
        modelBuilder.ConfigureSchedule();
    }
}
