﻿namespace Application.Dtos.Category;

public class GetCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}