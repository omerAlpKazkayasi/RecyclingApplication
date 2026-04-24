using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Api.Features.Catalog.Customers;

public static class GetCustomers
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/customers", async (
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var customers = await dbContext.Customers
                .AsNoTracking()
                .Select(c => new
                {
                    c.PublicId,
                    c.Name,
                    c.TaxNumber,
                    c.IdentityNumber,
                    c.Phone,
                    c.Address,
                    c.IsActive
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(customers);
        })
        .WithName("GetCustomers")
        .WithTags("Catalog");
    }
}
