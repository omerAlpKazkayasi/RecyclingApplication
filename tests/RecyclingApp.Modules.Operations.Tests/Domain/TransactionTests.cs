using FluentAssertions;
using RecyclingApp.Modules.Operations.Domain.Entities;
using RecyclingApp.Modules.Operations.Domain.Enums;
using Xunit;

namespace RecyclingApp.Modules.Operations.Tests.Domain;

public class TransactionTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _facilityId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    [Fact]
    public void CreateVehiclePurchase_WithValidData_CreatesTransaction()
    {
        var transaction = Transaction.CreateVehiclePurchase(_tenantId, _facilityId, "TRX-001", _customerId, "34 ABC 123", _userId);

        transaction.TransactionNo.Should().Be("TRX-001");
        transaction.TransactionType.Should().Be(TransactionType.Purchase);
        transaction.TransactionMode.Should().Be(TransactionMode.Vehicle);
        transaction.PlateNumber.Should().Be("34 ABC 123");
        transaction.Status.Should().Be(TransactionStatus.Created);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateVehiclePurchase_WithInvalidPlateNumber_ThrowsException(string? invalidPlate)
    {
        var action = () => Transaction.CreateVehiclePurchase(_tenantId, _facilityId, "TRX-001", _customerId, invalidPlate!, _userId);
        action.Should().Throw<ArgumentException>().WithParameterName("plateNumber");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateManualPurchase_WithInvalidTransactionNo_ThrowsException(string? invalidNo)
    {
        var action = () => Transaction.CreateManualPurchase(_tenantId, _facilityId, invalidNo!, _customerId, _userId);
        action.Should().Throw<ArgumentException>().WithParameterName("transactionNo");
    }

    [Fact]
    public void CreateManualPurchase_WithEmptyTenantId_ThrowsException()
    {
        var action = () => Transaction.CreateManualPurchase(Guid.Empty, _facilityId, "TRX", _customerId, _userId);
        action.Should().Throw<ArgumentException>().WithParameterName("tenantId");
    }

    [Fact]
    public void CreateManualPurchase_WithEmptyFacilityId_ThrowsException()
    {
        var action = () => Transaction.CreateManualPurchase(_tenantId, Guid.Empty, "TRX", _customerId, _userId);
        action.Should().Throw<ArgumentException>().WithParameterName("facilityId");
    }

    [Fact]
    public void AddTransactionItem_CalculatesAmountAndUpdatesTotals()
    {
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);

        transaction.AddTransactionItem(_productId, 100m, "kg", 5m, "Manual");
        transaction.AddTransactionItem(_productId, 50m, "kg", 10m, "Manual");

        transaction.Items.Should().HaveCount(2);
        transaction.Items.First().Amount.Should().Be(500m);
        transaction.Items.Last().Amount.Should().Be(500m);

        transaction.TotalWeight.Should().Be(150m);
        transaction.TotalAmount.Should().Be(1000m);
    }

    [Fact]
    public void VehicleTransaction_CannotRecordExitWeightBeforeEntryWeight()
    {
        var transaction = Transaction.CreateVehiclePurchase(_tenantId, _facilityId, "TRX", _customerId, "PLATE", _userId);

        var action = () => transaction.RecordExitWeight(1000m, _userId, WeightSourceType.Manual);
        action.Should().Throw<InvalidOperationException>().WithMessage("*Must record entry weight before exit weight.*");
    }

    [Fact]
    public void ManualTransaction_CannotRecordVehicleWeights()
    {
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);

        var action1 = () => transaction.RecordEntryWeight(1000m, _userId, WeightSourceType.Manual);
        var action2 = () => transaction.RecordExitWeight(1000m, _userId, WeightSourceType.Manual);

        action1.Should().Throw<InvalidOperationException>();
        action2.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void VehicleTransaction_RequestValidation_FailsWithoutBothWeights()
    {
        var transaction = Transaction.CreateVehiclePurchase(_tenantId, _facilityId, "TRX", _customerId, "PLATE", _userId);
        transaction.RecordEntryWeight(2000m, _userId, WeightSourceType.Manual);
        transaction.AddTransactionItem(_productId, 1000m, "kg", 1m, "Manual");

        var action = () => transaction.RequestValidation();
        action.Should().Throw<InvalidOperationException>().WithMessage("*Vehicle transactions require both entry and exit weights*");
    }

    [Fact]
    public void Complete_WithoutValidation_ThrowsException()
    {
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);
        transaction.AddTransactionItem(_productId, 1000m, "kg", 1m, "Manual");

        var action = () => transaction.Complete();
        action.Should().Throw<InvalidOperationException>().WithMessage("*must be validated*");
    }

    [Fact]
    public void TerminalState_RejectsMutations()
    {
        // Setup validated manual transaction
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);
        transaction.AddTransactionItem(_productId, 1000m, "kg", 1m, "Manual");
        transaction.RequestValidation();
        transaction.ApplyValidationResult(1000m, 1000m, 50m, "OK", _userId);
        
        // Complete
        transaction.Complete();
        transaction.Status.Should().Be(TransactionStatus.Completed);

        // Mutations should throw
        var addAction = () => transaction.AddTransactionItem(_productId, 100m, "kg", 1m, "Manual");
        var internalWAction = () => transaction.RecordInternalWeight(_productId, 100m, _userId, WeightSourceType.Manual);
        var cancelAction = () => transaction.Cancel("Reason");

        addAction.Should().Throw<InvalidOperationException>().WithMessage("*Cannot modify transaction in terminal state*");
        internalWAction.Should().Throw<InvalidOperationException>();
        cancelAction.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ApproveValidationOverride_WithoutNeedsApproval_ThrowsException()
    {
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);
        transaction.AddTransactionItem(_productId, 1000m, "kg", 1m, "Manual");
        transaction.RequestValidation();
        
        // Validation Passes
        transaction.ApplyValidationResult(1000m, 1000m, 50m, "OK", _userId);

        var action = () => transaction.ApproveValidationOverride(_userId, "Because");
        action.Should().Throw<InvalidOperationException>().WithMessage("*only allowed when the latest check requires approval*");
    }

    [Fact]
    public void ApproveValidationOverride_WithNeedsApproval_Succeeds()
    {
        var transaction = Transaction.CreateManualPurchase(_tenantId, _facilityId, "TRX", _customerId, _userId);
        transaction.AddTransactionItem(_productId, 1000m, "kg", 1m, "Manual");
        transaction.RequestValidation();
        
        // Validation Fails -> Needs Approval
        transaction.ApplyValidationResult(1000m, 1500m, 50m, "Mismatch", _userId);

        transaction.Status.Should().Be(TransactionStatus.PendingValidation);
        transaction.MismatchFlag.Should().BeTrue();

        transaction.ApproveValidationOverride(_userId, "Approved by Manager");

        transaction.Status.Should().Be(TransactionStatus.Validated);
        transaction.ManualOverrideFlag.Should().BeTrue();
        transaction.OverrideReason.Should().Be("Approved by Manager");
    }
}
