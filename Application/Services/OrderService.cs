using Application.Common;
using Application.Dtos.Order;
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
        // Check if table ID correct
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
            await _context.SaveChangesAsync();
            tableExists.IsFree = false;
            
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

}