using Application.Common;
using Application.Dtos;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<Response<RegisterDto>> RegisterAsync(RegisterDto dto);
    Task<Response<TokenDto>> LoginAsync(LoginDto dto);
    Response<ProfileDto> Profile();
}