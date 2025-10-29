using Microsoft.AspNetCore.Identity;

namespace Infrastructure;

public class Seeder
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public Seeder(DataContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedRole()
    {
        var newroles = new List<IdentityRole>()
        {
            new (Roles.Admin),
            new (Roles.Waiter),
            new (Roles.Kitchen),
        };

        var existing = _roleManager.Roles.ToList();
        foreach (var role in newroles)
        {
            if (existing.Exists(e => e.Name == role.Name) == false)
            {
                await _roleManager.CreateAsync(role);
            }
        }
    }


    public async Task SeedAdmin()
    {
        var existing = await _userManager.FindByNameAsync("admin");
        if (existing is not null) return;
        
        var identity = new IdentityUser()
        {
            UserName = "admin",
        };

        var result = await _userManager.CreateAsync(identity, "admin123");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(identity, Roles.Admin);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Couldn't create admin: {errors}");
        }
    }
    
    public async Task SeedKitchen()
    {
        var existing = await _userManager.FindByNameAsync("kitchen");
        if (existing is not null) return;
        
        var identity = new IdentityUser()
        {
            UserName = "kitchen",
        };

        var result = await _userManager.CreateAsync(identity, "kitchen123");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(identity, Roles.Kitchen);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"Couldn't create kitchen: {errors}");
        }
    }
}

public class Roles
{
    public const string Admin = "Admin";
    public const string Waiter = "Waiter";
    public const string Kitchen = "Kitchen";
}