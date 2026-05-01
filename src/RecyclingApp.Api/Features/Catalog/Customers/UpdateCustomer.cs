using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Api.Features.Catalog.Customers;

public static class UpdateCustomer
{
    public record Request(string Name, string TaxNumber, string IdentityNumber, string Phone, string Address, string Notes);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/customers/{publicId:guid}", async (
            Guid publicId,
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var customer = await dbContext.Customers
                .FirstOrDefaultAsync(c => c.PublicId == publicId, cancellationToken);

            if (customer == null)
                return Results.NotFound($"Customer '{publicId}' not found.");

            customer.Update(
                request.Name,
                request.TaxNumber ?? "",
                request.IdentityNumber ?? "",
                request.Phone ?? "",
                request.Address ?? "",
                request.Notes ?? "");

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { customer.PublicId, customer.Name, customer.IsActive });
        })
        .WithName("UpdateCustomer")
        .WithTags("Catalog");
    }
}
