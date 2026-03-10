using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Products;

public static class DeleteProduct
{
    public sealed record Command(Guid Id) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
                throw new Exception("Product not found.");

            var hasInventory = await _context.WarehouseInventories
                .AnyAsync(x => x.ProductId == request.Id && x.QuantityCurrent > 0, cancellationToken);

            if (hasInventory)
                throw new Exception("Cannot delete product with existing inventory.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}