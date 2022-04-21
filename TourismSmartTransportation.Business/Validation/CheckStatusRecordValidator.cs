using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.Business.Validation
{
    public class CheckStatusRecordValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                if (value.ToString().Equals("1") || value.ToString().Equals("2"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Status is not correct! It is only  1 or 2");
            }

            return ValidationResult.Success;
        }
    }
}
