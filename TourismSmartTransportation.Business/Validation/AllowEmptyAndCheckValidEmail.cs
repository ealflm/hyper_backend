using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace TourismSmartTransportation.Business.Validation
{
    public class AllowEmptyAndChekcValidEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return ValidationResult.Success;
                }

                MailAddress mail = new MailAddress(value.ToString());

            }
            catch (Exception)
            {
                return new ValidationResult("" + validationContext.DisplayName + " is invalid");
            }
            return ValidationResult.Success;
        }
    }
}
