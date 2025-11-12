using Domain.Entities;

namespace Application.Dtos.Order;

public class GetOrderForStatsDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TableId { get; set; }
    public string WaiterName { get; set; }
    public decimal CurrentTotal { get; set; }
    public OrderStatus Status { get; set; }
}