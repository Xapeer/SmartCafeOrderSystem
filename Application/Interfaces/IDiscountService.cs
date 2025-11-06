using Application.Common;
using Application.Dtos.Discount;

namespace Application.Interfaces;

public interface IDiscountService
{
    Task<Response<GetDiscountDto>> CreateDiscountAsync(CreateDiscountDto dto);
    Task<Response<bool>> EndDiscountAsync(int discountId);
    Task<PagedResponse<GetDiscountDto>> GetAllActiveDiscountsAsync(int pageNumber = 1, int pageSize = 10);
}