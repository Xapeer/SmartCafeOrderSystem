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
    public async Task<IActionResult> GetAllTables([FromQuery] TableFilter filter)
    {
        var response = await service.GetAllTables(filter);
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
}