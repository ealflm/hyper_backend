using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.Validation
{
    public class ValidValueMinMax : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public ValidValueMinMax(string comparisonProperty)
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
                    ErrorMessage = "Value is not a valid";
                }

                var currentValue = (Decimal)value;

                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (property == null)
                    throw new ArgumentException("Property with this name not found");

                if (property.GetValue(validationContext.ObjectInstance) == null)
                    return ValidationResult.Success;

                var comparisonValue = (Decimal)property.GetValue(validationContext.ObjectInstance);

                if (currentValue > comparisonValue)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}