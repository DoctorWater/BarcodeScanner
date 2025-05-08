using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BarcodeDecodeAccessControl;

public class IdentityDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        
        const string adminUserName = "Admin";
        const string adminPassword = "Pass12345!#";
        var admin = await userManager.FindByNameAsync(adminUserName);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminUserName,
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
            else
            {
                throw new Exception($"Не удалось создать пользователя Admin: {string.Join(", ", result.Errors)}");
            }
        }
    }
}