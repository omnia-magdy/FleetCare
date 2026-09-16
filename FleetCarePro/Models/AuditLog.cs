using System.ComponentModel.DataAnnotations;

namespace FleetCarePro.Models;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string ActionName { get; set; } = string.Empty;

    [Required]
    public string ControllerName { get; set; } = string.Empty;

    public string? IPAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? Details { get; set; }
}