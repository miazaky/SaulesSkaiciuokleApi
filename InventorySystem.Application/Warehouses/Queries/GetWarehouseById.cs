using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application;
using InventorySystem.Application.Interfaces;

namespace InventorySystem.Application.Warehouses;

public static class GetWarehouseById
{
    public sealed record Query(Guid Id) : IRequest<WarehouseDto>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Query, WarehouseDto>
    {
        private readonly IInventoryDbContext _db;
        public Handler(IInventoryDbContext db) => _db = db;

        public async Task<WarehouseDto> Handle(Query request, CancellationToken ct)
        {
            var w = await _db.Warehouses
                .AsNoTracking()
                .Where(x => x.Id == request.Id && !x.Deleted)
                .Select(x => new WarehouseDto(x.Id, x.Name, x.Location))
                .FirstOrDefaultAsync(ct);

            if (w is null)
                throw new KeyNotFoundException("Warehouse not found.");

            return w;
        }
    }
}