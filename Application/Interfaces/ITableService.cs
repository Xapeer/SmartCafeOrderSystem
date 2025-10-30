using Application.Common;
using Application.Dtos.Table;
using Application.Filters.Table;

namespace Application.Interfaces;

public interface ITableService
{
    Task<Response<List<GetTableDto>>> GetAllTables(TableFilter filter);
    Task<Response<GetTableDto>> CreateTableAsync(CreateTableDto dto);
    Task<Response<bool>> DeleteTableAsync(int tableId);
}