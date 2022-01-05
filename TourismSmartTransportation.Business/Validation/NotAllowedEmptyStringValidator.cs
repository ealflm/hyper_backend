using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Validate
{
    public class NotAllowedEmptyStringValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
                
            if ( value !=null  &&string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
                     
            return ValidationResult.Success;
        }
    }
}
