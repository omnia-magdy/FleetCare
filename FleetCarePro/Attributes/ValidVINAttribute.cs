using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FleetCarePro.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidVINAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string vin = value.ToString()!.Trim().ToUpper();

        
        if (vin.Length != 17)
        {
            return new ValidationResult("VIN must be exactly 17 characters long.");
        }

       
        if (Regex.IsMatch(vin, @"[IOQ]"))
        {
            return new ValidationResult("VIN cannot contain the letters I, O, or Q.");
        }

      
        if (!Regex.IsMatch(vin, @"^[A-HJ-NPR-Z0-9]{17}$"))
        {
            return new ValidationResult("VIN contains invalid characters.");
        }

        return ValidationResult.Success;
    }
}