using Application.Common;
using Application.Dtos.Kitchen;

namespace Application.Interfaces;

public interface IKitchenQueueService
{
    Task<bool> EnqueueOrderItemAsync(int orderItemId);
    Task<Response<List<GetOrderItemForKitchenDto>>> GetQueueAsync();
    Task<Response<bool>> MarkAsReadyAsync(int orderItemId);
}