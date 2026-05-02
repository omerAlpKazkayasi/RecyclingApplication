using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;

namespace RecyclingApp.Application.Catalog.Queries;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;
