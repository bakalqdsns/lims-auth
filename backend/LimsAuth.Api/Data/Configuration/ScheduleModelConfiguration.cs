using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 排课/预约/使用登记 实体 Fluent 配置
/// </summary>
internal static class ScheduleModelConfiguration
{
    public static void ConfigureScheduleModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.LabId);
            entity.HasIndex(e => new { e.WeekNumber, e.DayOfWeek, e.PeriodNumber });
            entity.HasIndex(e => e.Source);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Lab)
                .WithMany()
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // =========================
        // Reservation（预约申请）
        // =========================
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.LabId);
            entity.HasIndex(e => e.ApplicantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.WeekNumber);

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Lab)
                .WithMany()
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Applicant)
                .WithMany()
                .HasForeignKey(e => e.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================
        // TeachingApplication（授课申请）
        // =========================
        modelBuilder.Entity<TeachingApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.TeachingTaskId);
            entity.HasIndex(e => e.ApplicantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DayOfWeek);

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TeachingTask)
                .WithMany()
                .HasForeignKey(e => e.TeachingTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ExpectedLab)
                .WithMany()
                .HasForeignKey(e => e.ExpectedLabId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Applicant)
                .WithMany()
                .HasForeignKey(e => e.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================
        // UsageRegistration（使用登记）
        // =========================
        modelBuilder.Entity<UsageRegistration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.LabId);
            entity.HasIndex(e => e.FilledById);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.UseDate, e.PeriodNumber });

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Lab)
                .WithMany()
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.FilledBy)
                .WithMany()
                .HasForeignKey(e => e.FilledById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================
        // ScheduleStatistics（排课统计）
        // =========================
        modelBuilder.Entity<ScheduleStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.WeekNumber);
            entity.HasIndex(e => new { e.SemesterId, e.WeekNumber, e.LabId }).IsUnique();

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Lab)
                .WithMany()
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.SetNull);
        });

    }
}