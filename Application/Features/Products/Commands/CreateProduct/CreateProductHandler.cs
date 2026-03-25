using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Handler لـ CreateProductCommand
/// ينشئ المنتج في قاعدة البيانات ويُرجع Id الجديد
/// </summary>
public sealed class CreateProductHandler
    : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProductHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
