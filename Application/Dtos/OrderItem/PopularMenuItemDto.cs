namespace Application.Dtos.OrderItem;

public class PopularMenuItemDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = null!;
    public int OrdersCount { get; set; }
    public decimal TotalRevenue { get; set; }
}