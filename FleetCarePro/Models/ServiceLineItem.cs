using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public class ServiceLineItem
{
    public int Id { get; set; }

    public int ServiceRecordId { get; set; }
    public ServiceRecord ServiceRecord { get; set; } = null!;

    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    public decimal Cost { get; set; }
}