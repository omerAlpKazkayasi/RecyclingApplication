using Microsoft.AspNetCore.Mvc;
using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.Commands;
using RecyclingApp.Application.Catalog.Queries;

namespace RecyclingApp.Api.Controllers;

[ApiController]
[Route("api/catalog/customers")]
public class CustomersController : ControllerBase
{
    private readonly IApplicationDispatcher _dispatcher;

    public CustomersController(IApplicationDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new ListCustomersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new GetCustomerByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteCustomerCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
