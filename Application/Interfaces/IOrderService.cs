using Application.Common;
using Application.Dtos.Order;
using Application.Dtos.OrderItem;
using Application.Filters.Order;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<Response<GetOrderWithItemsDto>> GetSingleOrderAsync(SingleOrderFilter filter);
    Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto dto);
    Task<Response<GetOrderItemDto>> AddOrderItemAsync(int orderId, int menuItemId);
    Task<Response<bool>> RemoveOrderItemAsync(int orderId, int orderItemId);
    Task<Response<bool>> ConfirmOrderAsync(int orderId);
    Task<Response<bool>> PayForOrderAsync(int orderId);
    Task<Response<bool>> CancelOrderAsync(int orderId);
    Task<PagedResponse<GetOrderDto>> GetAllOrdersAsync(AllOrderFilter filter, int pageNumber = 1, int pageSize = 10);
    Task<Response<decimal>> GetOrderTotalAsync(int orderId);
    Task<Response<bool>> ServeOrderItemAsync(int orderItemId);
}
