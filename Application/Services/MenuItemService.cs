using System.ComponentModel.Design;
using Application.Common;
using Application.Dtos.MenuItem;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class MenuItemService : IMenuItemService
{
    private IDataContext _context;
    private readonly ILogger<MenuItemService> _logger;

    public MenuItemService(IDataContext context, ILogger<MenuItemService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Response<GetMenuItemDto>> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        // Check if Category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return new Response<GetMenuItemDto>(400, "Invalid category ID");
        }
        
        // Check if MenuItem with this name exists
        var existingMenuItem = await _context.MenuItems
            .FirstOrDefaultAsync(c => c.Name == dto.Name);

        if (existingMenuItem != null)
        {
            return new Response<GetMenuItemDto>(400, "MenuItem with this name already exists");
        }

        var menuItem = new MenuItem
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            PrepTime = dto.PrepTime,
            CategoryId = dto.CategoryId
        };

        try
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var result = new GetMenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                PrepTime = menuItem.PrepTime,
                IsActive = menuItem.IsActive,
                CategoryId = menuItem.CategoryId,
                CategoryName = (await _context.Categories.FirstAsync(c => c.Id == menuItem.CategoryId)).Name
            };

            _logger.LogInformation("Created new menu item {MenuItemId} with name {Name}", menuItem.Id, menuItem.Name);
            return new Response<GetMenuItemDto>(200, "Menu item created successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating menu item");
            return new Response<GetMenuItemDto>(500, "An error occurred while creating the menu item");
        }
    }

    public async Task<Response<GetMenuItemDto>> UpdateMenuItemAsync(UpdateMenuItemDto dto)
    {
        // Check if menu item exists
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == dto.Id);

        if (menuItem == null)
        {
            return new Response<GetMenuItemDto>(404, "Menu item not found");
        }

        // Check if category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            return new Response<GetMenuItemDto>(400, "Invalid category ID");
        }

        menuItem.Name = dto.Name;
        menuItem.Description = dto.Description;
        menuItem.Price = dto.Price;
        menuItem.PrepTime = dto.PrepTime;
        menuItem.CategoryId = dto.CategoryId;

        try
        {
            await _context.SaveChangesAsync();

            var result = new GetMenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                PrepTime = menuItem.PrepTime,
                IsActive = menuItem.IsActive,
                CategoryId = menuItem.CategoryId,
                CategoryName = (await _context.Categories.FirstAsync(c => c.Id == menuItem.CategoryId)).Name
            };

            _logger.LogInformation("Updated menu item {MenuItemId}", menuItem.Id);
            return new Response<GetMenuItemDto>(200, "Menu item updated successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating menu item {MenuItemId}", dto.Id);
            return new Response<GetMenuItemDto>(500, "An error occurred while updating the menu item");
        }
    }

    public async Task<Response<bool>> DeleteMenuItemAsync(int id)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == id);

        if (menuItem == null)
        {
            return new Response<bool>(404, "Menu item not found", false);
        }

        menuItem.IsActive = false;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Menu item {MenuItemId} marked as inactive", id);
            return new Response<bool>(200, "Menu item deleted successfully (marked as inactive)", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting menu item {MenuItemId}", id);
            return new Response<bool>(500, "An error occurred while deleting the menu item", false);
        }
    }

    public async Task<PagedResponse<GetMenuItemDto>> GetMenuItemsByCategoryAsync(int categoryId, int pageNumber = 1, int pageSize = 10)
    {
        // Check if category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == categoryId);

        if (!categoryExists)
        {
            return new PagedResponse<GetMenuItemDto>(new List<GetMenuItemDto>(), pageNumber, pageSize, 0)
            {
                Message = "Invalid category ID"
            };
        }

        var query = _context.MenuItems
            .Where(mi => mi.CategoryId == categoryId && mi.IsActive)
            .AsQueryable();

        var totalRecords = await query.CountAsync();

        var menuItems = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(mi => new GetMenuItemDto
            {
                Id = mi.Id,
                Name = mi.Name,
                Description = mi.Description,
                Price = mi.Price,
                PrepTime = mi.PrepTime,
                IsActive = mi.IsActive,
                CategoryId = mi.CategoryId,
                CategoryName = mi.Category.Name
            })
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} menu items for category {CategoryId}", menuItems.Count, categoryId);
        return new PagedResponse<GetMenuItemDto>(menuItems, pageNumber, pageSize, totalRecords)
        {
            Message = "Menu items fetched successfully"
        };
    }

    public async Task<PagedResponse<GetMenuItemDto>> SearchMenuItemsByNameAsync(string name, int pageNumber = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new PagedResponse<GetMenuItemDto>(new List<GetMenuItemDto>(), pageNumber, pageSize, 0)
            {
                Message = "Name cannot be empty"
            };
        }

        var query = _context.MenuItems
            .Where(mi => mi.Name.Contains(name) && mi.IsActive)
            .AsQueryable();

        var totalRecords = await query.CountAsync();

        var menuItems = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(mi => new GetMenuItemDto
            {
                Id = mi.Id,
                Name = mi.Name,
                Description = mi.Description,
                Price = mi.Price,
                PrepTime = mi.PrepTime,
                IsActive = mi.IsActive,
                CategoryId = mi.CategoryId,
                CategoryName = mi.Category.Name
            })
            .ToListAsync();

        _logger.LogInformation("Searched for menu items with name containing '{Name}', found {Count}", name, menuItems.Count);
        return new PagedResponse<GetMenuItemDto>(menuItems, pageNumber, pageSize, totalRecords)
        {
            Message = "Menu items searched successfully"
        };
    }
}