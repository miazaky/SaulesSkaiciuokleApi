using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.InventoryTransactions;

public static class GetTransactionHistory
{
    public sealed record Query(Guid ProductId)
        : IRequest<List<InventoryTransactionDto>>;

    public sealed class Handler
        : IRequestHandler<Query, List<InventoryTransactionDto>>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryTransactionDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            return await _context.InventoryTransactions
                .AsNoTracking()
                .Where(x => x.ProductId == request.ProductId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new InventoryTransactionDto(
                    x.Id,
                    x.ProductId,
                    x.WarehouseId,
                    x.Quantity,
                    x.Type,
                    x.CreatedDate
                ))
                .ToListAsync(cancellationToken);
        }
    }
}