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
                if (Regex.IsMatch(value.ToString(), @"^0\.[0-9]{1,2}$|^[0-1]$"))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Giá trị khuyến mãi chỉ nằm trong đoạn [0-1] và tối đa 2 chữ số sau dấu chấm thập phân");
            }

            return ValidationResult.Success;
        }
    }
}
