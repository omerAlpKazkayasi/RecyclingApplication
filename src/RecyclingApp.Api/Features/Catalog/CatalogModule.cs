using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using RecyclingApp.Api.Features.Catalog.Customers;
using RecyclingApp.Api.Features.Catalog.PriceLists;
using RecyclingApp.Api.Features.Catalog.Products;
using RecyclingApp.Api.Features.Catalog.Vehicles;

namespace RecyclingApp.Api.Features.Catalog;

public static class CatalogModule
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder builder)
    {
        CreateProduct.MapEndpoint(builder);
        GetProducts.MapEndpoint(builder);

        CreateCustomer.MapEndpoint(builder);
        GetCustomers.MapEndpoint(builder);

        CreateVehicle.MapEndpoint(builder);
        GetVehicles.MapEndpoint(builder);

        CreatePriceList.MapEndpoint(builder);
        AddPriceListItem.MapEndpoint(builder);
        GetActivePriceForProduct.MapEndpoint(builder);

        return builder;
    }
}
