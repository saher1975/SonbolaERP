using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command: طلب حذف منتج
/// </summary>
public record DeleteProductCommand(int Id) : IRequest;
