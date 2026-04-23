namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Marker interface for domain events.
/// Domain events are raised within aggregate roots and dispatched after persistence.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

/// <summary>
/// Base record for domain events with auto-generated ID and timestamp.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
