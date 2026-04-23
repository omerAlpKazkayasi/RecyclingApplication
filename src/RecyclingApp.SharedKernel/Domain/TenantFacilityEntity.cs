namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Entity scoped to both tenant and facility.
/// Used for operational data that belongs to a specific physical site.
/// </summary>
public abstract class TenantFacilityEntity : TenantEntity
{
    public Guid FacilityId { get; protected set; }

    protected TenantFacilityEntity() : base() { }

    public void SetFacility(Guid facilityId)
    {
        if (FacilityId != Guid.Empty)
            throw new InvalidOperationException("Facility cannot be changed after assignment.");
        FacilityId = facilityId;
    }
}
