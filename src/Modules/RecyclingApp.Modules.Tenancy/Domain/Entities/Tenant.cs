using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Tenancy.Domain.Entities;

/// <summary>
/// Represents a business/company using the system.
/// Top-level ownership boundary for all business data.
/// </summary>
public class Tenant : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Tenant() { } // EF constructor

    public static Tenant Create(string code, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Tenant
        {
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            IsActive = true
        };
    }

    public void Update(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        MarkUpdated();
    }

    public void Deactivate() { IsActive = false; MarkUpdated(); }
    public void Activate() { IsActive = true; MarkUpdated(); }
}
