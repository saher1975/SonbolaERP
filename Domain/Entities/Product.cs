using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// كيان المنتج - مثال على Entity يرث من BaseEntity
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
