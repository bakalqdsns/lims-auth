using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 教学管理模块：Semester / AcademicCalendar / CalendarTemplate / SemesterLog
/// Course / Major / Class / ClassStudent / TeachingTask / TeachingTaskTeacher / PeriodTime
/// </summary>
internal static class TeachingModelConfiguration
{
    public static void ConfigureTeaching(this ModelBuilder modelBuilder)
    {
        // ===== 复合主键 =====
        modelBuilder.Entity<ClassStudent>()
            .HasKey(cs => new { cs.ClassId, cs.StudentId });

        modelBuilder.Entity<TeachingTaskTeacher>()
            .HasKey(ttt => new { ttt.TeachingTaskId, ttt.TeacherId });

        // ===== 学期管理 - 外键关系 =====
        modelBuilder.Entity<Semester>()
            .HasOne(s => s.ParentSemester)
            .WithMany(s => s.ChildSemesters)
            .HasForeignKey(s => s.ParentSemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AcademicCalendar>()
            .HasOne(ac => ac.Semester)
            .WithMany(s => s.CalendarDays)
            .HasForeignKey(ac => ac.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        // ===== 教学管理 - 外键关系 =====
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

        // ===== 唯一索引 =====
        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Name).IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Code).IsUnique();

        modelBuilder.Entity<Semester>()
            .HasIndex(s => new { s.IsCurrent, s.IsActive });

        modelBuilder.Entity<Semester>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.Date }).IsUnique();

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => new { ac.SemesterId, ac.WeekNumber });

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(ac => ac.EventType);

        modelBuilder.Entity<CalendarTemplate>()
            .HasIndex(ct => ct.IsDefault);

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code).IsUnique();

        modelBuilder.Entity<Major>()
            .HasIndex(m => m.Code).IsUnique();

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.Code).IsUnique();

        modelBuilder.Entity<PeriodTime>()
            .HasIndex(pt => pt.PeriodNumber).IsUnique();

        // ===== 种子数据 =====

        modelBuilder.Entity<Semester>().HasData(
            new Semester
            {
                Id = SemesterId,
                Name = "2026-2027学年第一学期",
                Code = "2026-2027-1",
                AcademicYear = "2026-2027",
                SemesterType = SemesterType.Regular,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2027, 1, 19),
                TeachingStartDate = new DateTime(2026, 9, 2),
                TeachingEndDate = new DateTime(2027, 1, 10),
                TotalWeeks = 20,
                TeachingWeeks = 18,
                CourseSelectionStart = new DateTime(2026, 8, 20),
                CourseSelectionEnd = new DateTime(2026, 9, 5),
                CourseSelectionEndWithdraw = new DateTime(2026, 9, 15),
                SchedulingStart = new DateTime(2026, 7, 12),
                SchedulingEnd = new DateTime(2026, 8, 15),
                SchedulePublishTime = new DateTime(2026, 8, 25),
                ExamWeekStart = new DateTime(2027, 1, 6),
                ExamWeekEnd = new DateTime(2027, 1, 17),
                GradeEntryStart = new DateTime(2027, 1, 6),
                GradeEntryEnd = new DateTime(2027, 1, 24),
                GradePublishTime = new DateTime(2027, 1, 26),
                RegistrationStart = new DateTime(2026, 8, 25),
                RegistrationEnd = new DateTime(2026, 9, 1),
                TuitionPaymentStart = new DateTime(2026, 8, 20),
                TuitionPaymentEnd = new DateTime(2026, 9, 5),
                Status = SemesterStatus.InProgress,
                IsCurrent = true,
                IsActive = true,
                IsEditable = true,
                IsDeletable = false,
                Description = "2026-2027学年第一学期（秋季学期）",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        modelBuilder.Entity<CalendarTemplate>().HasData(
            new CalendarTemplate { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), Name = "标准学期模板（20周）", Description = "适用于常规春秋学期，包含18周教学+2周考试", IsDefault = true, TemplateData = "{\"totalWeeks\":20,\"teachingWeeks\":18,\"examWeeks\":2,\"schedule\":[{\"week\":1,\"type\":\"teaching\"},{\"week\":19,\"type\":\"exam\"},{\"week\":20,\"type\":\"exam\"}]}", IsActive = true, CreatedAt = SeedDate },
            new CalendarTemplate { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), Name = "短学期模板（4周）", Description = "适用于小学期或暑期课程", IsDefault = false, TemplateData = "{\"totalWeeks\":4,\"teachingWeeks\":3,\"examWeeks\":1}", IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<PeriodTime>().HasData(
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000001"), PeriodNumber = 1, Name = "第1-2节", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000002"), PeriodNumber = 2, Name = "第3-4节", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000003"), PeriodNumber = 3, Name = "第5-6节", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000004"), PeriodNumber = 4, Name = "第7-8节", StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000005"), PeriodNumber = 5, Name = "第9-10节", StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 40, 0), IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<Major>().HasData(
            new Major { Id = MajorId, Code = "CS", Name = "计算机科学与技术", DepartmentId = CsDeptId, Description = "计算机科学与技术专业", IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<Class>().HasData(
            new Class { Id = ClassId, Code = "CS202401", Name = "计算机科学与技术2024级1班", Grade = "2024", MajorId = MajorId, DepartmentId = CsDeptId, HeadTeacherId = TeacherUserId, StudentCount = 30, IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<ClassStudent>().HasData(
            new ClassStudent { ClassId = ClassId, StudentId = StudentUserId, JoinedAt = SeedDate }
        );

        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                Id = CourseId,
                Code = "CS101",
                Name = "程序设计基础",
                EnglishName = "Fundamentals of Programming",
                CourseType = "必修",
                Credits = 4,
                TotalHours = 64,
                TheoryHours = 32,
                PracticeHours = 16,
                ExperimentHours = 16,
                OnlineHours = 0,
                SemesterType = 1,
                DepartmentId = CsDeptId,
                Description = "计算机专业基础课程",
                IsActive = true,
                CreatedAt = SeedDate
            }
        );

        modelBuilder.Entity<TeachingTask>().HasData(
            new TeachingTask { Id = TaskId, SemesterId = SemesterId, CourseId = CourseId, ClassId = ClassId, TaskType = "主讲", Description = "程序设计基础教学任务", IsActive = true, CreatedAt = SeedDate }
        );

        modelBuilder.Entity<TeachingTaskTeacher>().HasData(
            new TeachingTaskTeacher { TeachingTaskId = TaskId, TeacherId = TeacherUserId, IsMainTeacher = true, AssignedAt = SeedDate }
        );
    }
}
