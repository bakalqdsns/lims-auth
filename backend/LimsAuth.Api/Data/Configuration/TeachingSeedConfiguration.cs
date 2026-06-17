using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 教学管理 实体 HasData 种子
/// </summary>
internal static class TeachingSeedConfiguration
{
    public static void SeedTeaching(this ModelBuilder modelBuilder)
    {
        // ========== 教学管理种子数据 ==========

        // 种子数据 - 学期（完整版）
        var SemesterId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
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
                // 选课时间
                CourseSelectionStart = new DateTime(2026, 8, 20),
                CourseSelectionEnd = new DateTime(2026, 9, 5),
                CourseSelectionEndWithdraw = new DateTime(2026, 9, 15),
                // 排课时间
                SchedulingStart = new DateTime(2026, 7, 12),
                SchedulingEnd = new DateTime(2026, 8, 15),
                SchedulePublishTime = new DateTime(2026, 8, 25),
                // 考试时间
                ExamWeekStart = new DateTime(2027, 1, 6),
                ExamWeekEnd = new DateTime(2027, 1, 17),
                GradeEntryStart = new DateTime(2027, 1, 6),
                GradeEntryEnd = new DateTime(2027, 1, 24),
                GradePublishTime = new DateTime(2027, 1, 26),
                // 注册缴费
                RegistrationStart = new DateTime(2026, 8, 25),
                RegistrationEnd = new DateTime(2026, 9, 1),
                TuitionPaymentStart = new DateTime(2026, 8, 20),
                TuitionPaymentEnd = new DateTime(2026, 9, 5),
                // 状态
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

        // 种子数据 - 校历模板
        modelBuilder.Entity<CalendarTemplate>().HasData(
            new CalendarTemplate
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Name = "标准学期模板（20周）",
                Description = "适用于常规春秋学期，包含18周教学+2周考试",
                IsDefault = true,
                TemplateData = "{\"totalWeeks\":20,\"teachingWeeks\":18,\"examWeeks\":2,\"schedule\":[{\"week\":1,\"type\":\"teaching\"},{\"week\":19,\"type\":\"exam\"},{\"week\":20,\"type\":\"exam\"}]}",
                IsActive = true,
                CreatedAt = SeedDate
            },
            new CalendarTemplate
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                Name = "短学期模板（4周）",
                Description = "适用于小学期或暑期课程",
                IsDefault = false,
                TemplateData = "{\"totalWeeks\":4,\"teachingWeeks\":3,\"examWeeks\":1}",
                IsActive = true,
                CreatedAt = SeedDate
            }
        );

        // 种子数据 - 节次时间
        modelBuilder.Entity<PeriodTime>().HasData(
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000001"), PeriodNumber = 1, Name = "第1-2节", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000002"), PeriodNumber = 2, Name = "第3-4节", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000003"), PeriodNumber = 3, Name = "第5-6节", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000004"), PeriodNumber = 4, Name = "第7-8节", StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 40, 0), IsActive = true, CreatedAt = SeedDate },
            new PeriodTime { Id = Guid.Parse("e0000000-0000-0000-0000-000000000005"), PeriodNumber = 5, Name = "第9-10节", StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 40, 0), IsActive = true, CreatedAt = SeedDate }
        );

        // 种子数据 - 专业
        var MajorId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        modelBuilder.Entity<Major>().HasData(
            new Major
            {
                Id = MajorId,
                Code = "CS",
                Name = "计算机科学与技术",
                DepartmentId = CsDeptId,
                Description = "计算机科学与技术专业",
                IsActive = true,
                CreatedAt = SeedDate
            }
        );

        // 种子数据 - 班级
        var ClassId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        modelBuilder.Entity<Class>().HasData(
            new Class
            {
                Id = ClassId,
                Code = "CS202401",
                Name = "计算机科学与技术2024级1班",
                Grade = "2024",
                MajorId = MajorId,
                DepartmentId = CsDeptId,
                HeadTeacherId = TeacherUserId,
                StudentCount = 30,
                IsActive = true,
                CreatedAt = SeedDate
            }
        );

        // 种子数据 - 班级学生关联
        modelBuilder.Entity<ClassStudent>().HasData(
            new ClassStudent { ClassId = ClassId, StudentId = StudentUserId, JoinedAt = SeedDate }
        );

        // 种子数据 - 课程
        var CourseId = Guid.Parse("11111111-2222-3333-4444-555555555555");
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

        // 种子数据 - 教学任务
        var TaskId = Guid.Parse("22222222-3333-4444-5555-666666666666");
        modelBuilder.Entity<TeachingTask>().HasData(
            new TeachingTask
            {
                Id = TaskId,
                SemesterId = SemesterId,
                CourseId = CourseId,
                ClassId = ClassId,
                TaskType = "主讲",
                Description = "程序设计基础教学任务",
                IsActive = true,
                CreatedAt = SeedDate
            }
        );

        // 种子数据 - 教学任务教师关联
        modelBuilder.Entity<TeachingTaskTeacher>().HasData(
            new TeachingTaskTeacher { TeachingTaskId = TaskId, TeacherId = TeacherUserId, IsMainTeacher = true, AssignedAt = SeedDate }
        );

    }
}