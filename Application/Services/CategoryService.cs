using Application.Common;
using Application.Dtos.Category;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private IDataContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IDataContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<PagedResponse<GetCategoryDto>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Categories
            .Where(c => c.IsActive)
            .AsQueryable();

        var totalRecords = await query.CountAsync();

        var categories = await query
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive
            })
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} categories", categories.Count);
        return new PagedResponse<GetCategoryDto>(categories, pageNumber, pageSize, totalRecords)
        {
            Message = "Categories fetched successfully"
        };
    }
    public async Task<Response<GetCategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == dto.Name);

        if (existingCategory != null)
        {
            return new Response<GetCategoryDto>(400, "Category with this name already exists");
        }

        var category = new Category
        {
            Name = dto.Name
        };

        try
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };

            _logger.LogInformation("Created new category {CategoryId} with name {Name}", category.Id, category.Name);
            return new Response<GetCategoryDto>(200, "Category created successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return new Response<GetCategoryDto>(500, "An error occurred while creating the category");
        }
    }

    public async Task<Response<GetCategoryDto>> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.Id);

        if (category == null)
        {
            return new Response<GetCategoryDto>(404, "Category not found");
        }

        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != dto.Id);

        if (existingCategory != null)
        {
            return new Response<GetCategoryDto>(400, "Category with this name already exists");
        }

        category.Name = dto.Name;

        try
        {
            await _context.SaveChangesAsync();

            var result = new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive
            };

            _logger.LogInformation("Updated category {CategoryId}", category.Id);
            return new Response<GetCategoryDto>(200, "Category updated successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", dto.Id);
            return new Response<GetCategoryDto>(500, "An error occurred while updating the category");
        }
    }

    public async Task<Response<bool>> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return new Response<bool>(404, "Category not found", false);
        }

        category.IsActive = false;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category {CategoryId} marked as inactive", id);
            return new Response<bool>(200, "Category deleted successfully (marked as inactive)", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return new Response<bool>(500, "An error occurred while deleting the category", false);
        }
    }

    public async Task<Response<bool>> AddMenuItemToCategoryAsync(int categoryId, int menuItemId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
        {
            return new Response<bool>(404, "Category not found", false);
        }

        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == menuItemId);

        if (menuItem == null)
        {
            return new Response<bool>(404, "Menu item not found", false);
        }

        menuItem.CategoryId = categoryId;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added menu item {MenuItemId} to category {CategoryId}", menuItemId, categoryId);
            return new Response<bool>(200, "Menu item added to category successfully", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding menu item {MenuItemId} to category {CategoryId}", menuItemId, categoryId);
            return new Response<bool>(500, "An error occurred while adding menu item to category", false);
        }
    }

    public async Task<Response<bool>> RemoveMenuItemFromCategoryAsync(int menuItemId)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == menuItemId);

        if (menuItem == null)
        {
            return new Response<bool>(404, "Menu item not found", false);
        }

        // Change Category to "Uncategorized" because of a foreign key constraints
        menuItem.CategoryId = _context.Categories.Where(c => c.Name == "Uncategorized").FirstOrDefault().Id;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Removed menu item {MenuItemId} from its category", menuItemId);
            return new Response<bool>(200, "Menu item removed from category successfully", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing menu item {MenuItemId} from category", menuItemId);
            return new Response<bool>(500, "An error occurred while removing menu item from category", false);
        }
    }
    
}