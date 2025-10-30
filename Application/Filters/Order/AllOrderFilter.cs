using Domain.Entities;

namespace Application.Filters.Order;

public class AllOrderFilter
{
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public OrderStatus Status { get; set; }
    public int OrderId { get; set; }
    public int TableId { get; set; }
    public int WaiterId { get; set; }
}