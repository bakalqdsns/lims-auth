using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 机构(SysInstitution)实体 HasData 种子
/// </summary>
internal static class InstitutionSeedConfiguration
{
    public static void SeedInstitution(this ModelBuilder modelBuilder)
    {
        // =========================
        // 机构种子数据（完整层级结构）
        // =========================

        var SchoolId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
        var DeptCSId = Guid.Parse("a0000000-0000-0000-0000-000000000002");
        var DeptPhysicsId = Guid.Parse("a0000000-0000-0000-0000-000000000003");
        var DeptChemistryId = Guid.Parse("a0000000-0000-0000-0000-000000000004");

        modelBuilder.Entity<SysInstitution>().HasData(
            new SysInstitution
            {
                Id = SchoolId,
                Code = "SCHOOL",
                Name = "信息科学与工程学院",
                InstitutionType = "学院",
                Level = 1,
                FullPath = "信息科学与工程学院",
                Status = "Active",
                SortOrder = 1,
                Description = "学校信息科学与工程学院",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new SysInstitution
            {
                Id = DeptCSId,
                Code = "DEPT-CS",
                Name = "计算机系",
                ParentId = SchoolId,
                InstitutionType = "系",
                Level = 2,
                FullPath = "信息科学与工程学院/计算机系",
                Status = "Active",
                SortOrder = 1,
                Description = "计算机科学与技术系",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new SysInstitution
            {
                Id = DeptPhysicsId,
                Code = "DEPT-PHY",
                Name = "物理系",
                ParentId = SchoolId,
                InstitutionType = "系",
                Level = 2,
                FullPath = "信息科学与工程学院/物理系",
                Status = "Active",
                SortOrder = 2,
                Description = "物理学系",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new SysInstitution
            {
                Id = DeptChemistryId,
                Code = "DEPT-CHEM",
                Name = "化学系",
                ParentId = SchoolId,
                InstitutionType = "系",
                Level = 2,
                FullPath = "信息科学与工程学院/化学系",
                Status = "Active",
                SortOrder = 3,
                Description = "化学系",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new SysInstitution
            {
                Id = LabCenterId,
                Code = "LAB-CENTER",
                Name = "实验中心",
                ParentId = SchoolId,
                InstitutionType = "实验中心",
                Level = 2,
                FullPath = "信息科学与工程学院/实验中心",
                Status = "Active",
                SortOrder = 4,
                Description = "学院实验教学中心",
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

    }
}