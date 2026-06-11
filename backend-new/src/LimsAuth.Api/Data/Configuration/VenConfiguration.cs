using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

public class VenBuildingConfiguration : IEntityTypeConfiguration<VenBuilding>
{
    public void Configure(EntityTypeBuilder<VenBuilding> b)
    {
        b.ToTable("Ven_Building");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("BuildingID");
        b.Property(x => x.Code).HasColumnName("BuildingCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("BuildingName").HasMaxLength(200).IsRequired();
        b.Property(x => x.NameEn).HasColumnName("BuildingNameEn").HasMaxLength(200);
        b.Property(x => x.InstitutionId).HasColumnName("InstitutionID");
        b.Property(x => x.Address).HasColumnName("Address").HasMaxLength(500);
        b.Property(x => x.TotalFloors).HasColumnName("TotalFloors");
        b.Property(x => x.Area).HasColumnName("Area").HasPrecision(18, 2);
        b.Property(x => x.BuildYear).HasColumnName("BuildYear");
        b.Property(x => x.UseType).HasColumnName("UseType").HasMaxLength(50);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Institution).WithMany(x => x.Buildings).HasForeignKey(x => x.InstitutionId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class VenRoomConfiguration : IEntityTypeConfiguration<LabRoom>
{
    public void Configure(EntityTypeBuilder<LabRoom> b)
    {
        b.ToTable("Ven_Room");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("RoomID");
        b.Property(x => x.Code).HasColumnName("RoomCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("RoomName").HasMaxLength(200).IsRequired();
        b.Property(x => x.BuildingId).HasColumnName("BuildingID");
        b.Property(x => x.FloorNo).HasColumnName("FloorNo");
        b.Property(x => x.RoomNumber).HasColumnName("RoomNumber").HasMaxLength(50);
        b.Property(x => x.SeatCount).HasColumnName("SeatCount");
        b.Property(x => x.Area).HasColumnName("Area").HasPrecision(18, 2);
        b.Property(x => x.RoomType).HasColumnName("RoomType").HasMaxLength(50);
        b.Property(x => x.Photo).HasColumnName("Photo").HasMaxLength(1000);
        b.Property(x => x.IsAvailable).HasColumnName("IsAvailable").HasDefaultValue(1);
        b.Property(x => x.SortOrder).HasColumnName("SortOrder").HasDefaultValue(0);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.Building).WithMany(x => x.Rooms).HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}
