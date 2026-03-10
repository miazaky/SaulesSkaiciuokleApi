using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.Warehouses;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
	private readonly IMediator _mediator;

	public WarehousesController(IMediator mediator) => _mediator = mediator;

	[HttpPost]
	public async Task<ActionResult<Guid>> Create([FromBody] CreateWarehouse.Command cmd, CancellationToken ct)
		=> Ok(await _mediator.Send(cmd, ct));

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
		=> Ok(await _mediator.Send(new GetWarehouses.Query(search, page, pageSize), ct));

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<WarehouseDto>> GetById(Guid id, CancellationToken ct)
		=> Ok(await _mediator.Send(new GetWarehouseById.Query(id), ct));

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouse.Command body, CancellationToken ct)
	{
		// Ensure route id wins
		var cmd = body with { Id = id };
		await _mediator.Send(cmd, ct);
		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
	{
		await _mediator.Send(new DeleteWarehouse.Command(id), ct);
		return NoContent();
	}
}