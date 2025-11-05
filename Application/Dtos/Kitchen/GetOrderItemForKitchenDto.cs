using Domain.Entities;

namespace Application.Dtos.Kitchen;

public class GetOrderItemForKitchenDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public OrderItemStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
}