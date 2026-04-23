namespace RecyclingApp.SharedKernel.Domain;

/// <summary>
/// Unit of work abstraction for transactional persistence.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
