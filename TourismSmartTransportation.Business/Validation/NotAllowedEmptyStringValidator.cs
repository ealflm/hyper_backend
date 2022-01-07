using System.ComponentModel.DataAnnotations;
namespace TourismSmartTransportation.Business.Validation
{
    public class NotAllowedEmptyStringValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null && string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }

            return ValidationResult.Success;
        }
    }
}
