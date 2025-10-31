using Application.Dtos.Category;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService service) : Controller
{
    [HttpPost("create-category")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var response = await service.CreateCategoryAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("update-category")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto dto)
    {
        var response = await service.UpdateCategoryAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("delete-category")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await service.DeleteCategoryAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("add-menu-item-to-category")]
    public async Task<IActionResult> AddMenuItemToCategory(int categoryId, int menuItemId)
    {
        var response = await service.AddMenuItemToCategoryAsync(categoryId, menuItemId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("remove-menu-item-from-category")]
    public async Task<IActionResult> RemoveMenuItemFromCategory(int menuItemId)
    {
        var response = await service.RemoveMenuItemFromCategoryAsync(menuItemId);
        return StatusCode(response.StatusCode, response);
    }
}