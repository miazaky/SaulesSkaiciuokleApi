using InventorySystem.Application.InventoryTransfers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferInventory([FromBody] TransferInventory.Command command)
    {
        await _mediator.Send(command);
        return Ok("Inventory transferred successfully.");
    }
}