namespace Application.Dtos.MenuItem;

public class UpdateMenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public TimeSpan PrepTime { get; set; }
    public int CategoryId { get; set; }
}