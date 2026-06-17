using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 机构(SysInstitution)实体 Fluent 配置
/// </summary>
internal static class InstitutionModelConfiguration
{
    public static void ConfigureInstitutionModel(this ModelBuilder modelBuilder)
    {
        // SysInstitution 自引用（层级结构）
        modelBuilder.Entity<SysInstitution>()
            .HasOne(i => i.Parent)
            .WithMany()
            .HasForeignKey(i => i.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // SysInstitution（树结构）
        // =========================
        modelBuilder.Entity<SysInstitution>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================
        // ScheduleEntry（统一排课记录）
        // =========================

    }
}