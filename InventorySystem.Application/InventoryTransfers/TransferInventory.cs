using FluentValidation;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.InventoryTransfers;

public static class TransferInventory
{
    public sealed record Command(
        Guid ProductId,
        Guid FromWarehouseId,
        Guid ToWarehouseId,
        int Quantity
    ) : IRequest<Unit>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.FromWarehouseId).NotEmpty();
            RuleFor(x => x.ToWarehouseId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Command, Unit>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var fromInventory = await _context.WarehouseInventories
                .FirstOrDefaultAsync(x =>
                    x.ProductId == request.ProductId &&
                    x.WarehouseId == request.FromWarehouseId,
                    cancellationToken);

            if (fromInventory == null)
                throw new Exception("Source inventory not found.");

            if (fromInventory.QuantityCurrent < request.Quantity)
                throw new Exception("Insufficient stock in source warehouse.");

            var toInventory = await _context.WarehouseInventories
                .FirstOrDefaultAsync(x =>
                    x.ProductId == request.ProductId &&
                    x.WarehouseId == request.ToWarehouseId,
                    cancellationToken);

            if (toInventory == null)
            {
                toInventory = new WarehouseInventory
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = request.ToWarehouseId,
                    ProductId = request.ProductId,
                    QuantityCurrent = 0,
                    QuantityMin = 1,
                    QuantityMax = 999999
                };

                _context.WarehouseInventories.Add(toInventory);
            }

            if (toInventory == null)
                throw new Exception("Destination inventory not found.");

            if (toInventory.QuantityCurrent + request.Quantity > toInventory.QuantityMax)
                throw new Exception("Destination warehouse capacity exceeded.");

            // Deduct from source
            fromInventory.QuantityCurrent -= request.Quantity;

            // Add to destination
            toInventory.QuantityCurrent += request.Quantity;

            // Log outbound transaction
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                WarehouseId = request.FromWarehouseId,
                Quantity = request.Quantity,
                Type = TransactionType.Outbound
            });

            // Log inbound transaction
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                WarehouseId = request.ToWarehouseId,
                Quantity = request.Quantity,
                Type = TransactionType.Inbound
            });

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}