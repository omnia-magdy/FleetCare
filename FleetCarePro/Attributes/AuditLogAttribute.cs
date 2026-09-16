using System.Security.Claims;
using System.Text.Json;
using FleetCarePro.Data;
using FleetCarePro.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace FleetCarePro.Attributes;

public class AuditLogAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (resultContext.Exception == null)
        {
            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            if (dbContext != null)
            {
                var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
                var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
                var actionName = context.RouteData.Values["action"]?.ToString() ?? "Unknown";
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

                var actionArguments = JsonSerializer.Serialize(context.ActionArguments);

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    ControllerName = controllerName,
                    ActionName = actionName,
                    IPAddress = ipAddress,
                    Details = actionArguments,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}