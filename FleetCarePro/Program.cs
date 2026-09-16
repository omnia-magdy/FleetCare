using FleetCarePro.Data;
using FleetCarePro.Middlewares;
using FleetCarePro.Models;
using FleetCarePro.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<ServiceRecordService>();
builder.Services.AddScoped<VehicleService>();


// 2. Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Admin Policy: Full access
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    // FleetManager Policy: Manage Fleet & Approvals
    options.AddPolicy("RequireFleetManagerRole", policy =>
        policy.RequireRole("Admin", "FleetManager"));

    // Driver Access Policy: Driver specific views
    options.AddPolicy("RequireDriverRole", policy =>
        policy.RequireRole("Admin", "FleetManager", "Driver"));
});

var app = builder.Build();


//  Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedRolesAndUsersAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseMiddleware<MaintenanceModeMiddleware>();

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();