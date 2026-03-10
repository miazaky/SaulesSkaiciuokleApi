using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.WarehouseInventories;

public static class GetWarehouseInventories
{
    public sealed record Query() : IRequest<List<WarehouseInventoryDto>>;

    public sealed class Handler : IRequestHandler<Query, List<WarehouseInventoryDto>>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<WarehouseInventoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.WarehouseInventories
                .AsNoTracking()
                .Select(x => new WarehouseInventoryDto(
                    x.Id,
                    x.WarehouseId,
                    x.ProductId,
                    x.QuantityCurrent,
                    x.QuantityMin,
                    x.QuantityMax
                ))
                .ToListAsync(cancellationToken);
        }
    }
}