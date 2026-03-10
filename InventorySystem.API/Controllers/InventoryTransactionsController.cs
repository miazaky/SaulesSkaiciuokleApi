using InventorySystem.Application.InventoryTransactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryTransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryTransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("inbound")]
    public async Task<Guid> Inbound(CreateInboundTransaction.Command command, CancellationToken ct)
        => await _mediator.Send(command, ct);

    [HttpPost("outbound")]
    public async Task<Guid> Outbound(CreateOutboundTransaction.Command command, CancellationToken ct)
        => await _mediator.Send(command, ct);

    [HttpGet("product/{productId:guid}")]
    public async Task<List<InventoryTransactionDto>> GetByProduct(Guid productId, CancellationToken ct)
        => await _mediator.Send(new GetTransactionsByProduct.Query(productId), ct);

    [HttpGet("history/{productId:guid}")]
    public async Task<ActionResult<List<InventoryTransactionDto>>> GetHistory(Guid productId)
    {
        var result = await _mediator.Send(
            new GetTransactionHistory.Query(productId));

        return Ok(result);
    }
}