using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaiterServiceController(IWaiterService service) : Controller
{
    [HttpGet("get-number-of-orders")]
    public async Task<IActionResult> GetNumberOfOrders()
    {
        var response = await service.GetNumberOfOrdersAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-orders-total")]
    public async Task<IActionResult> GetOrdersTotal()
    {
        var response = await service.GetOrdersTotalAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-avg-order-time")]
    public async Task<IActionResult> GetAvgOrderTimeSpan()
    {
        var response = await service.GetAvgOrderTimeSpanAsync();
        return StatusCode(response.StatusCode, response);
    }
}