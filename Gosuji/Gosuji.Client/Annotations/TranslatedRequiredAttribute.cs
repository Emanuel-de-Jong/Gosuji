using Gosuji.Client.Services;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Annotations
{
    public class TranslatedRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                ITranslateService translateService = (ITranslateService)validationContext.GetService(typeof(ITranslateService));
                string error = translateService.Get("aadc581d-0cac-4581-8244-358c4314f324", "Validate_Error_Required");
                return new ValidationResult(error);
            }

            return ValidationResult.Success;
        }
    }
}
