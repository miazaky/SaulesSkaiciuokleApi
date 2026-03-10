using FluentValidation;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.WarehouseInventories;

public static class CreateWarehouseInventory
{
    public sealed record Command(
        Guid WarehouseId,
        Guid ProductId,
        int QuantityCurrent,
        int QuantityMin,
        int QuantityMax
    ) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.QuantityMin).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuantityMax).GreaterThan(x => x.QuantityMin);
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
            var exists = await _context.WarehouseInventories
                .AnyAsync(x => x.WarehouseId == request.WarehouseId &&
                               x.ProductId == request.ProductId,
                               cancellationToken);

            if (exists)
                throw new Exception("Inventory for this warehouse/product already exists.");

            var entity = new WarehouseInventory
            {
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,
                QuantityCurrent = request.QuantityCurrent,
                QuantityMin = request.QuantityMin,
                QuantityMax = request.QuantityMax
            };

            _context.WarehouseInventories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}