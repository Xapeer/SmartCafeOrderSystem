using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataContext _context;
    protected readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IDataContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _logger = logger;
    }

    private async Task<TokenDto> GenerateJwtToken(IdentityUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var name = _context.Employees
            .Where(e => e.IdentityUserId == user.Id)
            .Select(e => e.Name)
            .FirstOrDefault();

        if (name != null)
        {
            claims.Add(new Claim(ClaimTypes.GivenName, name));
        }

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
    
    public Response<ProfileDto> Profile()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var employee = _context.Employees.FirstOrDefault(e => e.IdentityUserId == userId);

        return new Response<ProfileDto>(new ProfileDto()
        {
            UserId = userId,
            EmployeeId = employee?.Id,
            Name = user?.FindFirst(ClaimTypes.GivenName)?.Value,
            Username = user?.FindFirst(ClaimTypes.Name)?.Value,
            Role = user?.FindFirst(ClaimTypes.Role)?.Value,
        });
    }
    
    public async Task<Response<string>> GetRoleFromTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new Response<string>(400, "Token is required");

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var role = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
                return new Response<string>(404, "Role not found in token");

            return new Response<string>(200, "Role retrieved successfully", role);
        }
        catch (SecurityTokenExpiredException)
        {
            return new Response<string>(401, "Token has expired");
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Invalid token");
            return new Response<string>(401, "Invalid token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return new Response<string>(500, "An error occurred while validating the token");
        }
    }

    public async Task<PagedResponse<GetWaiterDto>> GetWaitersAsync(int pageNumber = 1, int pageSize = 10)
    {
        var waiters = await _context.Employees
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(employee => new GetWaiterDto
            {
                Id = employee.Id,
                Name = employee.Name,
            }).ToListAsync();
        
        var totalRecords = waiters.Count;
        
        return new PagedResponse<GetWaiterDto>(waiters, pageNumber, pageSize, totalRecords);
    }
}