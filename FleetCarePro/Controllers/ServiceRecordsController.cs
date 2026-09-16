using System.Security.Claims;
using FleetCarePro.Data;
using FleetCarePro.Services;
using FleetCarePro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FleetCarePro.Controllers;

[Authorize(Policy = "RequireFleetManagerRole")]
public class ServiceRecordsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceRecordService _serviceRecordService;

    public ServiceRecordsController(ApplicationDbContext context, ServiceRecordService serviceRecordService)
    {
        _context = context;
        _serviceRecordService = serviceRecordService;
    }


    public async Task<IActionResult> Index()
    {
        var records = await _context.ServiceRecords
            .Include(s => s.Vehicle)
            .Include(s => s.ServiceCenter)
            .ToListAsync();

        return View(records);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();

        var model = new ServiceRecordCreateViewModel();
        model.LineItems.Add(new ServiceLineItemViewModel());

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRecordCreateViewModel model)
    {
        if (model.LineItems == null || !model.LineItems.Any())
        {
            ModelState.AddModelError("", "At least one service line item is required.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                await _serviceRecordService.CreateServiceRecordAsync(model, userId);

                TempData["SuccessMessage"] = "Service record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        
        await PopulateDropdownsAsync(model.VehicleId, model.ServiceCenterId);
        return View(model);
    }

    private async Task PopulateDropdownsAsync(object? selectedVehicle = null, object? selectedCenter = null)
    {
        ViewBag.Vehicles = new SelectList(await _context.Vehicles.ToListAsync(), "Id", "LicensePlate", selectedVehicle);
        ViewBag.ServiceCenters = new SelectList(await _context.ServiceCenters.Where(c => c.IsActive).ToListAsync(), "Id", "Name", selectedCenter);
        ViewBag.ServiceCategories = new SelectList(await _context.ServiceCategories.ToListAsync(), "Id", "CategoryName");
    }
}