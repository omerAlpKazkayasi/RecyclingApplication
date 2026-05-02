using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID '{request.Id}' not found.");
        }

        return new ProductDto(product.Id, product.Code, product.Name, product.Category, product.QualityGrade, product.UnitName, product.IsActive);
    }
}
