using RecyclingApp.Application.Abstractions.Security;

namespace RecyclingApp.Infrastructure.Context;

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
