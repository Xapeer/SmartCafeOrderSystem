using Application.Common;

namespace Application.Interfaces;

public interface IWaiterService
{
    Task<Response<int>> GetNumberOfOrdersAsync();
    Task<Response<decimal>> GetOrdersTotalAsync();
    Task<Response<TimeSpan>> GetAvgOrderTimeSpanAsync();
}