using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecyclingApp.Modules.Tenancy.Domain.Entities;

namespace RecyclingApp.Modules.Tenancy.Infrastructure.Persistence;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("facilities");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(f => f.PublicId).HasColumnName("public_id").IsRequired();

        builder.Property(f => f.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(f => f.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(f => f.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(f => f.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(f => f.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(f => f.PublicId).IsUnique();
        builder.HasIndex(f => new { f.TenantId, f.Code }).IsUnique();
        builder.HasIndex(f => f.TenantId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(f => f.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<uint>("xmin").IsRowVersion();
    }
}
