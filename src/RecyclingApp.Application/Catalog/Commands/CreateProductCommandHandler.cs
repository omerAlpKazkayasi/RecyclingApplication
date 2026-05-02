using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Application.Abstractions.Security;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Commands;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IRequestContext _requestContext;

    public CreateProductCommandHandler(IProductRepository productRepository, IRequestContext requestContext)
    {
        _productRepository = productRepository;
        _requestContext = requestContext;
    }

    public Task<ProductDto> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!_requestContext.HasTenant)
        {
            throw new InvalidOperationException("Tenant context is required.");
        }

        var product = Product.Create(
            _requestContext.TenantId,
            request.Code,
            request.Name,
            request.Category ?? "",
            request.QualityGrade ?? "",
            request.UnitName ?? "");

        _productRepository.Add(product);

        return Task.FromResult(new ProductDto(product.Id, product.Code, product.Name, product.Category, product.QualityGrade, product.UnitName, product.IsActive));
    }
}
