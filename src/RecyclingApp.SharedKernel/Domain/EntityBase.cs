namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Base entity with a single Guid primary key.
/// All domain entities inherit from this.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Internal primary key.
    /// </summary>
    public Guid Id { get; protected set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
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
