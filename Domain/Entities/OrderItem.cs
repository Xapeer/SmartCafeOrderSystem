namespace Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal PriceAtOrderTime { get; set; }
    public string Notes { get; set; } = string.Empty;
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Started;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public Order Order { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}

public enum OrderItemStatus
{
    Started,
    Ready,
    Cancelled
}