using System.Security.Claims;
using Application.Common;
using Application.Filters.Order;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class WaiterService : IWaiterService
{
    private readonly IDataContext _context;
    private readonly ILogger<WaiterService> _logger;
    private readonly IHttpContextAccessor  _httpContextAccessor;
    
    public WaiterService(IDataContext context, ILogger <WaiterService> logger, IHttpContextAccessor  httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<int>> GetNumberOfOrdersAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var employee = _context.Employees.FirstOrDefault(e => e.IdentityUserId == userId);
        if (employee == null)
        {
            return new Response<int>(400, "Failed to get number of orders. Either no EmployeeId ot not logged in");
        }
        
        try
        {
            var numberOfOrders = await _context.Orders
                .CountAsync(o =>
                    o.CreatedAt.Date == DateTime.Now.Date && o.WaiterId == employee.Id &&
                    o.Status == OrderStatus.Paid);

            _logger.LogInformation("Number of orders fetched successfully");
            return new Response<int>(200, $"Number of orders fetched successfully", numberOfOrders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the number of orders");
            return new Response<int>(500, "An error occurred while fetching the number of orders");
        }
    }

    public async Task<Response<decimal>> GetOrdersTotalAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.IdentityUserId == userId);

        if (employee == null)
        {
            return new Response<decimal>(400, "Failed to get orders total. Either no EmployeeId or not logged in");
        }

        try
        {
            var ordersTotal = await _context.Orders
                .Where(o =>
                    o.CreatedAt.Date == DateTime.Now.Date && o.WaiterId == employee.Id &&
                    o.Status == OrderStatus.Paid)
                .SumAsync(o => o.TotalAmount);

            _logger.LogInformation("Orders total fetched successfully");
            return new Response<decimal>(200, "Orders total fetched successfully", ordersTotal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the orders total");
            return new Response<decimal>(500, "An error occurred while fetching the orders total");
        }
    }

    public async Task<Response<TimeSpan>> GetAvgOrderTimeSpanAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.IdentityUserId == userId);

        if (employee == null)
        {
            return new Response<TimeSpan>(400, "Failed to get avg TimeSpan. Either no EmployeeId or not logged in");
        }

        try
        {
            var timeSpans = await _context.Orders
                .Where(o =>
                    o.CreatedAt.Date == DateTime.Now.Date && o.WaiterId == employee.Id &&
                    o.Status == OrderStatus.Paid && o.CompletedAt.HasValue)
                .Select(o => o.CompletedAt.Value - o.CreatedAt)
                .ToListAsync();

            if (timeSpans.Count == 0)
            {
                _logger.LogInformation("No orders found for average calculation");
                return new Response<TimeSpan>(200, "No orders found for average calculation", TimeSpan.Zero);
            }

            var avgTicks = timeSpans.Average(ts => ts.Ticks);
            var avgTimeSpan = TimeSpan.FromTicks((long)avgTicks);

            _logger.LogInformation("Avg order time fetched successfully");
            return new Response<TimeSpan>(200, "Avg order time fetched successfully", avgTimeSpan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the avg order time");
            return new Response<TimeSpan>(500, "An error occurred while fetching the avg order time");
        }
    }
}