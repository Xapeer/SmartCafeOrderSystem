using Application.Common;
using Application.Dtos;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<Response<RegisterDto>> RegisterAsync(RegisterDto dto);
    Task<Response<TokenDto>> LoginAsync(LoginDto dto);
    Response<ProfileDto> Profile();
    Task<Response<string>> GetRoleFromTokenAsync(string token);
    Task<PagedResponse<GetWaiterDto>> GetWaitersAsync(int pageNumber = 1, int pageSize = 10);
}