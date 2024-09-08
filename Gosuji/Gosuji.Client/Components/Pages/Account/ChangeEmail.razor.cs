using Gosuji.Client.Components.Shared;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class ChangeEmail : CustomPage
    {
        [SupplyParameterFromForm]
        private UserInputModel userInput { get; set; } = new();
        [SupplyParameterFromForm]
        private GuestInputModel guestInput { get; set; } = new();

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private UserAPI userAPI { get; set; }

        private CStatusMessage statusMessage;
        private bool? isLoggedIn;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                isLoggedIn = true;
            }
            else
            {
                isLoggedIn = false;
            }
        }

        private async Task TryChangeEmail()
        {
            VMChangeEmail vmChangeEmail = null;
            if (isLoggedIn == true)
            {
                vmChangeEmail = new()
                {
                    BackupCode = userInput.BackupCode,
                    NewEmail = userInput.NewEmail
                };
            }
            else
            {
                vmChangeEmail = new()
                {
                    UserName = guestInput.UserName,
                    Password = guestInput.Password,
                    BackupCode = guestInput.BackupCode,
                    NewEmail = guestInput.NewEmail
                };
            }

            APIResponse response = await userAPI.ChangeEmail(vmChangeEmail);
            if (response.IsSuccess)
            {
                statusMessage.SetMessage("A confirmation email has been sent to your new email. Please use the link in the email to confirm the change.");
                guestInput = new();
                userInput = new();
            }
            else
            {
                statusMessage.HandleAPIResponse(response);
            }
        }

        private class UserInputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(32, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(32, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string BackupCode { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
                ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string NewEmail { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare(nameof(NewEmail), ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmNewEmail { get; set; }
        }

        private sealed class GuestInputModel : UserInputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Password { get; set; }
        }
    }
}
