using Gosuji.Client.Components.Shared;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class ForgotPassword : CustomPage
    {
        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private CStatusMessage statusMessage;

        private async Task TryForgotPassword()
        {
            VMForgotPassword vmForgotPassword = new()
            {
                Email = input.Email
            };

            APIResponse response = await userService.ForgotPassword(vmForgotPassword);
            if (response.IsSuccess)
            {
                statusMessage.SetMessage("An email has been sent. Please use the link in the email to set a new password.");
                input = new();
            }
            else
            {
                statusMessage.HandleAPIResponse(response);
            }
        }

        private sealed class InputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
                ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Email { get; set; }
        }
    }
}
