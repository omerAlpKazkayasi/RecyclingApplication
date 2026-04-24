using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Operations.Domain.Events;

public record TransactionCreatedEvent(Guid TransactionId, Guid TenantId) : DomainEventBase;

public record EntryWeightRecordedEvent(Guid TransactionId, decimal WeightValue) : DomainEventBase;

public record ExitWeightRecordedEvent(Guid TransactionId, decimal WeightValue) : DomainEventBase;

public record InternalWeightRecordedEvent(Guid TransactionId, decimal WeightValue) : DomainEventBase;

public record TransactionItemAddedEvent(Guid TransactionId, Guid ProductId, decimal Amount) : DomainEventBase;

public record ValidationRequestedEvent(Guid TransactionId) : DomainEventBase;

public record TransactionValidatedEvent(Guid TransactionId, bool Passed) : DomainEventBase;

public record ValidationOverriddenEvent(Guid TransactionId, Guid ApprovedByUserId) : DomainEventBase;

public record TransactionCompletedEvent(Guid TransactionId) : DomainEventBase;

public record TransactionCancelledEvent(Guid TransactionId) : DomainEventBase;
