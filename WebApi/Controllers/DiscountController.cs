using Application.Dtos.Discount;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountController(IDiscountService service) :  Controller
{
    [HttpGet("get-all-discounts")]
    public async Task<IActionResult> GetAllDiscounts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetAllActiveDiscountsAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("create-discount")]
    public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto dto)
    {
        var response = await service.CreateDiscountAsync(dto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete("end-discount")]
    public async Task<IActionResult> EndDiscount(int discountId)
    {
        var response = await service.EndDiscountAsync(discountId);
        return StatusCode(response.StatusCode, response);
    }
}