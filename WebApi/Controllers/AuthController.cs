using Application.Dtos;
using Application.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(IAuthService service): Controller
{
    [HttpPost("register")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var response  = await service.RegisterAsync(registerDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto registerDto)
    {
        var response  = await service.LoginAsync(registerDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var response  = service.Profile();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-role-from-token")]
    public async Task<IActionResult> GetRoleFromToken([FromBody] string token)
    {
        var response = await service.GetRoleFromTokenAsync(token);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-all-waiters")]
    public async Task<IActionResult> GetAllWaiters(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await service.GetWaitersAsync(pageNumber, pageSize);
        return StatusCode(response.StatusCode, response);
    }
}