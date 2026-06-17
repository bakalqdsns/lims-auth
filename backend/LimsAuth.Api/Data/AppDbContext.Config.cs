using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Data.Configuration;

namespace LimsAuth.Api.Data;

/// <summary>
/// 数据库上下文 - 配置调度入口
/// 各业务模块的 Fluent API 配置与种子数据按职责拆分到 Configuration/*ModelConfiguration.cs 与 Configuration/*SeedConfiguration.cs
/// </summary>
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureSystemModel();
        modelBuilder.SeedSystem();

        modelBuilder.ConfigureTeachingModel();
        modelBuilder.SeedTeaching();

        modelBuilder.ConfigureVenueModel();
        modelBuilder.SeedVenue();

        modelBuilder.ConfigureInstitutionModel();
        modelBuilder.SeedInstitution();

        modelBuilder.ConfigureExperimentModel();
        modelBuilder.SeedExperiment();

        modelBuilder.ConfigureScheduleModel();
        modelBuilder.SeedSchedule();
    }
}
