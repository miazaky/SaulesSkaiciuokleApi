using FluentValidation;
using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.WarehouseInventories;

public static class UpdateWarehouseInventoryQuantity
{
    public sealed record Command(
        Guid Id,
        int NewQuantity
    ) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NewQuantity).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IInventoryDbContext _context;

        public Handler(IInventoryDbContext context)
        {
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _context.WarehouseInventories
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new Exception("WarehouseInventory not found.");

            entity.QuantityCurrent = request.NewQuantity;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}