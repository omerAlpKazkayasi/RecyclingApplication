using RecyclingApp.Modules.Catalog.Domain.Enums;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Catalog.Domain.Entities;

public class Vehicle : TenantEntity
{
    public string PlateNumber { get; private set; } = string.Empty;
    public VehicleType VehicleType { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    protected Vehicle() : base() { }

    public static Vehicle Create(Guid tenantId, string plateNumber, VehicleType vehicleType, string notes)
    {
        var vehicle = new Vehicle
        {
            PlateNumber = NormalizePlateNumber(plateNumber),
            VehicleType = vehicleType,
            Notes = notes,
            IsActive = true
        };
        vehicle.SetTenant(tenantId);
        return vehicle;
    }

    public void Update(string plateNumber, VehicleType vehicleType, string notes)
    {
        PlateNumber = NormalizePlateNumber(plateNumber);
        VehicleType = vehicleType;
        Notes = notes;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static string NormalizePlateNumber(string plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            return string.Empty;
        
        // Remove spaces and uppercase
        return plateNumber.Replace(" ", "").ToUpperInvariant();
    }
}
