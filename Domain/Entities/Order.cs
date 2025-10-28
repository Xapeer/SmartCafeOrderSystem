namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public OrderStatus Status { get; set; } = OrderStatus.Created; 

    public int TableId { get; set; }
    public int WaiterId { get; set; }
    public int? DiscountId { get; set; }
    
    public Table Table { get; set; } = null!;
    public Employee Waiter { get; set; } = null!;
    public Discount? Discount { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Created,
    Paid,
    Cancelled
}