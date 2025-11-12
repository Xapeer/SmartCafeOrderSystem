using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportService service) : Controller
{
    [HttpGet("get-number-of-orders")]
    public async Task<IActionResult> GetNumberOfOrders()
    {
        var response = await service.GetOrderCountAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-today-total-revenue")]
    public async Task<IActionResult> GetTodayTotalRevenue()
    {
        var response = await service.GetTodayRevenueAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-avg-check-total")]
    public async Task<IActionResult> GetAvgCheckTotal()
    {
        var response = await service.GetAverageCheckAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-avg-order-duration")]
    public async Task<IActionResult> GetAvgOrderOrderDuration()
    {
        var response = await service.GetAverageOrderDurationAsync();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-popular-menu-items")]
    public async Task<IActionResult> GetPopularMenuItems(int pageNumber = 1, int pageSize = 10)
    {
        var response = await service.GetPopularMenuItemsAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-waiters-rating")]
    public async Task<IActionResult> GetWaitersRating(int pageNumber = 1, int pageSize = 10)
    {
        var response = await service.GetWaiterRatingAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
}