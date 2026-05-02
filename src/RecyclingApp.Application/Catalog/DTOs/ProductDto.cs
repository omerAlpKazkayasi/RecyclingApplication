namespace RecyclingApp.Application.Catalog.DTOs;

public record ProductDto(Guid Id, string Code, string Name, string Category, string QualityGrade, string UnitName, bool IsActive);
