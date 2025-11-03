using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KitchenController(IKitchenQueueService service) : Controller
{
    [HttpGet("get-queue")]
    public async Task<IActionResult> GetQueue()
    {
        var response = await service.GetQueueAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("mark-as-ready")]
    public async Task<IActionResult> MarkAsReady(int orderItemId)
    {
        var response = await service.MarkAsReadyAsync(orderItemId);
        return StatusCode(response.StatusCode, response);
    }
}