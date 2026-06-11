using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

public class EduSemesterConfiguration : IEntityTypeConfiguration<EduSemester>
{
    public void Configure(EntityTypeBuilder<EduSemester> b)
    {
        b.ToTable("Edu_Semester");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("SemesterID");
        b.Property(x => x.Code).HasColumnName("SemesterCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("SemesterName").HasMaxLength(100).IsRequired();
        b.Property(x => x.SchoolYear).HasColumnName("SchoolYear").HasMaxLength(20);
        b.Property(x => x.SemesterNo).HasColumnName("SemesterNo");
        b.Property(x => x.StartDate).HasColumnName("StartDate");
        b.Property(x => x.EndDate).HasColumnName("EndDate");
        b.Property(x => x.TotalWeeks).HasColumnName("TotalWeeks");
        b.Property(x => x.IsCurrent).HasColumnName("IsCurrent").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class EduCourseConfiguration : IEntityTypeConfiguration<EduCourse>
{
    public void Configure(EntityTypeBuilder<EduCourse> b)
    {
        b.ToTable("Edu_Course");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("CourseID");
        b.Property(x => x.Code).HasColumnName("CourseCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("CourseName").HasMaxLength(200).IsRequired();
        b.Property(x => x.NameEn).HasColumnName("CourseNameEn").HasMaxLength(200);
        b.Property(x => x.Nature).HasColumnName("CourseNature").HasMaxLength(20);
        b.Property(x => x.Credits).HasColumnName("Credits").HasPrecision(10, 2);
        b.Property(x => x.TotalHours).HasColumnName("TotalHours");
        b.Property(x => x.LectureHours).HasColumnName("LectureHours");
        b.Property(x => x.PracticeHours).HasColumnName("PracticeHours");
        b.Property(x => x.LabHours).HasColumnName("LabHours");
        b.Property(x => x.OnlineHours).HasColumnName("OnlineHours");
        b.Property(x => x.OpenSemesters).HasColumnName("OpenSemesters").HasMaxLength(50);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(1000);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class EduMajorConfiguration : IEntityTypeConfiguration<EduMajor>
{
    public void Configure(EntityTypeBuilder<EduMajor> b)
    {
        b.ToTable("Edu_Major");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MajorID");
        b.Property(x => x.Code).HasColumnName("MajorCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("MajorName").HasMaxLength(200).IsRequired();
        b.Property(x => x.NameEn).HasColumnName("MajorNameEn").HasMaxLength(200);
        b.Property(x => x.InstitutionId).HasColumnName("InstitutionID");
        b.Property(x => x.DepartmentId).HasColumnName("DepartmentID");
        b.Property(x => x.DegreeLevel).HasColumnName("DegreeLevel").HasMaxLength(20);
        b.Property(x => x.Duration).HasColumnName("Duration");
        b.Property(x => x.DegreeName).HasColumnName("DegreeName").HasMaxLength(100);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Institution).WithMany().HasForeignKey(x => x.InstitutionId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Department).WithMany(x => x.Majors).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class EduClassConfiguration : IEntityTypeConfiguration<EduClass>
{
    public void Configure(EntityTypeBuilder<EduClass> b)
    {
        b.ToTable("Edu_Class");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ClassID");
        b.Property(x => x.Code).HasColumnName("ClassCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("ClassName").HasMaxLength(100).IsRequired();
        b.Property(x => x.InstitutionId).HasColumnName("InstitutionID");
        b.Property(x => x.DepartmentId).HasColumnName("DepartmentID");
        b.Property(x => x.MajorId).HasColumnName("MajorID");
        b.Property(x => x.GradeName).HasColumnName("GradeName").HasMaxLength(20);
        b.Property(x => x.MonitorId).HasColumnName("MonitorID");
        b.Property(x => x.HeadTeacherId).HasColumnName("HeadTeacherID");
        b.Property(x => x.StudentCount).HasColumnName("StudentCount");
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Institution).WithMany().HasForeignKey(x => x.InstitutionId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Department).WithMany(x => x.Classes).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Major).WithMany(x => x.Classes).HasForeignKey(x => x.MajorId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class EduClassStudentConfiguration : IEntityTypeConfiguration<EduClassStudent>
{
    public void Configure(EntityTypeBuilder<EduClassStudent> b)
    {
        b.ToTable("Edu_ClassStudent");
        b.HasKey(x => new { x.ClassId, x.StudentId });
        b.Property(x => x.ClassId).HasColumnName("ClassID");
        b.Property(x => x.StudentId).HasColumnName("StudentID");
        b.Property(x => x.JoinedAt).HasColumnName("JoinedAt");
        b.HasOne(x => x.Class).WithMany(x => x.ClassStudents).HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Student).WithMany(x => x.ClassStudents).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EduTeachingTaskConfiguration : IEntityTypeConfiguration<EduTeachingTask>
{
    public void Configure(EntityTypeBuilder<EduTeachingTask> b)
    {
        b.ToTable("Edu_TeachingTask");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("TaskID");
        b.Property(x => x.Code).HasColumnName("TaskCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.SemesterId).HasColumnName("SemesterID");
        b.Property(x => x.CourseId).HasColumnName("CourseID");
        b.Property(x => x.MajorId).HasColumnName("MajorID");
        b.Property(x => x.ClassId).HasColumnName("ClassID");
        b.Property(x => x.WeeklyHours).HasColumnName("WeeklyHours");
        b.Property(x => x.StartWeek).HasColumnName("StartWeek");
        b.Property(x => x.EndWeek).HasColumnName("EndWeek");
        b.Property(x => x.ExamMode).HasColumnName("ExamMode").HasMaxLength(50);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Semester).WithMany(x => x.TeachingTasks).HasForeignKey(x => x.SemesterId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Course).WithMany(x => x.TeachingTasks).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Major).WithMany(x => x.TeachingTasks).HasForeignKey(x => x.MajorId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Class).WithMany(x => x.TeachingTasks).HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class EduTeachingTaskTeacherConfiguration : IEntityTypeConfiguration<EduTeachingTaskTeacher>
{
    public void Configure(EntityTypeBuilder<EduTeachingTaskTeacher> b)
    {
        b.ToTable("Edu_TeachingTaskTeacher");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("ID");
        b.Property(x => x.TaskId).HasColumnName("TaskID");
        b.Property(x => x.TeacherId).HasColumnName("TeacherID");
        b.Property(x => x.Role).HasColumnName("TeacherRole").HasMaxLength(20);
        b.Property(x => x.AssignedAt).HasColumnName("AssignedAt");
        b.HasOne(x => x.Task).WithMany(x => x.Teachers).HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Cascade);
    }
}
