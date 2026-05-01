using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Modules.Catalog.Domain.Entities;

public class Product : TenantEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string QualityGrade { get; private set; } = string.Empty;
    public string UnitName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    protected Product() : base() { }

    public static Product Create(Guid tenantId, string code, string name, string category, string qualityGrade, string unitName)
    {
        var product = new Product
        {
            Code = code,
            Name = name,
            Category = category,
            QualityGrade = qualityGrade,
            UnitName = unitName,
            IsActive = true
        };
        product.SetTenant(tenantId);
        return product;
    }

    public void Update(string name, string category, string qualityGrade, string unitName)
    {
        Name = name;
        Category = category;
        QualityGrade = qualityGrade;
        UnitName = unitName;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
