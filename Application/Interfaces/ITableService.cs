using Application.Common;
using Application.Dtos.Table;
using Application.Filters.Table;

namespace Application.Interfaces;

public interface ITableService
{
    Task<Response<GetTableDto>> CreateTableAsync(CreateTableDto dto);
    Task<Response<bool>> DeleteTableAsync(int tableId);
    Task<PagedResponse<GetTableDto>> GetAllTablesAsync(TableFilter filter, int pageNumber = 1, int pageSize = 10);
}