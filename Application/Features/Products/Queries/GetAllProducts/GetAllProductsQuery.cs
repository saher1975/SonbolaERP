using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// Query: طلب جلب كل المنتجات
/// يُرجع قائمة من ProductDto
/// </summary>
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
