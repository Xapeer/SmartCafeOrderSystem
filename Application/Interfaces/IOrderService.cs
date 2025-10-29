using Application.Common;
using Application.Dtos.Order;
using Application.Dtos.OrderItem;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto dto);
    Task<Response<GetOrderItemDto>> AddOrderItemAsync(int orderId, int menuItemId);
    Task<Response<bool>> RemoveOrderItemAsync(int orderId, int orderItemId);
    Task<Response<bool>> ConfirmOrderAsync(int orderId);
    Task<Response<bool>> CancelOrderAsync(int orderId);
}