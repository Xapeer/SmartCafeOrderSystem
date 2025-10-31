namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}