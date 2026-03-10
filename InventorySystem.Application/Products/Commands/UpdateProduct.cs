using FluentValidation;
using InventorySystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.Application.Products;

public static class UpdateProduct
{
    public sealed record Command(
        Guid Id,
        string? Sku,
        string Name,
        int? Length,
        string? Description
    ) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Sku).NotEmpty();
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
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
                throw new Exception("Product not found.");

            var skuExists = await _context.Products
                .AnyAsync(x => x.Sku == request.Sku && x.Id != request.Id, cancellationToken);

            if (skuExists)
                throw new Exception("SKU already used by another product.");

            product.Name = request.Name;
            product.Sku = request.Sku;
            product.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}