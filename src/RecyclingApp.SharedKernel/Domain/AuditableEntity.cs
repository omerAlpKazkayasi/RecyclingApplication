namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Entity with created/updated audit timestamps.
/// </summary>
public abstract class AuditableEntity : EntityBase
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected AuditableEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
