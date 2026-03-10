using InventorySystem.Application.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<Guid> Create(CreateOrder.Command command, CancellationToken ct)
        => await _mediator.Send(command, ct);

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, AddOrderItem.Command body, CancellationToken ct)
    {
        var command = body with { OrderId = id };
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CompleteOrder.Command(id), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<OrderDto> Get(Guid id, CancellationToken ct)
        => await _mediator.Send(new GetOrderById.Query(id), ct);
}