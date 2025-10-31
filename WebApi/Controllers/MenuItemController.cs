using Application.Dtos.MenuItem;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController(IMenuItemService service) : Controller
{
    [HttpPost("create-menu-item")]
    public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemDto dto)
    {
        var response = await service.CreateMenuItemAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("update-menu-item")]
    public async Task<IActionResult> UpdateMenuItem([FromBody] UpdateMenuItemDto dto)
    {
        var response = await service.UpdateMenuItemAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("delete-menu-item")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var response = await service.DeleteMenuItemAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-menu-items-by-category")]
    public async Task<IActionResult> GetMenuItemsByCategory(
        int categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetMenuItemsByCategoryAsync(categoryId, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-all-menu-items")]
    public async Task<IActionResult> GetAllMenuItems(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetAllMenuItemsAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("search-menu-items")]
    public async Task<IActionResult> SearchMenuItems(
        [FromQuery] string name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.SearchMenuItemsByNameAsync(name, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
}