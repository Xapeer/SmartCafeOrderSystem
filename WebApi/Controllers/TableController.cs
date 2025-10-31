using Application.Dtos.Table;
using Application.Filters.Table;
using Application.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

//[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]
public class TableController(ITableService service) : Controller
{

    [HttpGet("get-all-tables")]
    public async Task<IActionResult> GetAllTables(
        [FromQuery] TableFilter filter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetAllTablesAsync(filter, pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("create-table")]
    public async Task<IActionResult> CreateTable([FromBody] CreateTableDto dto)
    {
        var response = await service.CreateTableAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("deactivate-table")]
    public async Task<IActionResult> DeleteTable(int tableId)
    {
        var response = await service.DeleteTableAsync(tableId);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut("activate-table")]
    public async Task<IActionResult> ActivateTable(int tableId)
    {
        var response = await service.ActivateTableAsync(tableId);
        return StatusCode(response.StatusCode, response);
    }
}