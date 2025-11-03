using Application.Dtos.Category;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService service) : Controller
{
    [HttpGet("get-all-categories")]
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] bool onlyActive = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetAllCategoriesAsync(onlyActive, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
    
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
    
    [HttpPut("activate-category")]
    public async Task<IActionResult> ActivateCategory(int id)
    {
        var response = await service.ActivateCategoryAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("add-menu-item-to-category")]
    public async Task<IActionResult> AddMenuItemToCategory(int categoryId, int menuItemId)
    {
        var response = await service.AddMenuItemToCategoryAsync(categoryId, menuItemId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("remove-menu-item-from-category")]
    public async Task<IActionResult> RemoveMenuItemFromCategory(int menuItemId)
    {
        var response = await service.RemoveMenuItemFromCategoryAsync(menuItemId);
        return StatusCode(response.StatusCode, response);
    }
}