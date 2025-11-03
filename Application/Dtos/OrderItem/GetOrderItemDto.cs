using Domain.Entities;

namespace Application.Dtos.OrderItem;

public class GetOrderItemDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtOrderTime { get; set; }
    public string MenuItemName { get; set; }
    public string Notes { get; set; }
    public OrderItemStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}