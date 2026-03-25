using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query: طلب جلب منتج محدد بالـ Id
/// </summary>
public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
