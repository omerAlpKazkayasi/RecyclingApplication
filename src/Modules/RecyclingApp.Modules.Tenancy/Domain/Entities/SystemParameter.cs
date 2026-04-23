using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Tenancy.Domain.Entities;

/// <summary>
/// Tenant/facility-scoped configuration parameter.
/// Used for operational settings like weight tolerance, numbering formats, etc.
/// </summary>
public class SystemParameter : EntityBase
{
    public Guid TenantId { get; private set; }
    public Guid? FacilityId { get; private set; }
    public string ParameterCode { get; private set; } = null!;
    public string ParameterValue { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    private SystemParameter() { } // EF constructor

    public static SystemParameter Create(
        Guid tenantId,
        Guid? facilityId,
        string parameterCode,
        string parameterValue,
        string? description = null,
        Guid? updatedByUserId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterValue);

        return new SystemParameter
        {
            TenantId = tenantId,
            FacilityId = facilityId,
            ParameterCode = parameterCode.Trim().ToUpperInvariant(),
            ParameterValue = parameterValue.Trim(),
            Description = description?.Trim(),
            UpdatedAt = DateTime.UtcNow,
            UpdatedByUserId = updatedByUserId
        };
    }

    public void UpdateValue(string value, Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ParameterValue = value.Trim();
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = userId;
    }
}
