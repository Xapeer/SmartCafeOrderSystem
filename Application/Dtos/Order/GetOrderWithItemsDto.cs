using Application.Dtos.OrderItem;
using Domain.Entities;

namespace Application.Dtos.Order;

public class GetOrderWithItemsDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public OrderStatus Status { get; set; }

    public int TableId { get; set; }
    public int WaiterId { get; set; }
    public int? DiscountId { get; set; }
    
    public ICollection<GetOrderItemDto> OrderItems { get; set; }
}