using Application.Common;
using Application.Dtos.OrderItem;
using Application.Interfaces;
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
    
    public Task<Response<decimal>> GetTodayRevenueAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Response<decimal>> GetAverageCheckAsync(DateTime? date = null)
    {
        throw new NotImplementedException();
    }

    public Task<Response<int>> GetOrderCountAsync(DateTime? date = null)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<PopularMenuItemDto>>> GetPopularMenuItemsAsync(DateTime? from = null, DateTime? to = null)
    {
        throw new NotImplementedException();
    }

    public Task<Response<TimeSpan>> GetAverageOrderDurationAsync(DateTime? from = null, DateTime? to = null)
    {
        throw new NotImplementedException();
    }
}