using InventorySystem.Application.Common.DTOs;
using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Dashboard;

public static class GetInventorySummary
{
    public sealed record Query() : IRequest<InventorySummaryDto>;

    public sealed class Handler : IRequestHandler<Query, InventorySummaryDto>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<InventorySummaryDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var totalProducts = await _context.Products.CountAsync(cancellationToken);
            var totalWarehouses = await _context.Warehouses.CountAsync(cancellationToken);

            var totalInventoryRecords = await _context.WarehouseInventories
                .CountAsync(cancellationToken);

            var totalStockUnits = await _context.WarehouseInventories
                .SumAsync(x => x.QuantityCurrent, cancellationToken);

            var lowStockItems = await _context.WarehouseInventories
                .CountAsync(x => x.QuantityCurrent <= x.QuantityMin, cancellationToken);

            return new InventorySummaryDto(
                totalProducts,
                totalWarehouses,
                totalInventoryRecords,
                totalStockUnits,
                lowStockItems
            );
        }
    }
}