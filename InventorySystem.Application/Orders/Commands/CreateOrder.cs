using FluentValidation;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Orders;

public static class CreateOrder
{
    public sealed record Command(Guid WarehouseId) : IRequest<Guid>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
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
            var order = new Order
            {
                WarehouseId = request.WarehouseId,
                Status = "PENDING"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}