namespace FleetCarePro.Middlewares;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public MaintenanceModeMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool isMaintenanceMode = _configuration.GetValue<bool>("AppSettings:IsMaintenanceMode");

        var path = context.Request.Path.Value?.ToLower() ?? "";

        
        bool isMaintenancePage = path.Contains("/home/maintenance");
        bool isStaticFile = path.Contains(".") || path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js");

        if (isMaintenanceMode && !isMaintenancePage && !isStaticFile)
        {
            context.Response.Redirect("/Home/Maintenance");
            return;
        }

        await _next(context);
    }
}