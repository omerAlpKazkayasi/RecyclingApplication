using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> HandleAsync(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            return false;
        }

        _productRepository.Remove(product);
        return true;
    }
}
