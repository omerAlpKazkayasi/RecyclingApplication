using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Api.Features.Catalog.Customers;

public static class CreateCustomer
{
    public record Request(string Name, string TaxNumber, string IdentityNumber, string Phone, string Address, string Notes);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/customers", async (
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var customer = Customer.Create(
                context.TenantId,
                request.Name,
                request.TaxNumber ?? "",
                request.IdentityNumber ?? "",
                request.Phone ?? "",
                request.Address ?? "",
                request.Notes ?? "");

            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { customer.Id, customer.PublicId, customer.Name });
        })
        .WithName("CreateCustomer")
        .WithTags("Catalog");
    }
}
