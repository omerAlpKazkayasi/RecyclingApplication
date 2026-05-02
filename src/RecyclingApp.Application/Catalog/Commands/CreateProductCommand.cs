using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;

namespace RecyclingApp.Application.Catalog.Commands;

public record CreateProductCommand(string Code, string Name, string Category, string QualityGrade, string UnitName) : ICommand<ProductDto>;
