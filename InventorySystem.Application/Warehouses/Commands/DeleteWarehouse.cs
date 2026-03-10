using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Interfaces;

namespace InventorySystem.Application.Warehouses;

public static class DeleteWarehouse
{
    public sealed record Command(Guid Id) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IInventoryDbContext _db;
        public Handler(IInventoryDbContext db) => _db = db;

        public async Task Handle(Command request, CancellationToken ct)
        {
            var warehouse = await _db.Warehouses
                .FirstOrDefaultAsync(w => w.Id == request.Id && !w.Deleted, ct);

            if (warehouse is null)
                throw new KeyNotFoundException("Warehouse not found.");

            // This will become soft delete if you implemented SaveChanges override for Deleted
            _db.Warehouses.Remove(warehouse);
            await _db.SaveChangesAsync(ct);
        }
    }
}