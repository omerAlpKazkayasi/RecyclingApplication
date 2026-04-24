using RecyclingApp.Modules.Operations.Domain.Enums;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Entities;

public class InternalWeight : EntityBase
{
    public Guid TransactionId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal WeightValue { get; private set; }
    public DateTime MeasuredAt { get; private set; }
    public Guid MeasuredByUserId { get; private set; }
    public WeightSourceType SourceType { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancelReason { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    protected InternalWeight() : base() { }

    internal InternalWeight(Guid transactionId, Guid productId, decimal weightValue, DateTime measuredAt, Guid measuredByUserId, WeightSourceType sourceType, string notes)
    {
        if (weightValue <= 0) throw new ArgumentException("Weight must be greater than zero.", nameof(weightValue));

        TransactionId = transactionId;
        ProductId = productId;
        WeightValue = weightValue;
        MeasuredAt = measuredAt;
        MeasuredByUserId = measuredByUserId;
        SourceType = sourceType;
        IsCancelled = false;
        Notes = notes;
    }

    internal void Cancel(string reason)
    {
        IsCancelled = true;
        CancelReason = reason;
    }
}
