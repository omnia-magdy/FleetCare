using FleetCarePro.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FleetCarePro.Services;

public class VehicleService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public VehicleService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveVehicleImageAsync(IFormFile imageFile)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "vehicles");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileExtension = Path.GetExtension(imageFile.FileName);
        string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return $"/uploads/vehicles/{uniqueFileName}";
    }

    public void DeleteVehicleImage(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}