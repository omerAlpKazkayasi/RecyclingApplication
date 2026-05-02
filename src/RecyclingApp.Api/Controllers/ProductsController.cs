using Microsoft.AspNetCore.Mvc;
using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.Commands;
using RecyclingApp.Application.Catalog.Queries;

namespace RecyclingApp.Api.Controllers;

[ApiController]
[Route("api/catalog/products")]
public class ProductsController : ControllerBase
{
    private readonly IApplicationDispatcher _dispatcher;

    public ProductsController(IApplicationDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new ListProductsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteProductCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
