namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Entity scoped to a tenant. All tenant-aware business data inherits from this.
/// </summary>
public abstract class TenantEntity : AuditableEntity
{
    public Guid TenantId { get; protected set; }

    protected TenantEntity() : base() { }

    public void SetTenant(Guid tenantId)
    {
        if (TenantId != Guid.Empty)
            throw new InvalidOperationException("Tenant cannot be changed after assignment.");
        TenantId = tenantId;
    }
}
