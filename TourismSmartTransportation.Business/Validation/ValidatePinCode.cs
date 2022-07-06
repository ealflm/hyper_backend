using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.Business.Validation
{
    public class ValidatedPinCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    return ValidationResult.Success;
                }

                const string regexPinCode = @"^[0-9]{6}$";
                var compare = Regex.IsMatch(value.ToString().Trim(), regexPinCode);

                if (!compare)
                {
                    return new ValidationResult("" + validationContext.DisplayName + " is invalid. It should only include 6 numbers.");
                }
            }
            catch (Exception)
            {
                return new ValidationResult("" + validationContext.DisplayName + " is invalid");
            }
            return ValidationResult.Success;
        }
    }
}
