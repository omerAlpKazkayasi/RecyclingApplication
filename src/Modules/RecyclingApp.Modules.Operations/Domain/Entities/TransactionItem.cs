using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Entities;

public class TransactionItem : EntityBase
{
    public Guid TransactionId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal QuantityWeight { get; private set; }
    public string UnitName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal Amount { get; private set; }
    public string PricingSource { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    protected TransactionItem() : base() { }

    internal TransactionItem(Guid transactionId, Guid productId, decimal quantityWeight, string unitName, decimal unitPrice, string pricingSource, string notes)
    {
        if (quantityWeight <= 0) throw new ArgumentException("Quantity/Weight must be greater than zero.", nameof(quantityWeight));
        if (unitPrice <= 0) throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

        TransactionId = transactionId;
        ProductId = productId;
        QuantityWeight = quantityWeight;
        UnitName = unitName;
        UnitPrice = unitPrice;
        Amount = quantityWeight * unitPrice;
        PricingSource = pricingSource;
        Notes = notes;
    }
}
