using Microsoft.AspNetCore.Identity;
using UserRoles.Data;
using UserRoles.Models;

namespace UserRoles.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                // Ensure the database is ready
                logger.LogInformation("Ensuring the database is created.");
                await context.Database.EnsureCreatedAsync();

                // Add roles
                logger.LogInformation("Seeding roles.");
                await AddRoleAsync(roleManager, "Admin", logger);
                await AddRoleAsync(roleManager, "User", logger);
                await AddRoleAsync(roleManager, "Seller", logger);

                // Add admin user
                logger.LogInformation("Seeding admin user.");
                var adminEmail = "Admin@gmail.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new Users
                    {
                        FullName = "Admin Admin",
                        UserName = adminEmail,
                        NormalizedUserName = adminEmail.ToUpper(),
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Assigning Admin role to the admin user.");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Note: Seller users will be created by admin through the application
                // rather than being seeded here
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw; // Re-throw to ensure the error is not silently ignored
            }
        }

        private static async Task AddRoleAsync(
            RoleManager<IdentityRole> roleManager,
            string roleName,
            ILogger logger)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                logger.LogInformation("Creating {Role} role", roleName);
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create {Role} role: {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new Exception($"Failed to create role '{roleName}'");
                }

                logger.LogInformation("Successfully created {Role} role", roleName);
            }
            else
            {
                logger.LogInformation("{Role} role already exists", roleName);
            }
        }
    }
}