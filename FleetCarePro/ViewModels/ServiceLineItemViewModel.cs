using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.ViewModels;

public class ServiceLineItemViewModel
{
    [Required(ErrorMessage = "Service category is required.")]
    public int ServiceCategoryId { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cost is required.")]
    [Range(0.01, 100000, ErrorMessage = "Cost must be a positive value.")]
    public decimal Cost { get; set; }
}