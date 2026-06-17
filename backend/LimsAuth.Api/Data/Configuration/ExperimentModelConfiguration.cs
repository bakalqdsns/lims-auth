using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 实验/实训 实体 Fluent 配置
/// </summary>
internal static class ExperimentModelConfiguration
{
    public static void ConfigureExperimentModel(this ModelBuilder modelBuilder)
    {
        // =========================
        // ExperimentTeachingTask
        // =========================
        modelBuilder.Entity<ExperimentTeachingTask>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            // 索引（非常重要）
            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.MajorId);
            entity.HasIndex(e => e.ClassId);
            entity.HasIndex(e => e.Status);

            // 关系（手动补）
            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Major)
                .WithMany()
                .HasForeignKey(e => e.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.SetNull);

            // 一对多：Schedule
            entity.HasMany(e => e.Schedules)
                .WithOne(s => s.ExperimentTask)
                .HasForeignKey(s => s.ExperimentTaskId);

            // 一对一：质量评估
            entity.HasOne(e => e.QualityAssessment)
                .WithOne(q => q.ExperimentTask)
                .HasForeignKey<ExperimentQualityAssessment>(q => q.ExperimentTaskId);
        });


        // =========================
        // ExperimentItem
        // =========================
        modelBuilder.Entity<ExperimentItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.CourseCode);
            entity.HasIndex(e => e.ExperimentType);
            entity.HasIndex(e => e.Status);

            entity.HasMany(e => e.Schedules)
                .WithOne(s => s.ExperimentItem)
                .HasForeignKey(s => s.ExperimentItemId);
        });


        // =========================
        // ExperimentItemSchedule
        // =========================
        modelBuilder.Entity<ExperimentItemSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.ExperimentTaskId);
            entity.HasIndex(e => e.ExperimentItemId);
            entity.HasIndex(e => e.LabId);
            entity.HasIndex(e => new { e.WeekNumber, e.DayOfWeek });

            entity.HasOne(e => e.ExperimentTask)
                .WithMany(t => t.Schedules)
                .HasForeignKey(e => e.ExperimentTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ExperimentItem)
                .WithMany(i => i.Schedules)
                .HasForeignKey(e => e.ExperimentItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Lab)
                .WithMany(l => l.ExperimentSchedules)
                .HasForeignKey(e => e.LabId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        // =========================
        // ExperimentQualityAssessment
        // =========================
        modelBuilder.Entity<ExperimentQualityAssessment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.ExperimentTaskId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.ExperimentTask)
                .WithOne(t => t.QualityAssessment)
                .HasForeignKey<ExperimentQualityAssessment>(e => e.ExperimentTaskId);

            entity.HasOne(e => e.Institution)
                .WithMany()
                .HasForeignKey(e => e.InstitutionId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        // =========================
        // TrainingTeachingPlan
        // =========================
        modelBuilder.Entity<TrainingTeachingPlan>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(36);

            entity.HasIndex(e => e.SemesterId);
            entity.HasIndex(e => e.CourseId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Semester)
                .WithMany()
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Major)
                .WithMany()
                .HasForeignKey(e => e.MajorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.SetNull);
        });

    }
}