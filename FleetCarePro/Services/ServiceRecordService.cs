using FleetCarePro.Data;
using FleetCarePro.Models;
using FleetCarePro.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FleetCarePro.Services;

public class ServiceRecordService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ServiceRecordService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<(bool IsValid, string? FilePath, string? ErrorMessage)> SaveInvoiceDocumentAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return (true, null, null);

        const long maxSizeBytes = 5 * 1024 * 1024;
        if (file.Length > maxSizeBytes)
        {
            return (false, null, "File size must not exceed 5 MB.");
        }

        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return (false, null, "Only PDF, JPG, and PNG files are allowed.");
        }

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "invoices");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (true, $"/uploads/invoices/{uniqueFileName}", null);
    }


    public async Task<bool> CreateServiceRecordAsync(ServiceRecordCreateViewModel model, string createdByUserId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var fileUploadResult = await SaveInvoiceDocumentAsync(model.InvoiceDocument);
            if (!fileUploadResult.IsValid)
            {
                throw new InvalidOperationException(fileUploadResult.ErrorMessage);
            }

            decimal totalCost = model.LineItems.Sum(item => item.Cost);

            var serviceRecord = new ServiceRecord
            {
                VehicleId = model.VehicleId,
                ServiceCenterId = model.ServiceCenterId,
                ServiceDate = model.ServiceDate,
                CurrentMileage = model.CurrentMileage,
                TotalCost = totalCost,
                InvoiceDocumentPath = fileUploadResult.FilePath,
                Notes = model.Notes,
                Status = ServiceStatus.Pending,
                CreatedByUserId = createdByUserId
            };

            _context.ServiceRecords.Add(serviceRecord);
            await _context.SaveChangesAsync();

            foreach (var item in model.LineItems)
            {
                var lineItem = new ServiceLineItem
                {
                    ServiceRecordId = serviceRecord.Id,
                    ServiceCategoryId = item.ServiceCategoryId,
                    Description = item.Description,
                    Cost = item.Cost
                };
                _context.ServiceLineItems.Add(lineItem);
            }

            var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
            if (vehicle != null && model.CurrentMileage > vehicle.Mileage)
            {
                vehicle.Mileage = model.CurrentMileage;
                _context.Vehicles.Update(vehicle);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

