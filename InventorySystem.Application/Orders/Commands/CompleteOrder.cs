using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Orders;

public static class CompleteOrder
{
    public sealed record Command(Guid OrderId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != "PENDING")
                throw new Exception("Order already processed.");

            foreach (var item in order.Items)
            {
                var inventory = await _context.WarehouseInventories
                    .FirstOrDefaultAsync(x =>
                        x.WarehouseId == order.WarehouseId &&
                        x.ProductId == item.ProductId,
                        cancellationToken);

                if (inventory == null)
                    throw new Exception("Inventory not found.");

                if (inventory.QuantityCurrent < item.Quantity)
                    throw new Exception("Insufficient stock.");

                inventory.QuantityCurrent -= item.Quantity;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    WarehouseId = order.WarehouseId,
                    ProductId = item.ProductId,
                    Quantity = -item.Quantity,
                    Type = TransactionType.Outbound
                });
            }

            order.Status = "COMPLETED";

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}