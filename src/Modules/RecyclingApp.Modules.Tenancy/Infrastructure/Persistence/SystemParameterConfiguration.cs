using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecyclingApp.Modules.Tenancy.Domain.Entities;

namespace RecyclingApp.Modules.Tenancy.Infrastructure.Persistence;

public class SystemParameterConfiguration : IEntityTypeConfiguration<SystemParameter>
{
    public void Configure(EntityTypeBuilder<SystemParameter> builder)
    {
        builder.ToTable("system_parameters");

        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(sp => sp.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(sp => sp.FacilityId).HasColumnName("facility_id");
        builder.Property(sp => sp.ParameterCode).HasColumnName("parameter_code").HasMaxLength(100).IsRequired();
        builder.Property(sp => sp.ParameterValue).HasColumnName("parameter_value").HasMaxLength(500).IsRequired();
        builder.Property(sp => sp.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(sp => sp.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(sp => sp.UpdatedByUserId).HasColumnName("updated_by_user_id");

        builder.HasIndex(sp => new { sp.TenantId, sp.FacilityId, sp.ParameterCode }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(sp => sp.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Facility>()
            .WithMany()
            .HasForeignKey(sp => sp.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
