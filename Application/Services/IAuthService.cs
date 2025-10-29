using Application.Common;
using Application.Dtos;

namespace Application.Services;

public interface IAuthService
{
    Task<Response<RegisterDto>> RegisterAsync(RegisterDto dto);
    Task<Response<TokenDto>> LoginAsync(LoginDto dto);
}