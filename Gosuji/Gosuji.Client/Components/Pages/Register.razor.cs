using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages
{
    public partial class Register : ComponentBase
    {
        [SupplyParameterFromForm]
        private VMRegister input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }

        private string? message;
        private bool isMessageError;
        string? backupCode;

        public async Task RegisterUser()
        {
            backupCode = await userService.Register(input);

            if (backupCode != null)
            {
                isMessageError = false;
                message = "Registration successful. A confirmation email has been sent. Please use the link in the email to confirm your account.";
                input = new();
            }
            else
            {
                isMessageError = true;
                message = "There is already an account with this email.";
            }
        }

        public async Task BackupCodeToClipboard()
        {
            js.InvokeVoidAsync("navigator.clipboard.writeText", backupCode);
        }
    }
}
