using Gosuji.Client.Components.Shared;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class ChangePassword : CustomPage
    {
        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private CStatusMessage statusMessage;

        private async Task TryChangePassword()
        {
            VMChangePassword vmChangePassword = new()
            {
                NewPassword = input.NewPassword
            };

            APIResponse response = await userService.ChangePassword(vmChangePassword);
            if (response.IsSuccess)
            {
                statusMessage.SetMessage("Your password has been updated. You can now log in with the new password.");
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
            [MinLength(6, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
                ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string NewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare(nameof(NewPassword), ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmNewPassword { get; set; }
        }
    }
}
