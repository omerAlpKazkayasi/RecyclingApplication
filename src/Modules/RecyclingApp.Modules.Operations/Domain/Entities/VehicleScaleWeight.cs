using RecyclingApp.Modules.Operations.Domain.Enums;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Entities;

public class VehicleScaleWeight : EntityBase
{
    public Guid TransactionId { get; private set; }
    public WeightDirection Direction { get; private set; }
    public decimal WeightValue { get; private set; }
    public DateTime MeasuredAt { get; private set; }
    public Guid MeasuredByUserId { get; private set; }
    public WeightSourceType SourceType { get; private set; }
    public bool IsValid { get; private set; }
    public string? InvalidReason { get; private set; }

    protected VehicleScaleWeight() : base() { }

    internal VehicleScaleWeight(Guid transactionId, WeightDirection direction, decimal weightValue, DateTime measuredAt, Guid measuredByUserId, WeightSourceType sourceType)
    {
        if (weightValue <= 0) throw new ArgumentException("Weight must be greater than zero.", nameof(weightValue));

        TransactionId = transactionId;
        Direction = direction;
        WeightValue = weightValue;
        MeasuredAt = measuredAt;
        MeasuredByUserId = measuredByUserId;
        SourceType = sourceType;
        IsValid = true;
    }

    internal void Invalidate(string reason)
    {
        IsValid = false;
        InvalidReason = reason;
    }
}
