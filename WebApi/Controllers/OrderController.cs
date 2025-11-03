using Application.Dtos.Order;
using Application.Filters.Order;
using Application.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

//[Authorize(Roles = $"{Roles.Waiter}, {Roles.Admin}")]
[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService service) : Controller
{
    [HttpGet("get-all-orders")]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] AllOrderFilter filter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetAllOrdersAsync(filter, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-single-order")]
    public async Task<IActionResult> GetSingleOrder([FromQuery] SingleOrderFilter filter)
    {
        var response = await service.GetSingleOrderAsync(filter);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-order-total")]
    public async Task<IActionResult> GetOrderTotal(int orderId)
    {
        var response = await service.GetOrderTotalAsync(orderId);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var response = await service.CreateOrderAsync(dto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("add-order-item")]
    public async Task<IActionResult> AddOrderItem(int orderId, int menuItemId)
    {
        var response = await service.AddOrderItemAsync(orderId, menuItemId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("remove-order-item")]
    public async Task<IActionResult> RemoveOrderItem(int orderId, int orderItemId)
    {
        var response = await service.RemoveOrderItemAsync(orderId, orderItemId);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut("serve-order-item")]
    public async Task<IActionResult> ServeOrderItem(int orderId)
    {
        var response = await service.ServeOrderItemAsync(orderId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("confirm-order")]
    public async Task<IActionResult> ConfirmOrder(int orderId)
    {
        var response = await service.ConfirmOrderAsync(orderId);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut("pay-for-order")]
    public async Task<IActionResult> PayForOrder(int orderId)
    {
        var response = await service.PayForOrderAsync(orderId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("cancel-order")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var response = await service.CancelOrderAsync(orderId);
        return StatusCode(response.StatusCode, response);
    }
}