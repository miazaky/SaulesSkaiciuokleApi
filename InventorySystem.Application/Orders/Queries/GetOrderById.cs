using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Orders;

public static class GetOrderById
{
    public sealed record Query(Guid Id) : IRequest<OrderDto>;

    public sealed class Handler : IRequestHandler<Query, OrderDto>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(x => x.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order == null)
                throw new Exception("Order not found.");

            return new OrderDto(
                order.Id,
                order.WarehouseId,
                order.Status,
                order.Items.Select(i =>
                    new OrderItemDto(i.ProductId, i.Quantity)).ToList()
            );
        }
    }
}