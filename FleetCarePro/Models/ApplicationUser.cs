using Microsoft.AspNetCore.Identity;

namespace FleetCarePro.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;

    
    public ICollection<Vehicle> AssignedVehicles { get; set; } = new List<Vehicle>();
    public ICollection<ServiceRecord> CreatedServiceRecords { get; set; } = new List<ServiceRecord>();
}