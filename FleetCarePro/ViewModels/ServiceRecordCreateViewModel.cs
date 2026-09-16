using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FleetCarePro.ViewModels;

public class ServiceRecordCreateViewModel
{
    [Required(ErrorMessage = "Vehicle is required.")]
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Service center is required.")]
    public int ServiceCenterId { get; set; }

    [Required(ErrorMessage = "Service date is required.")]
    [DataType(DataType.Date)]
    public DateTime ServiceDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Current mileage is required.")]
    [Range(0, 2000000)]
    public int CurrentMileage { get; set; }

    public string? Notes { get; set; }

    public IFormFile? InvoiceDocument { get; set; }

    public List<ServiceLineItemViewModel> LineItems { get; set; } = new();
}