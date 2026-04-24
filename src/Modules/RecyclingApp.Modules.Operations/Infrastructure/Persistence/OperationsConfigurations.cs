using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecyclingApp.Modules.Operations.Domain.Entities;

namespace RecyclingApp.Modules.Operations.Infrastructure.Persistence;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.PublicId).HasColumnName("public_id").IsRequired();
        builder.Property(t => t.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(t => t.FacilityId).HasColumnName("facility_id").IsRequired();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");

        builder.Property(t => t.TransactionNo)
            .HasColumnName("transaction_no")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.TransactionType).HasColumnName("transaction_type").IsRequired();
        builder.Property(t => t.TransactionMode).HasColumnName("transaction_mode").IsRequired();
        builder.Property(t => t.Status).HasColumnName("status").IsRequired();
        builder.Property(t => t.CustomerId).HasColumnName("customer_id").IsRequired();
        
        builder.Property(t => t.PlateNumber)
            .HasColumnName("plate_number")
            .HasMaxLength(20);

        builder.Property(t => t.OpenedByUserId).HasColumnName("opened_by_user_id").IsRequired();
        builder.Property(t => t.ApprovedByUserId).HasColumnName("approved_by_user_id");
        builder.Property(t => t.OpenedAt).HasColumnName("opened_at").IsRequired();
        builder.Property(t => t.ClosedAt).HasColumnName("closed_at");

        builder.Property(t => t.TotalWeight)
            .HasColumnName("total_weight")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(t => t.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(t => t.MismatchFlag).HasColumnName("mismatch_flag").IsRequired();
        builder.Property(t => t.ManualOverrideFlag).HasColumnName("manual_override_flag").IsRequired();
        
        builder.Property(t => t.OverrideReason)
            .HasColumnName("override_reason")
            .HasMaxLength(1000);

        builder.Property(t => t.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.HasIndex(t => t.PublicId).IsUnique();
        // Unique transaction_no per tenant/facility
        builder.HasIndex(t => new { t.TenantId, t.FacilityId, t.TransactionNo }).IsUnique();

        // One-to-Many relationships mapping back to the private collections
        builder.HasMany(t => t.Items)
            .WithOne()
            .HasForeignKey(i => i.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.VehicleScaleWeights)
            .WithOne()
            .HasForeignKey(v => v.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.InternalWeights)
            .WithOne()
            .HasForeignKey(i => i.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Checks)
            .WithOne()
            .HasForeignKey(c => c.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Map private collections
        builder.Metadata.FindNavigation(nameof(Transaction.Items))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Transaction.VehicleScaleWeights))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Transaction.InternalWeights))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Transaction.Checks))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal class TransactionItemConfiguration : IEntityTypeConfiguration<TransactionItem>
{
    public void Configure(EntityTypeBuilder<TransactionItem> builder)
    {
        builder.ToTable("transaction_items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.PublicId).HasColumnName("public_id").IsRequired();

        builder.Property(i => i.TransactionId).HasColumnName("transaction_id").IsRequired();
        builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
        
        builder.Property(i => i.QuantityWeight)
            .HasColumnName("quantity_weight")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.UnitName)
            .HasColumnName("unit_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasColumnName("unit_price")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.PricingSource)
            .HasColumnName("pricing_source")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);
            
        builder.HasIndex(i => i.PublicId).IsUnique();
    }
}

internal class VehicleScaleWeightConfiguration : IEntityTypeConfiguration<VehicleScaleWeight>
{
    public void Configure(EntityTypeBuilder<VehicleScaleWeight> builder)
    {
        builder.ToTable("vehicle_scale_weights");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).HasColumnName("id");
        builder.Property(v => v.PublicId).HasColumnName("public_id").IsRequired();

        builder.Property(v => v.TransactionId).HasColumnName("transaction_id").IsRequired();
        builder.Property(v => v.Direction).HasColumnName("direction").IsRequired();
        
        builder.Property(v => v.WeightValue)
            .HasColumnName("weight_value")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(v => v.MeasuredAt).HasColumnName("measured_at").IsRequired();
        builder.Property(v => v.MeasuredByUserId).HasColumnName("measured_by_user_id").IsRequired();
        builder.Property(v => v.SourceType).HasColumnName("source_type").IsRequired();
        builder.Property(v => v.IsValid).HasColumnName("is_valid").IsRequired();
        
        builder.Property(v => v.InvalidReason)
            .HasColumnName("invalid_reason")
            .HasMaxLength(1000);
            
        builder.HasIndex(v => v.PublicId).IsUnique();
    }
}

internal class InternalWeightConfiguration : IEntityTypeConfiguration<InternalWeight>
{
    public void Configure(EntityTypeBuilder<InternalWeight> builder)
    {
        builder.ToTable("internal_weights");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.PublicId).HasColumnName("public_id").IsRequired();

        builder.Property(i => i.TransactionId).HasColumnName("transaction_id").IsRequired();
        builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
        
        builder.Property(i => i.WeightValue)
            .HasColumnName("weight_value")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(i => i.MeasuredAt).HasColumnName("measured_at").IsRequired();
        builder.Property(i => i.MeasuredByUserId).HasColumnName("measured_by_user_id").IsRequired();
        builder.Property(i => i.SourceType).HasColumnName("source_type").IsRequired();
        builder.Property(i => i.IsCancelled).HasColumnName("is_cancelled").IsRequired();
        
        builder.Property(i => i.CancelReason)
            .HasColumnName("cancel_reason")
            .HasMaxLength(1000);

        builder.Property(i => i.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);
            
        builder.HasIndex(i => i.PublicId).IsUnique();
    }
}

internal class TransactionCheckConfiguration : IEntityTypeConfiguration<TransactionCheck>
{
    public void Configure(EntityTypeBuilder<TransactionCheck> builder)
    {
        builder.ToTable("transaction_checks");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.PublicId).HasColumnName("public_id").IsRequired();

        builder.Property(c => c.TransactionId).HasColumnName("transaction_id").IsRequired();
        
        builder.Property(c => c.ExpectedWeightDifference)
            .HasColumnName("expected_weight_difference")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(c => c.ActualWeightDifference)
            .HasColumnName("actual_weight_difference")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(c => c.InternalItemsTotalWeight)
            .HasColumnName("internal_items_total_weight")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(c => c.ToleranceValue)
            .HasColumnName("tolerance_value")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(c => c.DifferenceValue)
            .HasColumnName("difference_value")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(c => c.CheckResult).HasColumnName("check_result").IsRequired();
        builder.Property(c => c.CheckedAt).HasColumnName("checked_at").IsRequired();
        builder.Property(c => c.CheckedByUserId).HasColumnName("checked_by_user_id").IsRequired();
        
        builder.Property(c => c.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);
            
        builder.HasIndex(c => c.PublicId).IsUnique();
    }
}
