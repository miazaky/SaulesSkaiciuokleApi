using FluentValidation;
using MediatR;
using InventorySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Interfaces;

namespace InventorySystem.Application.Warehouses;

public static class CreateWarehouse
{
    public sealed record Command(string Name, string? Location) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Location)
                .MaximumLength(300);
        }
    }

    public sealed class Handler : IRequestHandler<Command, Guid>
    {
        private readonly IInventoryDbContext _db;
        public Handler(IInventoryDbContext db) => _db = db;

        public async Task<Guid> Handle(Command request, CancellationToken ct)
        {
            //Prevent duplicates by Name (company choice)
            var exists = await _db.Warehouses
                .AnyAsync(w => !w.Deleted && w.Name == request.Name, ct);

            if (exists)
                throw new InvalidOperationException("Warehouse with the same name already exists.");

            var warehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim()
            };

            _db.Warehouses.Add(warehouse);
            await _db.SaveChangesAsync(ct);

            return warehouse.Id;
        }
    }
}