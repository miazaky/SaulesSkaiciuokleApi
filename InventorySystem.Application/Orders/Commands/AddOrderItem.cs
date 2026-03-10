using FluentValidation;
using InventorySystem.Application.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Orders;

public static class AddOrderItem
{
    public sealed record Command(
        Guid OrderId,
        Guid ProductId,
        int Quantity
    ) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
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
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != "PENDING")
                throw new Exception("Cannot modify completed order.");

            var item = new OrderItem
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}