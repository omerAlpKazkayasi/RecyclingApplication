using RecyclingApp.Modules.Catalog.Domain.Enums;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Catalog.Domain.Entities;

public class PriceList : TenantEntity
{
    public PriceListType Type { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsActive { get; private set; }

    protected PriceList() : base() { }

    public static PriceList Create(Guid tenantId, PriceListType type, string name, DateTime validFrom, DateTime? validTo)
    {
        var priceList = new PriceList
        {
            Type = type,
            Name = name,
            ValidFrom = validFrom,
            ValidTo = validTo,
            IsActive = true
        };
        priceList.SetTenant(tenantId);
        return priceList;
    }

    public void Update(string name, DateTime validFrom, DateTime? validTo)
    {
        Name = name;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
