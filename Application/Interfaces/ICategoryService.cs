using Application.Common;
using Application.Dtos.Category;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<Response<GetCategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<Response<GetCategoryDto>> UpdateCategoryAsync(UpdateCategoryDto dto);
    Task<Response<bool>> DeleteCategoryAsync(int id);
    Task<Response<bool>> AddMenuItemToCategoryAsync(int categoryId, int menuItemId);
    Task<Response<bool>> RemoveMenuItemFromCategoryAsync(int menuItemId);
    Task<PagedResponse<GetCategoryDto>> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
}