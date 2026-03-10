using InventorySystem.Application.WarehouseInventories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseInventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<Guid> Create(CreateWarehouseInventory.Command command, CancellationToken ct)
        => await _mediator.Send(command, ct);

    [HttpPut("{id:guid}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid id, UpdateWarehouseInventoryQuantity.Command body, CancellationToken ct)
    {
        var command = body with { Id = id };
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<WarehouseInventoryDto> Get(Guid id, CancellationToken ct)
        => await _mediator.Send(new GetWarehouseInventoryById.Query(id), ct);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetWarehouseInventories.Query()));
    }

    [HttpGet("by-product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        return Ok(await _mediator.Send(new GetWarehouseInventoriesByProduct.Query(productId)));
    }

    [HttpGet("by-warehouse/{warehouseId}")]
    public async Task<IActionResult> GetByWarehouse(Guid warehouseId)
    {
        return Ok(await _mediator.Send(new GetWarehouseInventoriesByWarehouse.Query(warehouseId)));
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        return Ok(await _mediator.Send(new GetLowStockInventories.Query()));
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulk(List<CreateWarehouseInventory.Command> items, CancellationToken ct)
    {
        foreach (var item in items)
        {
            await _mediator.Send(item, ct);
        }

        return Ok();
    }
}