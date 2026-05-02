using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Queries;

public class ListProductsQueryHandler : IQueryHandler<ListProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> HandleAsync(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        
        return products.Select(p => new ProductDto(p.Id, p.Code, p.Name, p.Category, p.QualityGrade, p.UnitName, p.IsActive)).ToList();
    }
}
