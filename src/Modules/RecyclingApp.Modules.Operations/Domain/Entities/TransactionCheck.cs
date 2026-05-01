using RecyclingApp.Modules.Operations.Domain.Enums;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Entities;

public class TransactionCheck : EntityBase
{
    public Guid TransactionId { get; private set; }
    public decimal ExpectedWeightDifference { get; private set; }
    public decimal ActualWeightDifference { get; private set; }
    public decimal InternalItemsTotalWeight { get; private set; }
    public decimal ToleranceValue { get; private set; }
    public decimal DifferenceValue { get; private set; }
    public CheckResult CheckResult { get; private set; }
    public DateTime CheckedAt { get; private set; }
    public Guid CheckedByUserId { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    protected TransactionCheck() : base() { }

    internal TransactionCheck(Guid transactionId, decimal expected, decimal actual, decimal internalTotal, decimal tolerance, decimal difference, CheckResult result, DateTime checkedAt, Guid checkedByUserId, string notes)
    {
        TransactionId = transactionId;
        ExpectedWeightDifference = expected;
        ActualWeightDifference = actual;
        InternalItemsTotalWeight = internalTotal;
        ToleranceValue = tolerance;
        DifferenceValue = difference;
        CheckResult = result;
        CheckedAt = checkedAt;
        CheckedByUserId = checkedByUserId;
        Notes = notes;
    }
}
