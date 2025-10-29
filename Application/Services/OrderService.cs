using Application.Common;
using Application.Dtos.Order;
using Application.Dtos.OrderItem;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService : IOrderService
{
    private IDataContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IDataContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto dto)
    {
        // Check if table ID correct and table is free
        var tableExists = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == dto.TableId);

        if (tableExists == null)
            return new Response<GetOrderDto>(400, "Invalid table ID");
        if (!tableExists.IsFree)
            return new Response<GetOrderDto>(400, "Table is not free");
        
        var order = new Order
        {
            TableId = dto.TableId,
            WaiterId = dto.WaiterId,
        };

        try
        {
            _context.Orders.Add(order);
            tableExists.IsFree = false;
            await _context.SaveChangesAsync();
            
            var result = new GetOrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                WaiterId = order.WaiterId,
                TableId = order.TableId,
                Status = order.Status
            };
            _logger.LogInformation("Created new order {OrderId} for table {TableId}", order.Id, dto.TableId);
            return new Response<GetOrderDto>(200, "Order created successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return new Response<GetOrderDto>(500, "An error occurred while creating the order");
        }
    }

    public async Task<Response<GetOrderItemDto>> AddOrderItemAsync(int orderId, int menuItemId)
    {
        // Check if Order and MenuItem exist
        var orderExists = await _context.Orders
            .FirstOrDefaultAsync(t => t.Id == orderId);
        var menuItemExists = await _context.MenuItems
            .FirstOrDefaultAsync(t => t.Id == menuItemId);
        
        if (orderExists == null)
            return new Response<GetOrderItemDto>(400, "Invalid Order ID");
        if (menuItemExists == null)
            return new Response<GetOrderItemDto>(400, "Invalid MenuItem ID");
        
        // Check if MenuItem is already among OrderItems in the Order
        var orderItemExists = orderExists.OrderItems.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (orderItemExists != null)
        {
            orderItemExists.Quantity++;
            await _context.SaveChangesAsync();

            var result = new GetOrderItemDto()
            {
                Id = orderItemExists.Id,
                OrderId = orderItemExists.OrderId,
                MenuItemId = orderItemExists.MenuItemId,
                Quantity = orderItemExists.Quantity,
                PriceAtOrderTime = orderItemExists.PriceAtOrderTime,
                Status = orderItemExists.Status
            };
            
            _logger.LogInformation("Incremented existing OrderItem {OrderItemId} for order {OrderId}", orderItemExists.Id, orderId);
            return new Response<GetOrderItemDto>(200, "Incremented existing OrderItem", result);
        }

        // If not, create a new one, add to order and DB
        var newOrderItem = new OrderItem()
        {
            OrderId = orderId,
            MenuItemId = menuItemId,
            PriceAtOrderTime = menuItemExists.Price
        };

        try
        {
            orderExists.OrderItems.Add(newOrderItem);
            _context.OrderItems.Add(newOrderItem);
            await _context.SaveChangesAsync();
            
            var result = new GetOrderItemDto()
            {
                Id = newOrderItem.Id,
                OrderId = newOrderItem.OrderId,
                MenuItemId = newOrderItem.MenuItemId,
                Quantity = newOrderItem.Quantity,
                PriceAtOrderTime = newOrderItem.PriceAtOrderTime,
                Status = newOrderItem.Status
            };
            
            _logger.LogInformation("Created OrderItem {OrderItemId} for order {OrderId}", orderItemExists.Id, orderId);
            return new Response<GetOrderItemDto>(200, "Created OrderItem", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating OrderItem");
            return new Response<GetOrderItemDto>(500, "An error occurred while adding OrderItem");
        }
    }

    public async Task<Response<bool>> RemoveOrderItemAsync(int orderId, int orderItemId)
    {
        // Check if Order and OrderItem exist
        var orderExists = await _context.Orders
            .FirstOrDefaultAsync(t => t.Id == orderId);
        var orderItemExists = await _context.OrderItems
            .FirstOrDefaultAsync(t => t.Id == orderItemId);
        
        if (orderExists == null)
            return new Response<bool>(400, "Invalid Order ID");
        if (orderItemExists == null)
            return new Response<bool>(400, "Invalid OrderItem ID");
        
        // Check if OrderItem is present in Order
        if (orderExists.OrderItems.All(item => item.Id != orderItemId))
            return new Response<bool>(400, "OrderItem is missing in Order");

        try
        {
            orderExists.OrderItems.Remove(orderItemExists);
            _context.OrderItems.Remove(orderItemExists);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Removed OrderItem {OrderItemId} for order {OrderId}", orderItemId, orderId);
            return new Response<bool>(200, "OrderItem removed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error removing OrderItem");
            return new Response<bool>(500, "An error occurred while removing OrderItem");
        }
    }

    public async Task<Response<bool>> ConfirmOrderAsync(int orderId)
    {
        // Check if Order exists
        var orderExists = await _context.Orders
            .FirstOrDefaultAsync(t => t.Id == orderId);
        
        if (orderExists == null)
            return new Response<bool>(400, "Invalid Order ID");

        
        orderExists.Status = OrderStatus.Confirmed;
        
        // For each OrderItem in Order change status to "Started"
        foreach (OrderItem orderItem in orderExists.OrderItems)
        {
            orderItem.Status = OrderItemStatus.Started;
            
            //TODO: send each OrderItem to KitchenQueue
        }

        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Confirmed Order {OrderId}", orderId);
            return new Response<bool>(200, "Order confirmed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error confirming Order");
            return new Response<bool>(500, "An error occurred while confirming Order");
        }
    }

    public async Task<Response<bool>> CancelOrderAsync(int orderId)
    {
        // Check if Order exists
        var orderExists = await _context.Orders
            .FirstOrDefaultAsync(t => t.Id == orderId);
        
        if (orderExists == null)
            return new Response<bool>(400, "Invalid Order ID");

        
        orderExists.Status = OrderStatus.Cancelled;
        
        // For each OrderItem in Order change status to "Cancelled"
        foreach (OrderItem orderItem in orderExists.OrderItems)
        {
            orderItem.Status = OrderItemStatus.Cancelled;
        }

        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Cancelled Order {OrderId}", orderId);
            return new Response<bool>(200, "Order cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error cancelling Order");
            return new Response<bool>(500, "An error occurred while cancelling Order");
        }
    }

}