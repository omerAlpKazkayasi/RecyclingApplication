using Microsoft.Extensions.DependencyInjection;
using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Behaviors;
using RecyclingApp.Application.Catalog.Commands;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Queries;

namespace RecyclingApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationDispatcher, ApplicationDispatcher>();

        // Register Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkSaveBehavior<,>));

        // Register Handlers manually (could use reflection/Scrutor later)
        services.AddTransient<IRequestHandler<CreateCustomerCommand, CustomerDto>, CreateCustomerCommandHandler>();
        services.AddTransient<IRequestHandler<DeleteCustomerCommand, bool>, DeleteCustomerCommandHandler>();
        services.AddTransient<IRequestHandler<ListCustomersQuery, List<CustomerDto>>, ListCustomersQueryHandler>();
        services.AddTransient<IRequestHandler<GetCustomerByIdQuery, CustomerDto>, GetCustomerByIdQueryHandler>();
        
        services.AddTransient<IRequestHandler<CreateProductCommand, ProductDto>, CreateProductCommandHandler>();
        services.AddTransient<IRequestHandler<DeleteProductCommand, bool>, DeleteProductCommandHandler>();
        services.AddTransient<IRequestHandler<ListProductsQuery, List<ProductDto>>, ListProductsQueryHandler>();
        services.AddTransient<IRequestHandler<GetProductByIdQuery, ProductDto>, GetProductByIdQueryHandler>();

        return services;
    }
}
