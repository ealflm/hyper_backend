using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.Business.Validation
{
    public class DiscountValueValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                if (Regex.IsMatch(value.ToString(), @"^0\.[0-9]{1}$|^[0-1]$"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("dicount value is not correct! It is only between 0 and 1");
            }

            return ValidationResult.Success;
        }
    }
}
