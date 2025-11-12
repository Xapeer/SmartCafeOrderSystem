using System.Text.Json;
using Application.Common;
using Application.Dtos.Kitchen;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackRedis = StackExchange.Redis;

namespace Application.Services;

public class KitchenQueueService : IKitchenQueueService
{
    private readonly StackRedis.IDatabase _redis;
    private readonly IDataContext _context;
    private readonly ILogger<KitchenQueueService> _logger;

    public KitchenQueueService(StackRedis.IConnectionMultiplexer redis, IDataContext context, ILogger<KitchenQueueService> logger)
    {
        _redis = redis.GetDatabase();
        _context = context;
        _logger = logger;
    }

    public async Task<bool> EnqueueOrderItemAsync(int orderItemId)
    {
        var orderItem = await _context.OrderItems
            .Include(oi => oi.MenuItem)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

        if (orderItem == null)
        {
            _logger.LogWarning("OrderItem {OrderItemId} not found for queue", orderItemId);
            return false;
        }

        var dto = new GetOrderItemForKitchenDto
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            MenuItemId = orderItem.MenuItemId,
            MenuItemName = orderItem.MenuItem.Name,
            Quantity = orderItem.Quantity,
            Notes = orderItem.Notes,
            Status = orderItem.Status,
            StartedAt = orderItem.StartedAt
        };

        var json = JsonSerializer.Serialize(dto);
        await _redis.ListRightPushAsync("kitchen:queue", json);

        _logger.LogInformation("OrderItem {OrderItemId} added to kitchen queue", orderItemId);
        return true;
    }

    public async Task<Response<List<GetOrderItemForKitchenDto>>> GetQueueAsync()
    {
        var items = await _redis.ListRangeAsync("kitchen:queue");
        var result = new List<GetOrderItemForKitchenDto>();

        foreach (var item in items)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<GetOrderItemForKitchenDto>(item);
                if (dto != null)
                    result.Add(dto);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize item from kitchen queue");
            }
        }

        return new Response<List<GetOrderItemForKitchenDto>>(200, "Queue fetched successfully", result);
    }

    public async Task<Response<bool>> MarkAsReadyAsync(int orderItemId)
    {
        // Update OrderItem status in DB
        var orderItem = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

        if (orderItem == null)
        {
            _logger.LogWarning("OrderItem {OrderItemId} not found in DB", orderItemId);
            return new Response<bool>(400, "OrderItem not found", false);
        }

        orderItem.Status = OrderItemStatus.Ready;
        orderItem.CompletedAt = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update OrderItem {OrderItemId} status in DB", orderItemId);
            return new Response<bool>(500, "An error occured when updating OrderItem");
        }

        // Remove from the Redis queue by adding all items from queue except the 'Ready' one into a new array
        // Then push everything back into the queue
        var queueItems = await _redis.ListRangeAsync("kitchen:queue");
        var remainingItems = new List<StackRedis.RedisValue>();

        bool found = false;

        foreach (var item in queueItems)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<GetOrderItemForKitchenDto>(item);
                if (dto != null && dto.Id == orderItemId)
                {
                    found = true; // Found required one, not being added into the queue
                }
                else
                {
                    remainingItems.Add(item); // Everything else will be added
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize item from kitchen queue, keeping it");
                remainingItems.Add(item); // If something went wrong, item remains in the queue
            }
        }

        if (found)
        {
            await _redis.KeyDeleteAsync("kitchen:queue");
            if (remainingItems.Count > 0)
            {
                await _redis.ListRightPushAsync("kitchen:queue", remainingItems.ToArray());
            }
        }

        return new Response<bool>(200, "OrderItem has been marked as 'Ready' successfully");
    }
}