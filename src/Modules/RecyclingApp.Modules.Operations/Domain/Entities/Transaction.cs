using RecyclingApp.Modules.Operations.Domain.Enums;
using RecyclingApp.Modules.Operations.Domain.Events;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Entities;

public class Transaction : TenantFacilityEntity
{
    public string TransactionNo { get; private set; } = string.Empty;
    public TransactionType TransactionType { get; private set; }
    public TransactionMode TransactionMode { get; private set; }
    public TransactionStatus Status { get; private set; }
    public Guid CustomerId { get; private set; }
    public string? PlateNumber { get; private set; }
    public Guid OpenedByUserId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    
    public decimal TotalWeight { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool MismatchFlag { get; private set; }
    public bool ManualOverrideFlag { get; private set; }
    public string? OverrideReason { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    private readonly List<TransactionItem> _items = new();
    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();

    private readonly List<VehicleScaleWeight> _vehicleScaleWeights = new();
    public IReadOnlyCollection<VehicleScaleWeight> VehicleScaleWeights => _vehicleScaleWeights.AsReadOnly();

    private readonly List<InternalWeight> _internalWeights = new();
    public IReadOnlyCollection<InternalWeight> InternalWeights => _internalWeights.AsReadOnly();

    private readonly List<TransactionCheck> _checks = new();
    public IReadOnlyCollection<TransactionCheck> Checks => _checks.AsReadOnly();

    protected Transaction() : base() { }

    private Transaction(Guid tenantId, Guid facilityId, string transactionNo, TransactionType type, TransactionMode mode, Guid customerId, string? plateNumber, Guid openedByUserId, string notes)
    {
        if (string.IsNullOrWhiteSpace(transactionNo)) throw new ArgumentException("Transaction number is required.", nameof(transactionNo));
        if (tenantId == Guid.Empty) throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        if (facilityId == Guid.Empty) throw new ArgumentException("Facility ID is required.", nameof(facilityId));
        
        SetTenant(tenantId);
        SetFacility(facilityId);
        
        TransactionNo = transactionNo;
        TransactionType = type;
        TransactionMode = mode;
        Status = TransactionStatus.Created;
        CustomerId = customerId;
        PlateNumber = plateNumber;
        OpenedByUserId = openedByUserId;
        OpenedAt = DateTime.UtcNow;
        Notes = notes ?? string.Empty;

        AddDomainEvent(new TransactionCreatedEvent(Id, tenantId));
    }

    public static Transaction CreateVehiclePurchase(Guid tenantId, Guid facilityId, string transactionNo, Guid customerId, string plateNumber, Guid openedByUserId, string notes = "")
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) throw new ArgumentException("Plate number is required for vehicle transactions.", nameof(plateNumber));
        return new Transaction(tenantId, facilityId, transactionNo, TransactionType.Purchase, TransactionMode.Vehicle, customerId, plateNumber, openedByUserId, notes);
    }

    public static Transaction CreateVehicleSale(Guid tenantId, Guid facilityId, string transactionNo, Guid customerId, string plateNumber, Guid openedByUserId, string notes = "")
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) throw new ArgumentException("Plate number is required for vehicle transactions.", nameof(plateNumber));
        return new Transaction(tenantId, facilityId, transactionNo, TransactionType.Sale, TransactionMode.Vehicle, customerId, plateNumber, openedByUserId, notes);
    }

    public static Transaction CreateManualPurchase(Guid tenantId, Guid facilityId, string transactionNo, Guid customerId, Guid openedByUserId, string notes = "")
    {
        return new Transaction(tenantId, facilityId, transactionNo, TransactionType.Purchase, TransactionMode.Manual, customerId, null, openedByUserId, notes);
    }

    public static Transaction CreateManualSale(Guid tenantId, Guid facilityId, string transactionNo, Guid customerId, Guid openedByUserId, string notes = "")
    {
        return new Transaction(tenantId, facilityId, transactionNo, TransactionType.Sale, TransactionMode.Manual, customerId, null, openedByUserId, notes);
    }

    private void EnsureNotTerminal()
    {
        if (Status == TransactionStatus.Completed || Status == TransactionStatus.Cancelled)
            throw new InvalidOperationException($"Cannot modify transaction in terminal state {Status}.");
    }

    public void AddTransactionItem(Guid productId, decimal quantityWeight, string unitName, decimal unitPrice, string pricingSource, string notes = "")
    {
        EnsureNotTerminal();

        var item = new TransactionItem(Id, productId, quantityWeight, unitName, unitPrice, pricingSource, notes);
        _items.Add(item);
        
        RecalculateTotals();
        
        AddDomainEvent(new TransactionItemAddedEvent(Id, productId, item.Amount));
    }

    public void RecordEntryWeight(decimal weightValue, Guid measuredByUserId, WeightSourceType sourceType)
    {
        EnsureNotTerminal();
        if (TransactionMode != TransactionMode.Vehicle)
            throw new InvalidOperationException("Cannot record vehicle weights for manual transactions.");

        if (_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Entry && w.IsValid))
            throw new InvalidOperationException("A valid entry weight already exists.");

        if (_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Exit && w.IsValid))
            throw new InvalidOperationException("Cannot record entry weight after an exit weight has been recorded.");

        var weight = new VehicleScaleWeight(Id, WeightDirection.Entry, weightValue, DateTime.UtcNow, measuredByUserId, sourceType);
        _vehicleScaleWeights.Add(weight);

        if (Status == TransactionStatus.Created)
            Status = TransactionStatus.EntryWeighed;

        AddDomainEvent(new EntryWeightRecordedEvent(Id, weightValue));
    }

    public void RecordExitWeight(decimal weightValue, Guid measuredByUserId, WeightSourceType sourceType)
    {
        EnsureNotTerminal();
        if (TransactionMode != TransactionMode.Vehicle)
            throw new InvalidOperationException("Cannot record vehicle weights for manual transactions.");

        if (!_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Entry && w.IsValid))
            throw new InvalidOperationException("Must record entry weight before exit weight.");

        if (_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Exit && w.IsValid))
            throw new InvalidOperationException("A valid exit weight already exists.");

        var weight = new VehicleScaleWeight(Id, WeightDirection.Exit, weightValue, DateTime.UtcNow, measuredByUserId, sourceType);
        _vehicleScaleWeights.Add(weight);

        Status = TransactionStatus.ExitWeighed;

        AddDomainEvent(new ExitWeightRecordedEvent(Id, weightValue));
    }

    public void RecordInternalWeight(Guid productId, decimal weightValue, Guid measuredByUserId, WeightSourceType sourceType, string notes = "")
    {
        EnsureNotTerminal();

        var weight = new InternalWeight(Id, productId, weightValue, DateTime.UtcNow, measuredByUserId, sourceType, notes);
        _internalWeights.Add(weight);

        if (Status == TransactionStatus.EntryWeighed)
            Status = TransactionStatus.InternalWeighingInProgress;

        AddDomainEvent(new InternalWeightRecordedEvent(Id, weightValue));
    }

    public void RequestValidation()
    {
        EnsureNotTerminal();

        if (TransactionMode == TransactionMode.Vehicle)
        {
            if (!_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Entry && w.IsValid) ||
                !_vehicleScaleWeights.Any(w => w.Direction == WeightDirection.Exit && w.IsValid))
            {
                throw new InvalidOperationException("Vehicle transactions require both entry and exit weights before validation.");
            }
        }

        if (!_items.Any())
            throw new InvalidOperationException("Cannot validate transaction without items.");

        Status = TransactionStatus.PendingValidation;
        AddDomainEvent(new ValidationRequestedEvent(Id));
    }

    public void ApplyValidationResult(decimal expectedDifference, decimal actualDifference, decimal tolerance, string notes, Guid checkedByUserId)
    {
        EnsureNotTerminal();
        if (Status != TransactionStatus.PendingValidation)
            throw new InvalidOperationException("Transaction must be in PendingValidation state to apply validation result.");

        var diffValue = Math.Abs(expectedDifference - actualDifference);
        var checkResult = diffValue <= tolerance ? CheckResult.Pass : CheckResult.NeedsApproval;

        var check = new TransactionCheck(Id, expectedDifference, actualDifference, TotalWeight, tolerance, diffValue, checkResult, DateTime.UtcNow, checkedByUserId, notes);
        _checks.Add(check);

        if (checkResult == CheckResult.Pass)
        {
            Status = TransactionStatus.Validated;
            MismatchFlag = false;
        }
        else
        {
            MismatchFlag = true;
        }

        AddDomainEvent(new TransactionValidatedEvent(Id, checkResult == CheckResult.Pass));
    }

    public void ApproveValidationOverride(Guid approvedByUserId, string reason)
    {
        EnsureNotTerminal();
        
        var lastCheck = _checks.OrderByDescending(c => c.CheckedAt).FirstOrDefault();
        if (lastCheck == null || lastCheck.CheckResult != CheckResult.NeedsApproval)
            throw new InvalidOperationException("Validation override is only allowed when the latest check requires approval.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Override reason is required.", nameof(reason));

        Status = TransactionStatus.Validated;
        ManualOverrideFlag = true;
        OverrideReason = reason;
        ApprovedByUserId = approvedByUserId;

        AddDomainEvent(new ValidationOverriddenEvent(Id, approvedByUserId));
    }

    public void Complete()
    {
        EnsureNotTerminal();
        
        if (Status != TransactionStatus.Validated)
            throw new InvalidOperationException("Transaction must be validated before it can be completed.");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot complete a transaction without items.");

        Status = TransactionStatus.Completed;
        ClosedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionCompletedEvent(Id));
    }

    public void Cancel(string reason)
    {
        EnsureNotTerminal();
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));

        Status = TransactionStatus.Cancelled;
        ClosedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(Notes) ? $"Cancelled: {reason}" : $"{Notes} | Cancelled: {reason}";

        AddDomainEvent(new TransactionCancelledEvent(Id));
    }

    private void RecalculateTotals()
    {
        TotalWeight = _items.Sum(i => i.QuantityWeight);
        TotalAmount = _items.Sum(i => i.Amount);
    }
}
