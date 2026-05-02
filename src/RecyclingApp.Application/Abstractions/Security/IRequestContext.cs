namespace RecyclingApp.Application.Abstractions.Security;

/// <summary>
/// Provides the current request's tenant, facility, and user context.
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
