using Application.Common;
using Application.Dtos.Order;
using Application.Dtos.Table;
using Application.Filters.Table;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TableService : ITableService
{
    private IDataContext _context;
    private readonly ILogger<TableService> _logger;

    public TableService(IDataContext context, ILogger<TableService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Response<List<GetTableDto>>> GetAllTables(TableFilter filter)
    {
        // Check filtration first
        var query = _context.Tables.AsQueryable();

        if (filter.OnlyActive)
            query = query.Where(table => table.IsActive);
        if (filter.OnlyFree)
            query = query.Where(table => table.IsFree);

        try
        {
            var tables = await query.Select(table => new GetTableDto()
            {
                Id = table.Id,
                NumberOfSeats = table.NumberOfSeats,
                IsActive = table.IsActive,
                IsFree = table.IsFree
            }).ToListAsync();
        
            _logger.LogInformation("Fetched tables with filters: OnlyActive={OnlyActive}, OnlyFree={OnlyFree}", 
                filter.OnlyActive, filter.OnlyFree);
            return new Response<List<GetTableDto>>(200, "Tables fetched successfully", tables);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error fetching tables");
            return new Response<List<GetTableDto>>(500, "An error occurred while fetching the tables");
        }
    }

    public async Task<Response<GetTableDto>> CreateTableAsync(CreateTableDto dto)
    {
        var table = new Table()
        {
            NumberOfSeats = dto.NumberOfSeats
        };
        
        try
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            
            var result = new GetTableDto()
            {
                Id = table.Id,
                IsActive = table.IsActive,
                IsFree = table.IsFree,
                NumberOfSeats = table.NumberOfSeats
            };
            _logger.LogInformation("Created table {TableId}", result.Id);
            return new Response<GetTableDto>(200, "Table created successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            return new Response<GetTableDto>(500, "An error occurred while creating the table");
        }
    }

    public async Task<Response<bool>> DeleteTableAsync(int tableId)
    {
        // Check if table exists
        var tableExists = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == tableId);

        if (tableExists == null)
            return new Response<bool>(400, "Invalid table ID");

        tableExists.IsActive = false;
        
        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted table {TableId}", tableId);
            return new Response<bool>(200, "Table deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting table");
            return new Response<bool>(500, "An error occurred while deleting the table");
        }
    }
}