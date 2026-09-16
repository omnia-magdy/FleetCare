using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public enum VehicleStatus
{
    Active,
    InService,
    Decommissioned
}

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [StringLength(17, MinimumLength = 17)]
    public string VIN { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public decimal PurchasePrice { get; set; }

    public VehicleStatus Status { get; set; } = VehicleStatus.Active;

    public int Mileage { get; set; }

    public string? VehicleImageURL { get; set; }

    public string? AssignedDriverId { get; set; }
    public ApplicationUser? AssignedDriver { get; set; }

    public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
}