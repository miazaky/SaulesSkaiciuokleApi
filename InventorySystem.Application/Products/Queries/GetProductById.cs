using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Products;

public static class GetProductById
{
    public sealed record Query(Guid Id) : IRequest<ProductDto>;

    public sealed class Handler : IRequestHandler<Query, ProductDto>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
                throw new Exception("Product not found.");

            return new ProductDto(
                product.Id,
                product.Sku,
                product.Name,
                product.Length,
                product.Description
            );
        }
    }
}