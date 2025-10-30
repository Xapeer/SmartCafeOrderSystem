using Domain.Entities;

namespace Application.Filters.Order;

public class SingleOrderFilter
{
    public int OrderId { get; set; }
    public int TableId { get; set; }
    public int WaiterId { get; set; }
}