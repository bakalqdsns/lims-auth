using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 教学管理 实体 Fluent 配置
/// </summary>
internal static class TeachingModelConfiguration
{
    public static void ConfigureTeachingModel(this ModelBuilder modelBuilder)
    {
        // 教学管理 - 复合主键
        modelBuilder.Entity<ClassStudent>()
            .HasKey(cs => new { cs.ClassId, cs.StudentId });

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasKey(ttt => new { ttt.TeachingTaskId, ttt.TeacherId });

        // 教学管理 - 外键关系
        modelBuilder.Entity<AcademicCalendar>()
            .HasOne(ac => ac.Semester)
            .WithMany(s => s.CalendarDays)
            .HasForeignKey(ac => ac.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Manager)
            .WithMany()
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Major>()
            .HasOne(m => m.Department)
            .WithMany()
            .HasForeignKey(m => m.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.Major)
            .WithMany(m => m.Classes)
            .HasForeignKey(c => c.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.HeadTeacher)
            .WithMany()
            .HasForeignKey(c => c.HeadTeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.AdminStudent)
            .WithMany()
            .HasForeignKey(c => c.AdminStudentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ClassStudent>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.ClassStudents)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClassStudent>()
            .HasOne(cs => cs.Student)
            .WithMany()
            .HasForeignKey(cs => cs.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Semester)
            .WithMany(s => s.TeachingTasks)
            .HasForeignKey(tt => tt.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Course)
            .WithMany(c => c.TeachingTasks)
            .HasForeignKey(tt => tt.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTask>()
            .HasOne(tt => tt.Class)
            .WithMany(c => c.TeachingTasks)
            .HasForeignKey(tt => tt.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasOne(ttt => ttt.TeachingTask)
            .WithMany(tt => tt.Teachers)
            .HasForeignKey(ttt => ttt.TeachingTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasOne(ttt => ttt.Teacher)
            .WithMany()
            .HasForeignKey(ttt => ttt.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // 学期管理 - 外键关系
        modelBuilder.Entity<Semester>()
            .HasOne(s => s.ParentSemester)
            .WithMany(s => s.ChildSemesters)
            .HasForeignKey(s => s.ParentSemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // 唯一索引
        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => new { s.IsCurrent, s.IsActive });

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.Date })
            .IsUnique();

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.WeekNumber });

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => ac.EventType);

        modelBuilder.Entity<CalendarTemplate>()
            .HasIndex(ct => ct.IsDefault);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Major>()
            .HasIndex(m => m.Code)
            .IsUnique();

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<PeriodTime>()
            .HasIndex(pt => pt.PeriodNumber)
            .IsUnique();

    }
}