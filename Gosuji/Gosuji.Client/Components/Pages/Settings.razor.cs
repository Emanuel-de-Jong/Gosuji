using Gosuji.Client.Data;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages
{
    public partial class Settings : ComponentBase
    {
        [SupplyParameterFromForm]
        private PrivacyInputModel privacyInput { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private string? privacyMessage;
        private bool isPrivacyMessageError;

        public async Task UpdatePrivacy()
        {
            VMUpdatePrivacy vmUpdatePrivacy = new();
            if (privacyInput.UserName != "Current")
            {
                vmUpdatePrivacy.UserName = privacyInput.UserName;
            }
            if (privacyInput.Email != "Current")
            {
                vmUpdatePrivacy.Email = privacyInput.Email;
            }
            if (!string.IsNullOrEmpty(privacyInput.NewPassword))
            {
                vmUpdatePrivacy.Password = privacyInput.NewPassword;
            }

            bool result = await userService.UpdatePrivacy(vmUpdatePrivacy);

            if (result)
            {
                isPrivacyMessageError = false;
                privacyMessage = "Privacy changes pending. A confirmation email has been sent. Please use the link in the email to confirm your changes.";
                privacyInput = new();
            }
            else
            {
                isPrivacyMessageError = true;
                privacyMessage = "There was nothing to change.";
            }
        }

        private sealed class PrivacyInputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(2, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(30, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?!\s*$).+$", ErrorMessageResourceName = "JustSpacesError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
                ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Email { get; set; }

            [MinLength(6, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
                ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string NewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare("NewPassword", ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmNewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string CurrentPassword { get; set; }
        }
    }
}
