using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application;
using InventorySystem.Application.Interfaces;

namespace InventorySystem.Application.Warehouses;

public static class GetWarehouses
{
    // optional search/paging
    public sealed record Query(string? Search, int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<WarehouseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
            RuleFor(x => x.Search).MaximumLength(200);
        }
    }

    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<WarehouseDto>>
    {
        private readonly IInventoryDbContext _db;
        public Handler(IInventoryDbContext db) => _db = db;

        public async Task<IReadOnlyList<WarehouseDto>> Handle(Query request, CancellationToken ct)
        {
            var q = _db.Warehouses.AsNoTracking().Where(x => !x.Deleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim();
                q = q.Where(x => x.Name.Contains(s) || (x.Location != null && x.Location.Contains(s)));
            }

            var skip = (request.Page - 1) * request.PageSize;

            return await q
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(request.PageSize)
                .Select(x => new WarehouseDto(x.Id, x.Name, x.Location))
                .ToListAsync(ct);
        }
    }
}