using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Tenancy.Domain.Entities;

/// <summary>
/// Represents a physical location (yard, branch, plant) belonging to a tenant.
/// Operational data is scoped to a facility.
/// </summary>
public class Facility : TenantEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }

    private Facility() { } // EF constructor

    public static Facility Create(Guid tenantId, string code, string name, string? address = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var facility = new Facility
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Address = address?.Trim(),
            IsActive = true
        };
        facility.SetTenant(tenantId);
        return facility;
    }

    public void Update(string name, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        Address = address?.Trim();
        MarkUpdated();
    }

    public void Deactivate() { IsActive = false; MarkUpdated(); }
    public void Activate() { IsActive = true; MarkUpdated(); }
}
