using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Gosuji.Client.Attributes
{
    public class NotEqualAttribute : ValidationAttribute
    {
        public string ComparisonProperty { get; }

        public NotEqualAttribute(string comparisonProperty)
        {
            ComparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(ComparisonProperty);

            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }

            object? comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (value != null && value.Equals(comparisonValue))
            {
                string errorMessage = string.Format("{0} should not be equal to {1}", validationContext.DisplayName, ComparisonProperty);
                if (!string.IsNullOrEmpty(ErrorMessageString))
                {
                    errorMessage = FormatErrorMessage(validationContext.DisplayName);
                }

                return new ValidationResult(ErrorMessage ?? errorMessage);
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, ComparisonProperty);
        }
    }
}
