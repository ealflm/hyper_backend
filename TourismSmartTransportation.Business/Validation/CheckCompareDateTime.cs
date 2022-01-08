using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.Validation
{
    public class ValidStartDateTimeAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public ValidStartDateTimeAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                ErrorMessage = ErrorMessageString;
                if (ErrorMessage == null || ErrorMessage.Trim() == "")
                {
                    ErrorMessage = "Time is not valid";
                }

                var currentValue = (DateTime)value;

                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (property == null)
                    throw new ArgumentException("Property with this name not found");

                if (property.GetValue(validationContext.ObjectInstance) == null)
                    return ValidationResult.Success;

                var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

                if (currentValue > comparisonValue)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}