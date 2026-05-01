using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Catalog.Domain.Entities;

public class PriceListItem : TenantEntity
{
    public Guid PriceListId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;

    // Navigation properties for EF Core
    public PriceList PriceList { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    protected PriceListItem() : base() { }

    public static PriceListItem Create(Guid tenantId, Guid priceListId, Guid productId, decimal unitPrice, string currencyCode)
    {
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

        var item = new PriceListItem
        {
            PriceListId = priceListId,
            ProductId = productId,
            UnitPrice = unitPrice,
            CurrencyCode = currencyCode
        };
        item.SetTenant(tenantId);
        return item;
    }

    public void UpdatePrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));
        
        UnitPrice = unitPrice;
    }
}
