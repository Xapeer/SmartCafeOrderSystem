using Application.Common;
using Application.Dtos.MenuItem;

namespace Application.Interfaces;

public interface IReportService
{
    Task<Response<decimal>> GetTodayRevenueAsync();
    Task<Response<decimal>> GetAverageCheckAsync();
    Task<Response<int>> GetOrderCountAsync();
    Task<PagedResponse<PopularMenuItemDto>> GetPopularMenuItemsAsync(int pageNumber = 1, int pageSize = 10);
    Task<Response<TimeSpan>> GetAverageOrderDurationAsync();
}