using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Enums;

namespace RecyclingApp.Api.Features.Catalog.Vehicles;

public static class UpdateVehicle
{
    public record Request(string PlateNumber, VehicleType VehicleType, string Notes);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/vehicles/{publicId:guid}", async (
            Guid publicId,
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var vehicle = await dbContext.Vehicles
                .FirstOrDefaultAsync(v => v.PublicId == publicId, cancellationToken);

            if (vehicle == null)
                return Results.NotFound($"Vehicle '{publicId}' not found.");

            // Check plate uniqueness if plate number changed
            var normalizedPlate = request.PlateNumber.Replace(" ", "").ToUpperInvariant();
            var plateConflict = await dbContext.Vehicles
                .AnyAsync(v => v.TenantId == context.TenantId
                            && v.PlateNumber == normalizedPlate
                            && v.PublicId != publicId,
                    cancellationToken);

            if (plateConflict)
                return Results.Conflict($"A vehicle with plate '{request.PlateNumber}' already exists for this tenant.");

            vehicle.Update(request.PlateNumber, request.VehicleType, request.Notes ?? "");
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { vehicle.PublicId, vehicle.PlateNumber, vehicle.VehicleType, vehicle.IsActive });
        })
        .WithName("UpdateVehicle")
        .WithTags("Catalog");
    }
}
