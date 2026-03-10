using FluentValidation;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.InventoryTransactions;

public static class CreateInboundTransaction
{
    public sealed record Command(
        Guid WarehouseId,
        Guid ProductId,
        int Quantity
    ) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Command, Guid>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var inventory = await _context.WarehouseInventories
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == request.WarehouseId &&
                    x.ProductId == request.ProductId,
                    cancellationToken);

            if (inventory == null)
                throw new Exception("Warehouse inventory not found.");

            inventory.QuantityCurrent += request.Quantity;

            var transaction = new InventoryTransaction
            {
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Type = TransactionType.Inbound
            };

            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            return transaction.Id;
        }
    }
}