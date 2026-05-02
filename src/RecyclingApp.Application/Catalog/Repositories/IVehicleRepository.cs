using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Repositories;

public interface IVehicleRepository
{
    void Add(Vehicle vehicle);
    void Remove(Vehicle vehicle);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default);
    Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);
}
