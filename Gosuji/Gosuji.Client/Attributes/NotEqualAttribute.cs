using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Gosuji.Client.Attributes
{
    public class NotEqualAttribute : ValidationAttribute
    {
        private readonly string comparisonProperty;

        public NotEqualAttribute(string comparisonProperty)
        {
            this.comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(comparisonProperty);

            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            object? comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (value != null && value.Equals(comparisonValue))
            {
                string errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(ErrorMessage ?? errorMessage);
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, comparisonProperty);
        }
    }
}
