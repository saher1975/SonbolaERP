using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command: طلب تحديث منتج موجود
/// </summary>
public record UpdateProductCommand(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock
) : IRequest;
