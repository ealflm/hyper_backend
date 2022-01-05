using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.Validator
{
    public class NotAllowedEmptyStringAttribute : ValidationAttribute
    {
        readonly string _value;

        public string Value { get => _value; }

        public override bool IsValid(object value)
        {
            var input = (string)value;
            if (input != null && input.Trim() == "")
                return false;
            return true;
        }
    }
}