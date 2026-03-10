using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.WarehouseInventories;

public static class GetWarehouseInventoryById
{
    public sealed record Query(Guid Id) : IRequest<WarehouseInventoryDto>;

    public sealed class Handler : IRequestHandler<Query, WarehouseInventoryDto>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<WarehouseInventoryDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.WarehouseInventories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new Exception("WarehouseInventory not found.");

            return new WarehouseInventoryDto(
                entity.Id,
                entity.WarehouseId,
                entity.ProductId,
                entity.QuantityCurrent,
                entity.QuantityMin,
                entity.QuantityMax
            );
        }
    }
}