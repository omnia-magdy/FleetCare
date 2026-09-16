using FleetCarePro.Data;
using FleetCarePro.Models;
using FleetCarePro.Services;
using FleetCarePro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FleetCarePro.Controllers;

[Authorize(Policy = "RequireFleetManagerRole")]
public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly VehicleService _vehicleService;

    public VehiclesController(ApplicationDbContext context, VehicleService vehicleService)
    {
        _context = context;
        _vehicleService = vehicleService;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.AssignedDriver)
            .ToListAsync();

        return View(vehicles);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDriversDropDownList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleCreateViewModel vehicleVm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDriversDropDownList(vehicleVm.AssignedDriverId);
            return View(vehicleVm);
        }

        if (await _context.Vehicles.AnyAsync(v => v.VIN == vehicleVm.VIN))
        {
            ModelState.AddModelError("VIN", "This VIN is already registered for another vehicle.");
            await PopulateDriversDropDownList(vehicleVm.AssignedDriverId);
            return View(vehicleVm);
        }

        try
        {
            string? imagePath = null;
            if (vehicleVm.VehicleImage != null && vehicleVm.VehicleImage.Length > 0)
            {
                imagePath = await _vehicleService.SaveVehicleImageAsync(vehicleVm.VehicleImage);
            }

            var vehicle = new Vehicle
            {
                VIN = vehicleVm.VIN,
                LicensePlate = vehicleVm.LicensePlate,
                Make = vehicleVm.Make,
                Model = vehicleVm.Model,
                Year = vehicleVm.Year,
                PurchasePrice = vehicleVm.PurchasePrice,
                Status = vehicleVm.Status,
                Mileage = vehicleVm.Mileage,
                VehicleImageURL = imagePath,
                AssignedDriverId = string.IsNullOrEmpty(vehicleVm.AssignedDriverId) ? null : vehicleVm.AssignedDriverId
            };

            _context.Add(vehicle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehicle added successfully to the fleet!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Database error: " + ex.Message);
            await PopulateDriversDropDownList(vehicleVm.AssignedDriverId);
            return View(vehicleVm);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        var model = new VehicleEditViewModel
        {
            Id = vehicle.Id,
            VIN = vehicle.VIN,
            LicensePlate = vehicle.LicensePlate,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            PurchasePrice = vehicle.PurchasePrice,
            Status = vehicle.Status,
            Mileage = vehicle.Mileage,
            ExistingImageUrl = vehicle.VehicleImageURL,
            AssignedDriverId = vehicle.AssignedDriverId
        };

        await PopulateDriversDropDownList(model.AssignedDriverId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleEditViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (await _context.Vehicles.AnyAsync(v => v.VIN == model.VIN && v.Id != id))
            {
                ModelState.AddModelError("VIN", "This VIN is already registered for another vehicle.");
                await PopulateDriversDropDownList(model.AssignedDriverId);
                return View(model);
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            if (model.VehicleImage != null && model.VehicleImage.Length > 0)
            {
                _vehicleService.DeleteVehicleImage(vehicle.VehicleImageURL);
                vehicle.VehicleImageURL = await _vehicleService.SaveVehicleImageAsync(model.VehicleImage);
            }

            vehicle.VIN = model.VIN;
            vehicle.LicensePlate = model.LicensePlate;
            vehicle.Make = model.Make;
            vehicle.Model = model.Model;
            vehicle.Year = model.Year;
            vehicle.PurchasePrice = model.PurchasePrice;
            vehicle.Status = model.Status;
            vehicle.Mileage = model.Mileage;
            vehicle.AssignedDriverId = string.IsNullOrEmpty(model.AssignedDriverId) ? null : model.AssignedDriverId;

            _context.Update(vehicle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehicle updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDriversDropDownList(model.AssignedDriverId);
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _vehicleService.DeleteVehicleImage(vehicle.VehicleImageURL);
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vehicle deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDriversDropDownList(object? selectedDriver = null)
    {
        var drivers = await _context.Users.ToListAsync();
        ViewBag.Drivers = new SelectList(drivers, "Id", "UserName", selectedDriver);
    }


    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.AssignedDriver)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }
}