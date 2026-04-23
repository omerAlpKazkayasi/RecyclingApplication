namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Base entity with UUID v7 primary key and UUID v4 public identifier.
/// All domain entities inherit from this.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Internal primary key. UUID v7 (time-ordered, index-friendly).
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// External-facing identifier. UUID v4 (random, safe to expose in APIs).
    /// </summary>
    public Guid PublicId { get; protected set; }

    protected EntityBase()
    {
        Id = UuidV7.Create();
        PublicId = Guid.NewGuid();
    }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Id == Guid.Empty || other.Id == Guid.Empty) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
