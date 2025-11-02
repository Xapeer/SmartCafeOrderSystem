using Application.Common;
using Application.Dtos.Order;
using Application.Dtos.OrderItem;
using Application.Filters.Order;
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

    public async Task<PagedResponse<GetOrderDto>> GetAllOrdersAsync(AllOrderFilter filter, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Orders.AsQueryable();

        // Filter by date
        if (filter.FromTime != default && filter.ToTime != default)
            query = query.Where(o => o.CreatedAt >= filter.FromTime && o.CreatedAt <= filter.ToTime);

        // Filter by status
        if (filter.Status != default)
            query = query.Where(o => o.Status == filter.Status);

        // Filter by OrderId
        if (filter.OrderId > 0)
            query = query.Where(o => o.Id == filter.OrderId);

        // Filter by TableId
        if (filter.TableId > 0)
            query = query.Where(o => o.TableId == filter.TableId);

        // Filter by WaiterId
        if (filter.WaiterId > 0)
            query = query.Where(o => o.WaiterId == filter.WaiterId);

        var totalRecords = await query.CountAsync();

        try
        {
            var orders = await query
                .OrderBy(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new GetOrderDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    Status = o.Status,
                    TableId = o.TableId,
                    WaiterId = o.WaiterId
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            _logger.LogInformation("Orders fetched successfully");
            return new PagedResponse<GetOrderDto>(orders, pageNumber, pageSize, totalRecords)
            {
                Message = "Orders fetched successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders");
            return new PagedResponse<GetOrderDto>(new List<GetOrderDto>(), pageNumber, pageSize, 0)
            {
                Message = "An error occurred while fetching the orders"
            };
        }
    }
    public async Task<Response<GetOrderWithItemsDto>> GetSingleOrderAsync(SingleOrderFilter filter)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .AsQueryable();
        
        // Filter by OrderId
        if (filter.OrderId > 0)
            query = query.Where(o => o.Id == filter.OrderId);

        // Filter by TableId
        if (filter.TableId > 0)
            query = query
                .Where(o => o.TableId == filter.TableId && (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Created));

        // Filter by WaiterId
        if (filter.WaiterId > 0)
            query = query.Where(o => o.WaiterId == filter.WaiterId);
        
        try
        {
            var order = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new GetOrderWithItemsDto()
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    CompletedAt = o.CompletedAt,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    Status = o.Status,
                    TableId = o.TableId,
                    WaiterId = o.WaiterId,
                    OrderItems = o.OrderItems
                        .Where(oi => oi.Status != OrderItemStatus.Cancelled)
                        .Select(oi => new GetOrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        Quantity = oi.Quantity,
                        PriceAtOrderTime = oi.PriceAtOrderTime,
                        Status = oi.Status,
                        StartedAt = oi.StartedAt,
                        CompletedAt = oi.CompletedAt,
                        Notes = oi.Notes
                    }).ToList()
                }).FirstOrDefaultAsync();

            _logger.LogInformation("Order fetched successfully");
            return new Response<GetOrderWithItemsDto>(200, "Order fetched successfully", order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order");
            return new Response<GetOrderWithItemsDto>(500, "An error occurred while fetching the order");
        }
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
        if (!tableExists.IsActive)
            return new Response<GetOrderDto>(400, "Table is not active");
        
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
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(t => t.Id == orderId);
        var menuItemExists = await _context.MenuItems
            .FirstOrDefaultAsync(t => t.Id == menuItemId);
        
        if (orderExists == null)
            return new Response<GetOrderItemDto>(400, "Invalid Order ID");
        if (menuItemExists == null)
            return new Response<GetOrderItemDto>(400, "Invalid MenuItem ID");
        
        // Check if MenuItem is Active
        if (!menuItemExists.IsActive)
            return new Response<GetOrderItemDto>(400, "MenuItem is not active");
            
        // Check if MenuItem is already among OrderItems in the Order and "New"
        // If so, just increment existing one
        var orderItemExists = orderExists.OrderItems.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (orderItemExists != null)
        {
            if (orderItemExists.Status == OrderItemStatus.New)
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
        }

        // If not, create a new one, join to order and add to DB
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
            
            _logger.LogInformation("Created OrderItem for order {OrderId}", orderId);
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
            .Include(o => o.OrderItems)
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
        
        // Check if OrderItem has status = "New"
        if (orderItemExists.Status != OrderItemStatus.New)
            return new Response<bool>(400, "OrderItem is not New");

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
        
        // Check if Order status = "Created" or "Confirmed"
        if (orderExists.Status != OrderStatus.Created && orderExists.Status != OrderStatus.Confirmed)
            return new Response<bool>(400, "Order confirmation is only allowed for orders with status 'Created' or 'Confirmed'");

        // Load OrderItems of Order
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderExists.Id).ToListAsync();
        
        // Change Order status to "Confirmed"
        orderExists.Status = OrderStatus.Confirmed;
        
        // For each OrderItem in Order change status to "Started" and send to KitchenQueue
        foreach (OrderItem orderItem in orderItems)
        {
            if (orderItem.Status == OrderItemStatus.New)
            {
                orderItem.Status = OrderItemStatus.Started;
                orderItem.StartedAt = DateTime.UtcNow;
            }
            
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
    public async Task<Response<bool>> PayForOrderAsync(int orderId)
    {
        // Check if Order exists
        var orderExists = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (orderExists == null)
            return new Response<bool>(400, "Invalid Order ID");
        
        // Check if Order status = "Confirmed"
        if (orderExists.Status != OrderStatus.Confirmed)
            return new Response<bool>(400, "Confirm the Order first");

        var orderTable = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == orderExists.TableId);
        
        //...
        //Some payment methods calls
        //...

        orderTable.IsFree = true;
        orderExists.Status = OrderStatus.Paid;
        orderExists.CompletedAt = DateTime.UtcNow;
        
        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Order {OrderId} paid", orderId);
            return new Response<bool>(200, "Order paid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error paying Order");
            return new Response<bool>(500, "An error occurred while paying Order");
        }
    }
    public async Task<Response<bool>> CancelOrderAsync(int orderId)
    {
        // Check if Order exists
        var orderExists = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(t => t.Id == orderId);
        
        if (orderExists == null)
            return new Response<bool>(400, "Invalid Order ID");
        
        // Check if Order status = "Created"
        if (orderExists.Status != OrderStatus.Created)
            return new Response<bool>(400, "Order cancellation is only allowed for orders with status 'Created'");
        
        var orderTable = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == orderExists.TableId);
        
        orderExists.Status = OrderStatus.Cancelled;
        orderTable.IsFree = true;
        
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
    public async Task<Response<decimal>> GetOrderTotalAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return new Response<decimal>(404, "Order not found");

        var total = order.OrderItems
            .Where(oi => oi.Status != OrderItemStatus.Cancelled)
            .Sum(oi => oi.PriceAtOrderTime * oi.Quantity);

        return new Response<decimal>(200, "Total calculated successfully", total);
    }

}