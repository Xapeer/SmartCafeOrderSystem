using Application.Common;
using Application.Dtos.MenuItem;
using Application.Dtos.Report;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReportService : IReportService
{
    private IDataContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IDataContext context, ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Response<decimal>> GetTodayRevenueAsync()
    {
        var revenue = await _context.Orders
            .Where(o => o.CreatedAt.Date == DateTime.Now.AddHours(5).Date && o.Status == OrderStatus.Paid)
            .SumAsync(o => o.TotalAmount);
        
        return new Response<decimal>(200, "Revenue has been retrieved successfully", revenue);
    }
    public async Task<Response<decimal>> GetAverageCheckAsync()
    {
        var avgCheck = await _context.Orders
            .Where(o => o.CreatedAt.Date == DateTime.Now.AddHours(5).Date && o.Status == OrderStatus.Paid)
            .AverageAsync(o => o.TotalAmount);
        
        return new Response<decimal>(200, "Average check has been retrieved successfully", avgCheck);
    }
    public async Task<Response<int>> GetOrderCountAsync()
    {
        var orderCount = await _context.Orders
            .Where(o => o.CreatedAt.Date == DateTime.Now.AddHours(5).Date && o.Status == OrderStatus.Paid)
            .CountAsync();
        
        return new Response<int>(200, "Order count has been retrieved successfully", orderCount);
    }
    public async Task<PagedResponse<PopularMenuItemDto>> GetPopularMenuItemsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.OrderItems
            .Where(oi => oi.Status == OrderItemStatus.Served && oi.StartedAt.Value.Date == DateTime.Now.AddHours(5).Date)
            .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.Name })
            .Select(g => new PopularMenuItemDto
            {
                MenuItemId = g.Key.MenuItemId,
                MenuItemName = g.Key.Name,
                OrdersCount = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(p => p.OrdersCount);

        var totalRecords = await query.CountAsync();

        var popularMenuItems = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} popular menu items for today", popularMenuItems.Count);
        return new PagedResponse<PopularMenuItemDto>(popularMenuItems, pageNumber, pageSize, totalRecords)
        {
            Message = "Popular menu items fetched successfully"
        };
    }
    public async Task<Response<TimeSpan>> GetAverageOrderDurationAsync()
    {
        try
        {
            var timeSpans = await _context.Orders
                .Where(o =>
                    o.CreatedAt.Date == DateTime.Now.AddHours(5).Date &&
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
    public async Task<PagedResponse<WaiterKpiDto>> GetWaiterRatingAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = from e in _context.Employees
            join o in _context.Orders on e.Id equals o.WaiterId
            where o.Status == OrderStatus.Paid && o.CreatedAt.Date == DateTime.Now.AddHours(5).Date
            group o by new { e.Id, e.Name } into g
            select new WaiterKpiDto
            {
                EmployeeId = g.Key.Id,
                EmployeeName = g.Key.Name,
                TotalRevenue = g.Sum(o => o.TotalAmount)
            };

        var kpiList = await query.ToListAsync();
        
        kpiList = kpiList.OrderByDescending(k => k.TotalRevenue).ToList();

        var totalRecords = kpiList.Count;

        var pagedKpiList = kpiList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        _logger.LogInformation("Fetched waiters rating");
        return new PagedResponse<WaiterKpiDto>(pagedKpiList, pageNumber, pageSize, totalRecords)
        {
            Message = "Waiters rating fetched successfully"
        };
    }
}