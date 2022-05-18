using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.Business.Validation
{
    public class AllowNullOrEmptyAndCheckValidPhone : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
                {
                    return ValidationResult.Success;
                }

                const string regexPhoneNumber = @"^0[0-9]{9,12}$";
                var compare = Regex.IsMatch(value.ToString().Trim(), regexPhoneNumber);

                if (!compare)
                {
                    return new ValidationResult("" + validationContext.DisplayName + " is invalid");
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
