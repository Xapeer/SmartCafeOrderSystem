using Application.Common;
using Application.Dtos.MenuItem;

namespace Application.Interfaces;

public interface IMenuItemService
{
    Task<Response<GetMenuItemDto>> CreateMenuItemAsync(CreateMenuItemDto dto);
    Task<Response<GetMenuItemDto>> UpdateMenuItemAsync(UpdateMenuItemDto dto);
    Task<Response<bool>> DeleteMenuItemAsync(int id);
    Task<PagedResponse<GetMenuItemDto>> GetMenuItemsByCategoryAsync(int categoryId, int pageNumber = 1, int pageSize = 10);
    Task<PagedResponse<GetMenuItemDto>> SearchMenuItemsByNameAsync(string name, int pageNumber = 1, int pageSize = 10);
    Task<PagedResponse<GetMenuItemDto>> GetAllMenuItemsAsync(bool onlyActive = true, int pageNumber = 1, int pageSize = 10);
    Task<Response<bool>> ActivateMenuItem(int id);
}