using Gosuji.Client.Attributes;
using Gosuji.Client.Data;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Gosuji.Client.Services.User;
using Gosuji.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace Gosuji.Client.Components.Pages
{
    public partial class Settings : ComponentBase
    {
        [SupplyParameterFromForm]
        private PrivacyInputModel? privacyInput { get; set; }

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private UserService userService { get; set; }

        private string? currentUserName;
        private string? currentEmail;

        private string? privacyMessage;
        private bool isPrivacyMessageError;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo($"login?{G.ReturnUriName}={Uri.EscapeDataString(navigationManager.Uri)}");
                return;
            }

            currentUserName = claimsPrincipal.Identity.Name;
            currentEmail = claimsPrincipal.FindFirst(ClaimTypes.Email).Value;

            privacyInput = new PrivacyInputModel
            {
                UserName = currentUserName,
                Email = currentEmail
            };
        }

        public async Task UpdatePrivacy()
        {
            VMUpdatePrivacy vmUpdatePrivacy = new()
            {
                CurrentPassword = privacyInput.CurrentPassword
            };

            if (privacyInput.UserName != currentUserName)
            {
                vmUpdatePrivacy.UserName = privacyInput.UserName;
            }
            if (privacyInput.Email != currentEmail)
            {
                vmUpdatePrivacy.Email = privacyInput.Email;
            }
            if (!string.IsNullOrEmpty(privacyInput.NewPassword))
            {
                vmUpdatePrivacy.NewPassword = privacyInput.NewPassword;
            }

            if (vmUpdatePrivacy.UserName == null && vmUpdatePrivacy.Email == null && vmUpdatePrivacy.NewPassword == null)
            {
                isPrivacyMessageError = true;
                privacyMessage = "There was nothing to change.";
                return;
            }

            bool result = await userService.UpdatePrivacy(vmUpdatePrivacy);

            if (result)
            {
                isPrivacyMessageError = false;
                privacyMessage = "Privacy changes pending. A confirmation email has been sent to your current email address. Please use the link in the email to confirm your changes.";
                privacyInput = new();
            }
            else
            {
                isPrivacyMessageError = true;
                privacyMessage = "The provided password is incorrect.";
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
            [NotEqual(nameof(CurrentPassword), ErrorMessageResourceName = "NewPasswordEqualError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
                ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string NewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare(nameof(NewPassword), ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmNewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string CurrentPassword { get; set; }
        }
    }
}
