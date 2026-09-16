using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public class ServiceCenter
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Relationships
    public ICollection<VendorService> VendorServices { get; set; } = new List<VendorService>();
    public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
}