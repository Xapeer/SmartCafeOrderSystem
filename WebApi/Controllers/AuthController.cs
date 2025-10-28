using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class AuthController(IAuthService service): Controller
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var response  = await service.RegisterAsync(registerDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto registerDto)
    {
        var response  = await service.LoginAsync(registerDto);
        return StatusCode(response.StatusCode, response);
    }
    
}