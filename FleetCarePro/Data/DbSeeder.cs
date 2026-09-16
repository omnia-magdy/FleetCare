using FleetCarePro.Models;
using Microsoft.AspNetCore.Identity;

namespace FleetCarePro.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "FleetManager", "Driver" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@fleetcare.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                EmployeeId = "EMP-001"
            };

            var result = await userManager.CreateAsync(admin, "AdminPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        var managerEmail = "manager@fleetcare.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        if (managerUser == null)
        {
            var manager = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                FullName = "Ahmed Manager",
                EmployeeId = "EMP-002"
            };

            var result = await userManager.CreateAsync(manager, "ManagerPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, "FleetManager");
            }
        }

        var driverEmail = "driver@fleetcare.com";
        var driverUser = await userManager.FindByEmailAsync(driverEmail);
        if (driverUser == null)
        {
            var driver = new ApplicationUser
            {
                UserName = driverEmail,
                Email = driverEmail,
                EmailConfirmed = true,
                FullName = "Mohamed Driver",
                EmployeeId = "EMP-003"
            };

            var result = await userManager.CreateAsync(driver, "DriverPass123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(driver, "Driver");
            }
        }
    }
}