using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public enum ServiceStatus
{
    Pending,
    Approved,
    Completed,
    Cancelled
}

public class ServiceRecord
{
    public int Id { get; set; }

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public int ServiceCenterId { get; set; }
    public ServiceCenter ServiceCenter { get; set; } = null!;

    public DateTime ServiceDate { get; set; }

    public int CurrentMileage { get; set; }

    public decimal TotalCost { get; set; }

    public string? InvoiceDocumentPath { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

    [Required]
    public string CreatedByUserId { get; set; } = string.Empty;
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public ICollection<ServiceLineItem> ServiceLineItems { get; set; } = new List<ServiceLineItem>();
}