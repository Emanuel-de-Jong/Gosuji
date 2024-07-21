using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Pages
{
    public partial class Settings : ComponentBase
    {
        [SupplyParameterFromForm]
        private VMUpdatePrivacy privacyInput { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private string? privacyMessage;
        private bool isPrivacyMessageError;

        public async Task UpdatePrivacy()
        {
            //string? error = await userService.UpdatePrivacy(privacyInput);

            //if (error == null)
            //{
            //    isPrivacyMessageError = false;
            //    privacyMessage = "Privacy changes pending. A confirmation email has been sent. Please use the link in the email to confirm your changes.";
            //    privacyInput = new();
            //}
            //else
            //{
            //    isPrivacyMessageError = true;
            //    privacyMessage = error;
            //}
        }
    }
}
