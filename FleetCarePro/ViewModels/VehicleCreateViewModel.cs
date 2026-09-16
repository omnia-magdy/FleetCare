using System.ComponentModel.DataAnnotations;
using FleetCarePro.Attributes;
using FleetCarePro.Models;
using Microsoft.AspNetCore.Http;

namespace FleetCarePro.ViewModels;

public class VehicleCreateViewModel
{
    [Required(ErrorMessage = "VIN is required.")]
    [ValidVIN]
    public string VIN { get; set; } = string.Empty;

    [Required(ErrorMessage = "License plate is required.")]
    [Display(Name = "License Plate")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Make is required.")]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required.")]
    public string Model { get; set; } = string.Empty;

    [Range(1990, 2030, ErrorMessage = "Please enter a valid year between 1990 and 2030.")]
    public int Year { get; set; } = DateTime.Now.Year;

    [Range(0, 10000000, ErrorMessage = "Purchase price must be a positive value.")]
    [Display(Name = "Purchase Price")]
    public decimal PurchasePrice { get; set; }

    public VehicleStatus Status { get; set; } = VehicleStatus.Active;

    [Range(0, 2000000, ErrorMessage = "Mileage must be a positive value.")]
    public int Mileage { get; set; }

    [Display(Name = "Vehicle Image")]
    public IFormFile? VehicleImage { get; set; }

    [Display(Name = "Assigned Driver")]
    public string? AssignedDriverId { get; set; }
}

public class VehicleEditViewModel : VehicleCreateViewModel
{
    public int Id { get; set; }
    public string? ExistingImageUrl { get; set; }
}