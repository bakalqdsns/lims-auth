using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

public class DevAssetConfiguration : IEntityTypeConfiguration<DevAsset>
{
    public void Configure(EntityTypeBuilder<DevAsset> b)
    {
        b.ToTable("Dev_Asset");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("AssetID");
        b.Property(x => x.Code).HasColumnName("AssetCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("DeviceName").HasMaxLength(200).IsRequired();
        b.Property(x => x.ModelNumber).HasColumnName("ModelNumber").HasMaxLength(100);
        b.Property(x => x.Specification).HasColumnName("Specification").HasMaxLength(200);
        b.Property(x => x.Category).HasColumnName("Category").HasMaxLength(50);
        b.Property(x => x.Unit).HasColumnName("Unit").HasMaxLength(20);
        b.Property(x => x.PurchaseDate).HasColumnName("PurchaseDate");
        b.Property(x => x.Brand).HasColumnName("Brand").HasMaxLength(100);
        b.Property(x => x.SerialNumber).HasColumnName("SerialNumber").HasMaxLength(100);
        b.Property(x => x.Price).HasColumnName("Price").HasPrecision(18, 2);
        b.Property(x => x.FundingSource).HasColumnName("FundingSource").HasMaxLength(100);
        b.Property(x => x.ServiceLife).HasColumnName("ServiceLife");
        b.Property(x => x.Supplier).HasColumnName("Supplier").HasMaxLength(200);
        b.Property(x => x.WarrantyPeriod).HasColumnName("WarrantyPeriod");
        b.Property(x => x.StorageLocation).HasColumnName("StorageLocation").HasMaxLength(200);
        b.Property(x => x.ResponsibleUserId).HasColumnName("ResponsibleUserID");
        b.Property(x => x.DepartmentId).HasColumnName("DepartmentID");
        b.Property(x => x.IsImportant).HasColumnName("IsImportant").HasDefaultValue(0);
        b.Property(x => x.LabelInfo).HasColumnName("LabelInfo").HasMaxLength(200);
        b.Property(x => x.PhotoPath).HasColumnName("PhotoPath").HasMaxLength(1000);
        b.Property(x => x.DeviceStatus).HasColumnName("DeviceStatus").HasMaxLength(50);
        b.Property(x => x.TotalQuantity).HasColumnName("TotalQuantity").HasDefaultValue(1);
        b.Property(x => x.AvailableQuantity).HasColumnName("AvailableQuantity").HasDefaultValue(1);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.Description).HasColumnName("Description").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasOne(x => x.ResponsibleUser).WithMany().HasForeignKey(x => x.ResponsibleUserId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class DevLoanApplyConfiguration : IEntityTypeConfiguration<DevLoanApply>
{
    public void Configure(EntityTypeBuilder<DevLoanApply> b)
    {
        b.ToTable("Dev_LoanApply");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("LoanID");
        b.Property(x => x.AssetId).HasColumnName("AssetID");
        b.Property(x => x.BorrowerId).HasColumnName("BorrowerID");
        b.Property(x => x.BorrowerType).HasColumnName("BorrowerType").HasMaxLength(20);
        b.Property(x => x.LoanPurpose).HasColumnName("LoanPurpose").HasMaxLength(1000);
        b.Property(x => x.LoanQuantity).HasColumnName("LoanQuantity").HasDefaultValue(1);
        b.Property(x => x.ExpectedReturnDate).HasColumnName("ExpectedReturnDate");
        b.Property(x => x.ActualLoanTime).HasColumnName("ActualLoanTime");
        b.Property(x => x.ActualReturnTime).HasColumnName("ActualReturnTime");
        b.Property(x => x.AuditStatus).HasColumnName("AuditStatus").HasMaxLength(20);
        b.Property(x => x.AuditorId).HasColumnName("AuditorID");
        b.Property(x => x.AuditTime).HasColumnName("AuditTime");
        b.Property(x => x.AuditOpinion).HasColumnName("AuditOpinion").HasMaxLength(500);
        b.Property(x => x.ReturnConfirmUserId).HasColumnName("ReturnConfirmUserID");
        b.Property(x => x.DeviceCondition).HasColumnName("DeviceCondition").HasMaxLength(50);
        b.Property(x => x.Remark).HasColumnName("Remark").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Asset).WithMany(x => x.LoanApplies).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Borrower).WithMany().HasForeignKey(x => x.BorrowerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Auditor).WithMany().HasForeignKey(x => x.AuditorId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ReturnConfirmUser).WithMany().HasForeignKey(x => x.ReturnConfirmUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
