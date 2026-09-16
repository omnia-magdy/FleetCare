using FleetCarePro.Data;
using FleetCarePro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetCarePro.ViewComponents;

public class OverdueMaintenanceViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public OverdueMaintenanceViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        DateTime sixMonthsAgo = DateTime.Today.AddMonths(-6);

        var overdueVehicles = await _context.Vehicles
            .Where(v => !v.ServiceRecords.Any(sr => sr.ServiceDate >= sixMonthsAgo))
            .Select(v => new OverdueVehicleItem
            {
                VehicleId = v.Id,
                LicensePlate = v.LicensePlate,
                Make = v.Make,
                Model = v.Model,
                LastServiceDate = v.ServiceRecords
                    .OrderByDescending(sr => sr.ServiceDate)
                    .Select(sr => (DateTime?)sr.ServiceDate)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return View(overdueVehicles);
    }
}

public class OverdueVehicleItem
{
    public int VehicleId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime? LastServiceDate { get; set; }
}