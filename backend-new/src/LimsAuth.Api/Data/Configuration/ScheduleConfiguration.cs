using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

public class LabScheduleConfiguration : IEntityTypeConfiguration<LabSchedule>
{
    public void Configure(EntityTypeBuilder<LabSchedule> b)
    {
        b.ToTable("Lab_Schedule");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ScheduleID");
        b.Property(x => x.SemesterId).HasColumnName("SemesterID");
        b.Property(x => x.RoomId).HasColumnName("RoomID");
        b.Property(x => x.TeachingTaskId).HasColumnName("TaskID");
        b.Property(x => x.WeekNo).HasColumnName("WeekNo");
        b.Property(x => x.DayOfWeek).HasColumnName("DayOfWeek");
        b.Property(x => x.SectionNo).HasColumnName("SectionNo").HasMaxLength(50);
        b.Property(x => x.CourseName).HasColumnName("CourseName").HasMaxLength(200);
        b.Property(x => x.ClassId).HasColumnName("ClassID");
        b.Property(x => x.TeacherId).HasColumnName("TeacherID");
        b.Property(x => x.ExperimentItemId).HasColumnName("ExperimentItemID");
        b.Property(x => x.ScheduleType).HasColumnName("ScheduleType").HasMaxLength(50);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Remark).HasColumnName("Remark").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.HasOne(x => x.Semester).WithMany().HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Room).WithMany(x => x.Schedules).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.TeachingTask).WithMany().HasForeignKey(x => x.TeachingTaskId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ExperimentItem).WithMany(x => x.Schedules).HasForeignKey(x => x.ExperimentItemId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class LabExperimentItemConfiguration : IEntityTypeConfiguration<LabExperimentItem>
{
    public void Configure(EntityTypeBuilder<LabExperimentItem> b)
    {
        b.ToTable("Lab_ExperimentItem");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ItemID");
        b.Property(x => x.Code).HasColumnName("ItemCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("ItemName").HasMaxLength(200).IsRequired();
        b.Property(x => x.CourseId).HasColumnName("CourseID");
        b.Property(x => x.ExperimentType).HasColumnName("ExperimentType").HasMaxLength(50);
        b.Property(x => x.StandardHours).HasColumnName("StandardHours");
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Course).WithMany(x => x.ExperimentItems).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class LabBookingApplyConfiguration : IEntityTypeConfiguration<LabBookingApply>
{
    public void Configure(EntityTypeBuilder<LabBookingApply> b)
    {
        b.ToTable("Lab_BookingApply");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ApplyID");
        b.Property(x => x.Code).HasColumnName("ApplyCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.ApplicantId).HasColumnName("ApplicantID");
        b.Property(x => x.ApplicantType).HasColumnName("ApplicantType").HasMaxLength(20);
        b.Property(x => x.RoomId).HasColumnName("RoomID");
        b.Property(x => x.Purpose).HasColumnName("Purpose").HasMaxLength(1000);
        b.Property(x => x.TargetDate).HasColumnName("TargetDate");
        b.Property(x => x.TargetSection).HasColumnName("TargetSection").HasMaxLength(50);
        b.Property(x => x.WeekNo).HasColumnName("WeekNo");
        b.Property(x => x.EstimatedPeople).HasColumnName("EstimatedPeople");
        b.Property(x => x.AuditStatus).HasColumnName("AuditStatus").HasMaxLength(20);
        b.Property(x => x.AuditOpinion).HasColumnName("AuditOpinion").HasMaxLength(500);
        b.Property(x => x.AuditorId).HasColumnName("AuditorID");
        b.Property(x => x.AuditTime).HasColumnName("AuditTime");
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Applicant).WithMany().HasForeignKey(x => x.ApplicantId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Room).WithMany(x => x.BookingApplies).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Auditor).WithMany().HasForeignKey(x => x.AuditorId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class LabUsageRegisterConfiguration : IEntityTypeConfiguration<LabUsageRegister>
{
    public void Configure(EntityTypeBuilder<LabUsageRegister> b)
    {
        b.ToTable("Lab_UsageRegister");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("RegisterID");
        b.Property(x => x.ScheduleId).HasColumnName("ScheduleID");
        b.Property(x => x.BookingApplyId).HasColumnName("BookingApplyID");
        b.Property(x => x.SemesterId).HasColumnName("SemesterID");
        b.Property(x => x.RoomId).HasColumnName("RoomID");
        b.Property(x => x.ItemName).HasColumnName("ItemName").HasMaxLength(200);
        b.Property(x => x.ExperimentType).HasColumnName("ExperimentType").HasMaxLength(50);
        b.Property(x => x.PlannedHours).HasColumnName("PlannedHours");
        b.Property(x => x.ActualHours).HasColumnName("ActualHours").HasPrecision(10, 2);
        b.Property(x => x.ClassName).HasColumnName("ClassName").HasMaxLength(100);
        b.Property(x => x.ExpectedCount).HasColumnName("ExpectedCount");
        b.Property(x => x.ActualCount).HasColumnName("ActualCount");
        b.Property(x => x.AttendanceRecord).HasColumnName("AttendanceRecord").HasMaxLength(500);
        b.Property(x => x.TeachingRecord).HasColumnName("TeachingRecord").HasMaxLength(200);
        b.Property(x => x.DeviceRecord).HasColumnName("DeviceRecord").HasMaxLength(200);
        b.Property(x => x.RegisterStatus).HasColumnName("RegisterStatus").HasMaxLength(20);
        b.Property(x => x.RegisterUserId).HasColumnName("RegisterUserID");
        b.Property(x => x.RegisterTime).HasColumnName("RegisterTime");
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Schedule).WithMany(x => x.UsageRegisters).HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.BookingApply).WithMany(x => x.UsageRegisters).HasForeignKey(x => x.BookingApplyId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Semester).WithMany().HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Room).WithMany(x => x.UsageRegisters).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.RegisterUser).WithMany().HasForeignKey(x => x.RegisterUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
