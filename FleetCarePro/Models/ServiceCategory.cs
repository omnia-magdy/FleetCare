using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public class ServiceCategory
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int RecommendedIntervalMonths { get; set; }

    
    public ICollection<VendorService> VendorServices { get; set; } = new List<VendorService>();
    public ICollection<ServiceLineItem> ServiceLineItems { get; set; } = new List<ServiceLineItem>();
}