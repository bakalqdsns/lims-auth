using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 实验室/设备/校区/楼宇 实体 Fluent 配置
/// </summary>
internal static class VenueModelConfiguration
{
    public static void ConfigureVenueModel(this ModelBuilder modelBuilder)
    {
        // 实验室设备管理 - 外键关系
        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Department)
            .WithMany()
            .HasForeignKey(l => l.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Building)
            .WithMany(b => b.Labs)
            .HasForeignKey(l => l.BuildingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Manager)
            .WithMany()
            .HasForeignKey(l => l.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Lab)
            .WithMany(l => l.Equipments)
            .HasForeignKey(e => e.LabId)
            .OnDelete(DeleteBehavior.SetNull);

        // 唯一索引
        modelBuilder.Entity<Lab>()
            .HasIndex(l => l.Code)
            .IsUnique();

        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Code)
            .IsUnique();

        // 校区楼宇管理 - 外键关系
        modelBuilder.Entity<Building>()
            .HasOne(b => b.Campus)
            .WithMany(c => c.Buildings)
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Building>()
            .HasOne(b => b.Manager)
            .WithMany()
            .HasForeignKey(b => b.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Campus>()
            .HasOne(c => c.Manager)
            .WithMany()
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // 唯一索引
        modelBuilder.Entity<Campus>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Building>()
            .HasIndex(b => b.Code)
            .IsUnique();

    }
}