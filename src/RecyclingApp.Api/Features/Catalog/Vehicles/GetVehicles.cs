using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Api.Features.Catalog.Vehicles;

public static class GetVehicles
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/vehicles", async (
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var vehicles = await dbContext.Vehicles
                .AsNoTracking()
                .Select(v => new
                {
                    v.PublicId,
                    v.PlateNumber,
                    v.VehicleType,
                    v.Notes,
                    v.IsActive
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(vehicles);
        })
        .WithName("GetVehicles")
        .WithTags("Catalog");
    }
}
