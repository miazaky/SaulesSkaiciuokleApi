using InventorySystem.Application.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("inventory-summary")]
    public async Task<IActionResult> GetInventorySummary()
    {
        var result = await _mediator.Send(new GetInventorySummary.Query());
        return Ok(result);
    }
}