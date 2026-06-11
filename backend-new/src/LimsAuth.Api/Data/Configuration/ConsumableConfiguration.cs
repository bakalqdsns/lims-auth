using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LimsAuth.Api.Models.Entities;

namespace LimsAuth.Api.Data.Configuration;

public class MatConsumableConfiguration : IEntityTypeConfiguration<MatConsumable>
{
    public void Configure(EntityTypeBuilder<MatConsumable> b)
    {
        b.ToTable("Mat_Consumable");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MatID");
        b.Property(x => x.Code).HasColumnName("MatCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasColumnName("MatName").HasMaxLength(200).IsRequired();
        b.Property(x => x.Category).HasColumnName("Category").HasMaxLength(50);
        b.Property(x => x.SpecModel).HasColumnName("SpecModel").HasMaxLength(200);
        b.Property(x => x.Unit).HasColumnName("Unit").HasMaxLength(20);
        b.Property(x => x.CurrentStock).HasColumnName("CurrentStock").HasPrecision(18, 4).HasDefaultValue(0);
        b.Property(x => x.AvailableStock).HasColumnName("AvailableStock").HasPrecision(18, 4).HasDefaultValue(0);
        b.Property(x => x.LockedStock).HasColumnName("LockedStock").HasPrecision(18, 4).HasDefaultValue(0);
        b.Property(x => x.MinStockThreshold).HasColumnName("MinStockThreshold").HasPrecision(18, 4).HasDefaultValue(0);
        b.Property(x => x.StorageLocation).HasColumnName("StorageLocation").HasMaxLength(200);
        b.Property(x => x.Supplier).HasColumnName("Supplier").HasMaxLength(200);
        b.Property(x => x.UnitPrice).HasColumnName("UnitPrice").HasPrecision(18, 2);
        b.Property(x => x.Status).HasColumnName("Status").HasDefaultValue(1);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        b.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class MatInboundOrderConfiguration : IEntityTypeConfiguration<MatInboundOrder>
{
    public void Configure(EntityTypeBuilder<MatInboundOrder> b)
    {
        b.ToTable("Mat_InboundOrder");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("InboundID");
        b.Property(x => x.Code).HasColumnName("InboundCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.ConsumableId).HasColumnName("MatID");
        b.Property(x => x.Quantity).HasColumnName("InboundQty").HasPrecision(18, 4);
        b.Property(x => x.UnitPrice).HasColumnName("UnitPrice").HasPrecision(18, 2);
        b.Property(x => x.Supplier).HasColumnName("Supplier").HasMaxLength(200);
        b.Property(x => x.InboundTime).HasColumnName("InboundTime");
        b.Property(x => x.HandlerId).HasColumnName("HandlerID");
        b.Property(x => x.Status).HasColumnName("AuditStatus").HasMaxLength(20);
        b.Property(x => x.AuditorId).HasColumnName("AuditorID");
        b.Property(x => x.AuditTime).HasColumnName("AuditTime");
        b.Property(x => x.AuditRemark).HasColumnName("AuditRemark").HasMaxLength(500);
        b.Property(x => x.Remark).HasColumnName("Remark").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Consumable).WithMany(x => x.InboundOrders).HasForeignKey(x => x.ConsumableId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Handler).WithMany().HasForeignKey(x => x.HandlerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Auditor).WithMany().HasForeignKey(x => x.AuditorId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class MatOutboundOrderConfiguration : IEntityTypeConfiguration<MatOutboundOrder>
{
    public void Configure(EntityTypeBuilder<MatOutboundOrder> b)
    {
        b.ToTable("Mat_OutboundOrder");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("OutboundID");
        b.Property(x => x.Code).HasColumnName("OutboundCode").HasMaxLength(50).IsRequired();
        b.Property(x => x.ConsumableId).HasColumnName("MatID");
        b.Property(x => x.Quantity).HasColumnName("RequestQty").HasPrecision(18, 4);
        b.Property(x => x.Purpose).HasColumnName("Purpose").HasMaxLength(1000);
        b.Property(x => x.TargetRoomId).HasColumnName("TargetRoomID");
        b.Property(x => x.ApplicantId).HasColumnName("RequesterID");
        b.Property(x => x.OutTime).HasColumnName("OutTime");
        b.Property(x => x.Status).HasColumnName("AuditStatus").HasMaxLength(20);
        b.Property(x => x.AuditorId).HasColumnName("AuditorID");
        b.Property(x => x.AuditTime).HasColumnName("AuditTime");
        b.Property(x => x.AuditRemark).HasColumnName("AuditRemark").HasMaxLength(500);
        b.Property(x => x.Remark).HasColumnName("Remark").HasMaxLength(500);
        b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(0);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
        b.HasOne(x => x.Consumable).WithMany(x => x.OutboundOrders).HasForeignKey(x => x.ConsumableId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.TargetRoom).WithMany().HasForeignKey(x => x.TargetRoomId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.Applicant).WithMany().HasForeignKey(x => x.ApplicantId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Auditor).WithMany().HasForeignKey(x => x.AuditorId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class MatStockLogConfiguration : IEntityTypeConfiguration<MatStockLog>
{
    public void Configure(EntityTypeBuilder<MatStockLog> b)
    {
        b.ToTable("Mat_StockLog");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("LogID");
        b.Property(x => x.ConsumableId).HasColumnName("MatID");
        b.Property(x => x.ChangeType).HasColumnName("ChangeType").HasMaxLength(50);
        b.Property(x => x.BeforeQuantity).HasColumnName("OldQty").HasPrecision(18, 4);
        b.Property(x => x.ChangeQuantity).HasColumnName("ChangeQty").HasPrecision(18, 4);
        b.Property(x => x.AfterQuantity).HasColumnName("NewQty").HasPrecision(18, 4);
        b.Property(x => x.ReferenceId).HasColumnName("ReferenceID");
        b.Property(x => x.ReferenceNo).HasColumnName("ReferenceNo").HasMaxLength(50);
        b.Property(x => x.OperatorId).HasColumnName("OperatorID");
        b.Property(x => x.Remark).HasColumnName("Reason").HasMaxLength(500);
        b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        b.HasOne(x => x.Consumable).WithMany(x => x.StockLogs).HasForeignKey(x => x.ConsumableId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Operator).WithMany().HasForeignKey(x => x.OperatorId).OnDelete(DeleteBehavior.Restrict);
    }
}
