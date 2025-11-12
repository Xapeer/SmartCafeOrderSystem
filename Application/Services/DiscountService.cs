using Application.Common;
using Application.Dtos.Category;
using Application.Dtos.Discount;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DiscountService : IDiscountService
{
    private IDataContext _context;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDataContext context, ILogger<DiscountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResponse<GetDiscountDto>> GetAllActiveDiscountsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Discounts
            .Where(d => d.IsActive && d.EndTime > DateTime.Now.AddHours(5) && d.StartTime < DateTime.Now.AddHours(5));

        var totalRecords = await query.CountAsync();

        var discounts = await query
            .OrderBy(d => d.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new GetDiscountDto()
            {
                Id = d.Id,
                DiscountPercent = d.DiscountPercent,
                StartTime = d.StartTime,
                EndTime = d.EndTime
            })
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} active discounts", discounts.Count);
        return new PagedResponse<GetDiscountDto>(discounts, pageNumber, pageSize, totalRecords)
        {
            Message = "Active discounts fetched successfully"
        };
    }
    
    public async Task<Response<GetDiscountDto>> CreateDiscountAsync(CreateDiscountDto dto)
    {
        var isOverlapping = await _context.Discounts
            .Where(d => d.IsActive && d.EndTime > DateTime.Now.AddHours(5))
            .AnyAsync(d => d.StartTime < dto.EndTime && d.EndTime > dto.StartTime);

        if (isOverlapping)
            return new Response<GetDiscountDto>(400, "Discount for the given period already exists");

        var discount = new Discount()
        {
            DiscountPercent = dto.DiscountPercent,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true
        };

        try
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            var result = new GetDiscountDto()
            {
                Id = discount.Id,
                DiscountPercent = discount.DiscountPercent,
                StartTime = discount.StartTime,
                EndTime = discount.EndTime
            };

            _logger.LogInformation("Created new discount {Discount}", discount.Id);
            return new Response<GetDiscountDto>(200, "Discount created successfully", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating discount");
            return new Response<GetDiscountDto>(500, "An error occurred while creating the discount");
        }
    }

    public async Task<Response<bool>> EndDiscountAsync(int discountId)
    {
        var discount = await _context.Discounts
            .FirstOrDefaultAsync(c => c.Id == discountId && c.IsActive);

        if (discount == null)
        {
            return new Response<bool>(400, "Discount not found or already ended", false);
        }

        try
        {
            discount.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ended discount {Discount}", discount.Id);
            return new Response<bool>(200, "Discount ended successfully", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending discount");
            return new Response<bool>(500, "An error occurred while ending the discount");
        }
    }
}