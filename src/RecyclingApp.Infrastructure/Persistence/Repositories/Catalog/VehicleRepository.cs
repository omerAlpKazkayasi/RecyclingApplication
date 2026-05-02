using Microsoft.EntityFrameworkCore;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Infrastructure.Persistence.Repositories.Catalog;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _dbContext;

    public VehicleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Vehicle vehicle)
    {
        _dbContext.Vehicles.Add(vehicle);
    }

    public void Remove(Vehicle vehicle)
    {
        _dbContext.Vehicles.Remove(vehicle);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.PlateNumber == plateNumber, cancellationToken);
    }

    public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vehicles.ToListAsync(cancellationToken);
    }
}
