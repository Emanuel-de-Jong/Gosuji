using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace Gosuji.Client.Attributes
{
    public class NotEqualAttribute : ValidationAttribute
    {
        public string ComparisonProperty { get; }

        public NotEqualAttribute(string comparisonProperty)
        {
            if (string.IsNullOrWhiteSpace(comparisonProperty))
            {
                throw new ArgumentException("Comparison property name cannot be null or whitespace", nameof(comparisonProperty));
            }

            ComparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            PropertyInfo? comparisonPropertyInfo = validationContext.ObjectType.GetProperty(ComparisonProperty);

            if (comparisonPropertyInfo == null)
            {
                throw new ArgumentException($"Property '{ComparisonProperty}' not found on object of type '{validationContext.ObjectType.FullName}'");
            }

            object? comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value != null && value.Equals(comparisonValue))
            {
                string errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            string errorMessageString = ErrorMessageString ?? "{0} should not be equal to {1}";

            if (ErrorMessageResourceType != null && !string.IsNullOrEmpty(ErrorMessageResourceName))
            {
                ResourceManager resourceManager = new(ErrorMessageResourceType);
                string? resourceErrorMessageString = resourceManager.GetString(ErrorMessageResourceName);
                if (!string.IsNullOrEmpty(resourceErrorMessageString))
                {
                    errorMessageString = resourceErrorMessageString;
                }
            }

            return string.Format(errorMessageString, name, ComparisonProperty);
        }
    }
}
