using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command: طلب إنشاء منتج جديد
/// يُرجع Id المنتج الجديد بعد الحفظ
/// </summary>
public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int Stock
) : IRequest<int>;
