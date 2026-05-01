using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Catalog.Domain.Entities;

public class Customer : TenantEntity
{
    public string Name { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public string IdentityNumber { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    protected Customer() : base() { }

    public static Customer Create(Guid tenantId, string name, string taxNumber, string identityNumber, string phone, string address, string notes)
    {
        var customer = new Customer
        {
            Name = name,
            TaxNumber = taxNumber,
            IdentityNumber = identityNumber,
            Phone = phone,
            Address = address,
            Notes = notes,
            IsActive = true
        };
        customer.SetTenant(tenantId);
        return customer;
    }

    public void Update(string name, string taxNumber, string identityNumber, string phone, string address, string notes)
    {
        Name = name;
        TaxNumber = taxNumber;
        IdentityNumber = identityNumber;
        Phone = phone;
        Address = address;
        Notes = notes;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
