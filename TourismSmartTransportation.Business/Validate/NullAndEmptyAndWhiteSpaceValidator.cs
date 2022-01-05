using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Validate
{
    public class NullAndEmptyAndWhiteSpaceValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string str = value.ToString();
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                {
                    return new ValidationResult("" + validationContext.DisplayName + " is required");
                }
            }            
            return ValidationResult.Success;
        }
    }
}
