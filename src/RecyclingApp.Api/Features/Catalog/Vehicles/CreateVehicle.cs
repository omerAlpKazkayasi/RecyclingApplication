using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Entities;
using RecyclingApp.Modules.Catalog.Domain.Enums;

namespace RecyclingApp.Api.Features.Catalog.Vehicles;

public static class CreateVehicle
{
    public record Request(string PlateNumber, VehicleType VehicleType, string Notes);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/vehicles", async (
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            // Basic normalization check
            var normalizedPlate = request.PlateNumber?.Replace(" ", "").ToUpperInvariant();

            var exists = await dbContext.Vehicles.AnyAsync(
                v => v.TenantId == context.TenantId && v.PlateNumber == normalizedPlate,
                cancellationToken);

            if (exists)
                return Results.Conflict($"Vehicle with plate '{normalizedPlate}' already exists for this tenant.");

            var vehicle = Vehicle.Create(
                context.TenantId,
                request.PlateNumber ?? "",
                request.VehicleType,
                request.Notes ?? "");

            dbContext.Vehicles.Add(vehicle);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { vehicle.Id, vehicle.PublicId, vehicle.PlateNumber });
        })
        .WithName("CreateVehicle")
        .WithTags("Catalog");
    }
}
