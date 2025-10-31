using Application.Common;
using Application.Dtos.OrderItem;

namespace Application.Interfaces;

public interface IReportService
{
    public Task<Response<decimal>> GetTodayRevenueAsync();
    
    public Task<Response<decimal>> GetAverageCheckAsync(DateTime? date = null);
    
    public Task<Response<int>> GetOrderCountAsync(DateTime? date = null);
    
    public Task<Response<List<PopularMenuItemDto>>> GetPopularMenuItemsAsync(DateTime? from = null, DateTime? to = null);
    
    public Task<Response<TimeSpan>> GetAverageOrderDurationAsync(DateTime? from = null, DateTime? to = null);
}