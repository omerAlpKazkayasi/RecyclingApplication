namespace RecyclingApp.Infrastructure.Context;

/// <summary>
/// Provides the current request's tenant, facility, and user context.
/// In Phase 1, populated from X-Tenant-Id / X-Facility-Id / X-User-Id headers.
/// Phase 10 will replace this with real JWT-based context.
/// </summary>
public interface IRequestContext
{
    Guid TenantId { get; }
    Guid? FacilityId { get; }
    Guid? UserId { get; }
    bool HasTenant { get; }
    bool HasFacility { get; }
    bool HasUser { get; }
}

public class RequestContext : IRequestContext
{
    public Guid TenantId { get; private set; }
    public Guid? FacilityId { get; private set; }
    public Guid? UserId { get; private set; }

    public bool HasTenant => TenantId != Guid.Empty;
    public bool HasFacility => FacilityId.HasValue && FacilityId.Value != Guid.Empty;
    public bool HasUser => UserId.HasValue && UserId.Value != Guid.Empty;

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
    public void SetFacility(Guid facilityId) => FacilityId = facilityId;
    public void SetUser(Guid userId) => UserId = userId;
}
