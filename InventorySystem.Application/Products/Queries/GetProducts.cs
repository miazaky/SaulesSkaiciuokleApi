using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Products;

public static class GetProducts
{
    public sealed record Query : IRequest<List<ProductDto>>;

    public sealed class Handler : IRequestHandler<Query, List<ProductDto>>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Select(x => new ProductDto(
                    x.Id,
                    x.Sku,
                    x.Name,
                    x.Length,
                    x.Description
                ))
                .ToListAsync(cancellationToken);
        }
    }
}