using InventorySystem.Application.Products;
using InventorySystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<Guid> Create(CreateProduct.Command command, CancellationToken ct)
        => await _mediator.Send(command, ct);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProduct.Command body, CancellationToken ct)
    {
        var command = body with { Id = id };
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProduct.Command(id), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ProductDto> Get(Guid id, CancellationToken ct)
        => await _mediator.Send(new GetProductById.Query(id), ct);

    [HttpGet]
    public async Task<List<ProductDto>> GetAll(CancellationToken ct)
        => await _mediator.Send(new GetProducts.Query(), ct);

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateProducts(List<CreateProduct.Command> products, CancellationToken ct)
    {
        foreach (var product in products)
        {
            await _mediator.Send(product, ct);
        }

        return Ok();
    }
}