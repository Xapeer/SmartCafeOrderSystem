using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private IDataContext _context;

    public AuthService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IDataContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context;
    }

    private async Task<TokenDto> GenerateJwtToken(IdentityUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expireHours = double.Parse(_configuration["Jwt:ExpireHours"] ?? "1");
        var expiresIn = DateTime.UtcNow.AddHours(expireHours);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresIn,
            signingCredentials: credentials
        );

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = expiresIn
        };
    }

    public async Task<Response<RegisterDto>> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByNameAsync(dto.Username) != null)
            return new Response<RegisterDto>(400, "User already exists");

        var user = new IdentityUser { UserName = dto.Username };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new Response<RegisterDto>(400, errors);
        }
        
        await _userManager.AddToRoleAsync(user, "Waiter");

        var employee = new Employee()
        {
            Name = dto.Name,
            IdentityUserId = user.Id
        };
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        return new Response<RegisterDto>(200, "User registered successfully");
    }

    public async Task<Response<TokenDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return new Response<TokenDto>(400, "Login or password is incorrect");

        var token = await GenerateJwtToken(user);
        return new Response<TokenDto>(200, "Login successful", token);
    }
}