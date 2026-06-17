using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 排课预约管理模块：ScheduleEntry / Reservation / TeachingApplication / UsageRegistration / ScheduleStatistics
/// 实体 Fluent 配置（种子数据位于 AppDbContext.cs）
/// </summary>
internal static class ScheduleModelConfiguration
{
    public static void ConfigureSchedule(this ModelBuilder modelBuilder)
    {
        // ========== ScheduleEntry（统一排课记录）==========
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

        // ========== Reservation（预约申请）==========
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

        // ========== TeachingApplication（授课申请）==========
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

            entity.HasOne(e => e.ExperimentTask)
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

        // ========== UsageRegistration（使用登记）==========
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

        // ========== ScheduleStatistics（排课统计）==========
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

        // ========== 种子数据 ==========

        // 统一排课记录
        modelBuilder.Entity<ScheduleEntry>().HasData(
            new ScheduleEntry
            {
                Id = Entry1Id,
                SemesterId = SemesterId,
                LabId = Lab1Id,
                WeekNumber = 1,
                DayOfWeek = 1,
                PeriodNumber = 1,
                Source = ScheduleSource.CentralScheduling,
                Status = "Active",
                CourseId = CourseId,
                CourseName = "程序设计基础",
                TeachingTaskId = TaskId,
                TeacherId = TeacherUserId,
                TeacherName = "张老师",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                StudentCount = 30,
                CreatedAt = SeedDate
            },
            new ScheduleEntry
            {
                Id = Entry2Id,
                SemesterId = SemesterId,
                LabId = Lab2Id,
                WeekNumber = 2,
                DayOfWeek = 2,
                PeriodNumber = 3,
                Source = ScheduleSource.CentralScheduling,
                Status = "Active",
                CourseId = CourseId,
                CourseName = "计算机网络实验",
                TeacherId = TeacherUserId,
                TeacherName = "张老师",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                StudentCount = 28,
                CreatedAt = SeedDate
            },
            new ScheduleEntry
            {
                Id = Entry3Id,
                SemesterId = SemesterId,
                LabId = Lab1Id,
                WeekNumber = 3,
                DayOfWeek = 3,
                PeriodNumber = 2,
                Source = ScheduleSource.CentralScheduling,
                Status = "Active",
                CourseId = CourseId,
                CourseName = "程序设计基础",
                TeachingTaskId = TaskId,
                TeacherId = TeacherUserId,
                TeacherName = "张老师",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                StudentCount = 30,
                CreatedAt = SeedDate
            },
            new ScheduleEntry
            {
                Id = Entry4Id,
                SemesterId = SemesterId,
                LabId = Lab3Id,
                WeekNumber = 4,
                DayOfWeek = 4,
                PeriodNumber = 4,
                Source = ScheduleSource.CentralScheduling,
                Status = "Active",
                CourseName = "嵌入式系统实验",
                TeacherId = TeacherUserId,
                TeacherName = "张老师",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                StudentCount = 25,
                CreatedAt = SeedDate
            }
        );

        // 预约申请
        modelBuilder.Entity<Reservation>().HasData(
            new Reservation
            {
                Id = Res1Id,
                SemesterId = SemesterId,
                LabId = Lab1Id,
                UseDate = new DateTime(2026, 9, 8),
                DayOfWeek = 1,
                PeriodNumbersJson = "[1, 2]",
                WeekNumber = 2,
                ExpectedDurationHours = 3,
                ProjectName = "大学生创新创业项目",
                ProjectCategory = "InnovationEntrepreneurship",
                Remark = "用于大学生创新项目小组研讨与开发",
                ApplicantId = StudentUserId,
                ApplicantName = "李同学",
                ApplicantPhone = "13800000003",
                ProjectLeaderId = StudentUserId,
                ProjectLeaderName = "李同学",
                ProjectLeaderPhone = "13800000003",
                MemberGrade = "2024",
                MemberClassName = "计算机科学与技术2024级1班",
                MemberCount = 8,
                Status = ApprovalStatus.Approved,
                ApprovalComment = "已通过，请按时使用实验室",
                ApprovedBy = AdminUserId,
                ApprovedAt = new DateTime(2026, 9, 1, 10, 0, 0),
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 8, 25, 9, 0, 0)
            },
            new Reservation
            {
                Id = Res2Id,
                SemesterId = SemesterId,
                LabId = Lab2Id,
                UseDate = new DateTime(2026, 9, 10),
                DayOfWeek = 3,
                PeriodNumbersJson = "[3, 4, 5]",
                WeekNumber = 2,
                ExpectedDurationHours = 5,
                ProjectName = "网络工程课程设计",
                ProjectCategory = "CourseTeaching",
                Remark = "课程配套实验，需使用交换机和路由器设备",
                ApplicantId = StudentUserId,
                ApplicantName = "李同学",
                ApplicantPhone = "13800000003",
                ProjectLeaderId = StudentUserId,
                ProjectLeaderName = "李同学",
                ProjectLeaderPhone = "13800000003",
                MemberGrade = "2024",
                MemberClassName = "计算机科学与技术2024级1班",
                MemberCount = 15,
                Status = ApprovalStatus.Pending,
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 9, 3, 14, 30, 0)
            },
            new Reservation
            {
                Id = Res3Id,
                SemesterId = SemesterId,
                LabId = Lab3Id,
                UseDate = new DateTime(2026, 9, 15),
                DayOfWeek = 1,
                PeriodNumbersJson = "[4, 5]",
                WeekNumber = 3,
                ExpectedDurationHours = 4,
                ProjectName = "嵌入式课程设计",
                ProjectCategory = "CourseTeaching",
                Remark = "申请使用嵌入式实验室进行STM32开发实验",
                ApplicantId = StudentUserId,
                ApplicantName = "李同学",
                ApplicantPhone = "13800000003",
                ProjectLeaderId = StudentUserId,
                ProjectLeaderName = "李同学",
                ProjectLeaderPhone = "13800000003",
                MemberGrade = "2024",
                MemberClassName = "计算机科学与技术2024级1班",
                MemberCount = 10,
                Status = ApprovalStatus.Rejected,
                ApprovalComment = "该时段已有其他教学安排，实验室不可用",
                ApprovedBy = AdminUserId,
                ApprovedAt = new DateTime(2026, 9, 4, 16, 0, 0),
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 9, 2, 11, 0, 0)
            }
        );

        // 使用登记
        modelBuilder.Entity<UsageRegistration>().HasData(
            new UsageRegistration
            {
                Id = Reg1Id,
                SemesterId = SemesterId,
                LabId = Lab1Id,
                LabName = "计算机基础实验室",
                UseDate = new DateTime(2026, 9, 1),
                WeekNumber = 1,
                DayOfWeek = 1,
                PeriodNumber = 1,
                Source = ScheduleSource.CentralScheduling,
                ScheduleEntryId = Entry1Id,
                TeachingTaskId = TaskId,
                CourseName = "程序设计基础",
                ExperimentItemName = "顺序结构程序设计",
                ExperimentItemType = "验证性实验",
                PlannedHours = 4,
                ActualHours = 3.5,
                ClassName = "计算机科学与技术2024级1班",
                ExpectedStudentCount = 30,
                ActualStudentCount = 28,
                AttendanceRecord = "2人请假",
                TeachingCondition = "良好",
                EquipmentCondition = "正常",
                Status = RegistrationStatus.Registered,
                FilledById = TeacherUserId,
                FilledByName = "张老师",
                FilledAt = new DateTime(2026, 9, 1, 12, 0, 0),
                CreatedAt = new DateTime(2026, 9, 1, 12, 0, 0)
            },
            new UsageRegistration
            {
                Id = Reg2Id,
                SemesterId = SemesterId,
                LabId = Lab2Id,
                LabName = "网络工程实验室",
                UseDate = new DateTime(2026, 9, 2),
                WeekNumber = 1,
                DayOfWeek = 2,
                PeriodNumber = 3,
                Source = ScheduleSource.TeachingRequest,
                ReservationId = Res1Id,
                ProjectName = "大学生创新创业项目",
                PlannedHours = 3,
                ActualHours = 3,
                ClassName = "计算机科学与技术2024级1班",
                ExpectedStudentCount = 8,
                ActualStudentCount = 8,
                AttendanceRecord = "全员出勤",
                TeachingCondition = "良好",
                EquipmentCondition = "正常",
                Status = RegistrationStatus.Registered,
                FilledById = StudentUserId,
                FilledByName = "李同学",
                FilledAt = new DateTime(2026, 9, 2, 17, 30, 0),
                CreatedAt = new DateTime(2026, 9, 2, 17, 30, 0)
            },
            new UsageRegistration
            {
                Id = Reg3Id,
                SemesterId = SemesterId,
                LabId = Lab1Id,
                LabName = "计算机基础实验室",
                UseDate = new DateTime(2026, 9, 8),
                WeekNumber = 2,
                DayOfWeek = 1,
                PeriodNumber = 1,
                Source = ScheduleSource.CentralScheduling,
                ScheduleEntryId = Entry1Id,
                TeachingTaskId = TaskId,
                CourseName = "程序设计基础",
                ExperimentItemName = "选择结构程序设计",
                ExperimentItemType = "验证性实验",
                PlannedHours = 4,
                ActualHours = 0,
                ClassName = "计算机科学与技术2024级1班",
                ExpectedStudentCount = 30,
                ActualStudentCount = null,
                Status = RegistrationStatus.Pending,
                FilledById = TeacherUserId,
                FilledByName = "张老师",
                FilledAt = new DateTime(2026, 9, 8, 8, 0, 0),
                CreatedAt = new DateTime(2026, 9, 8, 8, 0, 0)
            },
            new UsageRegistration
            {
                Id = Reg4Id,
                SemesterId = SemesterId,
                LabId = Lab2Id,
                LabName = "网络工程实验室",
                UseDate = new DateTime(2026, 8, 25),
                WeekNumber = -1,
                DayOfWeek = 1,
                PeriodNumber = 2,
                Source = ScheduleSource.TeachingRequest,
                CourseName = "计算机网络实验",
                PlannedHours = 4,
                ActualHours = 4,
                ClassName = "计算机科学与技术2024级1班",
                ExpectedStudentCount = 28,
                ActualStudentCount = 27,
                AttendanceRecord = "1人请假",
                TeachingCondition = "良好",
                EquipmentCondition = "正常",
                Status = RegistrationStatus.Registered,
                FilledById = TeacherUserId,
                FilledByName = "张老师",
                FilledAt = new DateTime(2026, 8, 25, 17, 0, 0),
                CreatedAt = new DateTime(2026, 8, 25, 17, 0, 0)
            }
        );

        // 授课申请
        modelBuilder.Entity<TeachingApplication>().HasData(
            new TeachingApplication
            {
                Id = Ta1Id,
                SemesterId = SemesterId,
                TeachingTaskId = TaskId,
                CourseName = "程序设计基础",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                WeekNumbersJson = "[1,2,3,4,5,6,7,8,9,10]",
                DayOfWeek = 1,
                PeriodNumbersJson = "[1, 2]",
                ExpectedLabId = Lab1Id,
                Remark = "需使用投影仪和学生用机",
                ApplicantId = TeacherUserId,
                ApplicantName = "张老师",
                Status = ApprovalStatus.Approved,
                ApprovalComment = "已通过排课安排",
                ApprovedBy = AdminUserId,
                ApprovedAt = new DateTime(2026, 8, 20, 10, 0, 0),
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 8, 15, 9, 0, 0)
            },
            new TeachingApplication
            {
                Id = Ta2Id,
                SemesterId = SemesterId,
                TeachingTaskId = TaskId,
                CourseName = "计算机网络实验",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                WeekNumbersJson = "[2,4,6,8,10,12,14,16]",
                DayOfWeek = 2,
                PeriodNumbersJson = "[3, 4, 5]",
                ExpectedLabId = Lab2Id,
                Remark = "需使用网络交换机和路由器设备，请提前检查设备状态",
                ApplicantId = TeacherUserId,
                ApplicantName = "张老师",
                Status = ApprovalStatus.Pending,
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 9, 1, 14, 0, 0)
            },
            new TeachingApplication
            {
                Id = Ta3Id,
                SemesterId = SemesterId,
                TeachingTaskId = TaskId,
                CourseName = "数据结构与算法实验",
                MajorId = MajorId,
                MajorName = "计算机科学与技术",
                ClassId = ClassId,
                ClassName = "计算机科学与技术2024级1班",
                WeekNumbersJson = "[3,5,7,9,11,13,15,17]",
                DayOfWeek = 3,
                PeriodNumbersJson = "[2, 3]",
                ExpectedLabId = Lab1Id,
                Remark = "需安装C语言和Python开发环境",
                ApplicantId = TeacherUserId,
                ApplicantName = "张老师",
                Status = ApprovalStatus.Rejected,
                ApprovalComment = "第7周和第9周与计算机基础实验室已有排课冲突，请调整周次",
                ApprovedBy = AdminUserId,
                ApprovedAt = new DateTime(2026, 9, 2, 11, 0, 0),
                IsCancelled = false,
                CreatedAt = new DateTime(2026, 9, 1, 16, 0, 0)
            }
        );
    }
}
