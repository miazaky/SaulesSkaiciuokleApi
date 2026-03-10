using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Interfaces;
namespace InventorySystem.Application.Warehouses;

public static class UpdateWarehouse
{
    public sealed record Command(Guid Id, string Name, string? Location) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Location)
                .MaximumLength(300);
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

            warehouse.Name = request.Name.Trim();
            warehouse.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();

            await _db.SaveChangesAsync(ct);
        }
    }
}