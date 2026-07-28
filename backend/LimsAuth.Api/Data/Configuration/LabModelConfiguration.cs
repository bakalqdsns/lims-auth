using Microsoft.EntityFrameworkCore;
using LimsAuth.Api.Models;
using static LimsAuth.Api.Data.Configuration.SeedKeys;

namespace LimsAuth.Api.Data.Configuration;

/// <summary>
/// 实验室与设备管理模块：Lab / Equipment / Campus / Building
/// </summary>
internal static class LabModelConfiguration
{
    public static void ConfigureLab(this ModelBuilder modelBuilder)
    {
        // ===== 实验室设备管理 - 外键关系 =====
        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Department)
            .WithMany()
            .HasForeignKey(l => l.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Building)
            .WithMany(b => b.Labs)
            .HasForeignKey(l => l.BuildingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lab>()
            .HasOne(l => l.Manager)
            .WithMany()
            .HasForeignKey(l => l.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Lab)
            .WithMany(l => l.Equipments)
            .HasForeignKey(e => e.LabId)
            .OnDelete(DeleteBehavior.SetNull);

        // ===== 校区楼宇管理 - 外键关系 =====
        modelBuilder.Entity<Building>()
            .HasOne(b => b.Campus)
            .WithMany(c => c.Buildings)
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Building>()
            .HasOne(b => b.Manager)
            .WithMany()
            .HasForeignKey(b => b.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Campus>()
            .HasOne(c => c.Manager)
            .WithMany()
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // ===== 唯一索引 =====
        modelBuilder.Entity<Lab>()
            .HasIndex(l => l.Code).IsUnique();

        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Code).IsUnique();

        modelBuilder.Entity<Campus>()
            .HasIndex(c => c.Code).IsUnique();

        modelBuilder.Entity<Building>()
            .HasIndex(b => b.Code).IsUnique();

        // ===== 种子数据 =====

        // 校区
        modelBuilder.Entity<Campus>().HasData(
            new Campus
            {
                Id = MainCampusId,
                Code = "MAIN",
                Name = "主校区",
                Address = "XX市XX区XX路1号",
                Area = 1500000,
                CampusType = "主校区",
                ContactPhone = "010-12345678",
                ManagerId = AdminUserId,
                Description = "学校主校区，包含大部分教学和实验设施",
                IsActive = true,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new Campus
            {
                Id = EastCampusId,
                Code = "EAST",
                Name = "东校区",
                Address = "XX市XX区XX路2号",
                Area = 800000,
                CampusType = "分校区",
                ContactPhone = "010-87654321",
                ManagerId = AdminUserId,
                Description = "东校区，主要用于研究生教学和部分实验",
                IsActive = true,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        );

        // 楼宇
        modelBuilder.Entity<Building>().HasData(
            new Building { Id = BuildingAId, Code = "BLD-A", Name = "实验楼A座", CampusId = MainCampusId, Address = "主校区北区", FloorCount = 5, BuildingArea = 12000, BuildingType = "实验楼", BuiltYear = 2018, ManagerId = TeacherUserId, Description = "计算机学院实验楼，配备各类计算机实验室", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new Building { Id = BuildingBId, Code = "BLD-B", Name = "实验楼B座", CampusId = MainCampusId, Address = "主校区北区", FloorCount = 4, BuildingArea = 8000, BuildingType = "实验楼", BuiltYear = 2020, ManagerId = TeacherUserId, Description = "物理、化学实验室", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new Building { Id = BuildingCId, Code = "BLD-E1", Name = "东校区实验楼", CampusId = EastCampusId, Address = "东校区中心", FloorCount = 6, BuildingArea = 15000, BuildingType = "实验楼", BuiltYear = 2022, ManagerId = TeacherUserId, Description = "东校区主要实验楼", IsActive = true, CreatedAt = SeedDate, UpdatedAt = SeedDate }
        );

        // 实验室
        modelBuilder.Entity<Lab>().HasData(
            new Lab { Id = Lab1Id, Code = "LAB001", Name = "计算机基础实验室", DepartmentId = CsDeptId, Location = "实验楼A101", Capacity = 60, LabType = "计算机实验室", SafetyLevel = "一般", ManagerId = TeacherUserId, Description = "配备高性能计算机，用于程序设计、数据结构等课程实验", IsActive = true, CreatedAt = SeedDate },
            new Lab { Id = Lab2Id, Code = "LAB002", Name = "网络工程实验室", DepartmentId = CsDeptId, BuildingId = BuildingAId, Floor = 1, RoomNumber = "A102", Location = "实验楼A座1层A102", Capacity = 40, LabType = "网络实验室", SafetyLevel = "一般", ManagerId = TeacherUserId, Description = "配备网络交换机和路由器，用于计算机网络课程实验", IsActive = true, CreatedAt = SeedDate },
            new Lab { Id = Lab3Id, Code = "LAB003", Name = "嵌入式系统实验室", DepartmentId = CsDeptId, BuildingId = BuildingAId, Floor = 2, RoomNumber = "A201", Location = "实验楼A座2层A201", Capacity = 30, LabType = "嵌入式实验室", SafetyLevel = "较高", ManagerId = TeacherUserId, Description = "配备嵌入式开发板和示波器，用于嵌入式系统课程实验", IsActive = true, CreatedAt = SeedDate }
        );

        // 设备
        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"), Code = "PC001", Name = "高性能计算机", Model = "Dell OptiPlex 7090", Manufacturer = "Dell", SerialNumber = "SN123456789", LabId = Lab1Id, Category = "计算机设备", Status = "正常", PurchaseDate = new DateTime(2023, 9, 1), WarrantyMonths = 36, Price = 8000, Location = "实验楼A101-01", RequiresBooking = false, Description = "i7处理器，32GB内存，512GB SSD", IsActive = true, CreatedAt = SeedDate },
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000002"), Code = "PC002", Name = "高性能计算机", Model = "Dell OptiPlex 7090", Manufacturer = "Dell", SerialNumber = "SN123456790", LabId = Lab1Id, Category = "计算机设备", Status = "正常", PurchaseDate = new DateTime(2023, 9, 1), WarrantyMonths = 36, Price = 8000, Location = "实验楼A101-02", RequiresBooking = false, Description = "i7处理器，32GB内存，512GB SSD", IsActive = true, CreatedAt = SeedDate },
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000003"), Code = "SW001", Name = "三层交换机", Model = "H3C S5120V3-28P-SI", Manufacturer = "H3C", SerialNumber = "SN987654321", LabId = Lab2Id, Category = "网络设备", Status = "正常", PurchaseDate = new DateTime(2023, 6, 15), WarrantyMonths = 24, Price = 5000, Location = "实验楼A102机柜A", RequiresBooking = true, MaxBookingHours = 4, Description = "24口千兆交换机，支持VLAN和路由功能", IsActive = true, CreatedAt = SeedDate },
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000004"), Code = "RT001", Name = "企业级路由器", Model = "H3C MSR3600-28", Manufacturer = "H3C", SerialNumber = "SN987654322", LabId = Lab2Id, Category = "网络设备", Status = "正常", PurchaseDate = new DateTime(2023, 6, 15), WarrantyMonths = 24, Price = 12000, Location = "实验楼A102机柜A", RequiresBooking = true, MaxBookingHours = 4, Description = "多业务路由器，支持多种路由协议", IsActive = true, CreatedAt = SeedDate },
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000005"), Code = "OSC001", Name = "数字示波器", Model = "Rigol DS1054Z", Manufacturer = "Rigol", SerialNumber = "SN555566667", LabId = Lab3Id, Category = "测试仪器", Status = "正常", PurchaseDate = new DateTime(2023, 3, 10), WarrantyMonths = 24, Price = 3500, Location = "实验楼A103仪器柜", RequiresBooking = true, MaxBookingHours = 2, Description = "4通道，50MHz带宽，1GSa/s采样率", IsActive = true, CreatedAt = SeedDate },
            new Equipment { Id = Guid.Parse("f1000000-0000-0000-0000-000000000006"), Code = "DEV001", Name = "STM32开发板", Model = "STM32F407VGT6", Manufacturer = "ST", SerialNumber = "SN777788889", LabId = Lab3Id, Category = "开发板", Status = "正常", PurchaseDate = new DateTime(2023, 9, 1), WarrantyMonths = 12, Price = 200, Location = "实验楼A103储物柜", RequiresBooking = false, Description = "ARM Cortex-M4内核，1MB Flash，192KB SRAM", IsActive = true, CreatedAt = SeedDate }
        );
    }
}
