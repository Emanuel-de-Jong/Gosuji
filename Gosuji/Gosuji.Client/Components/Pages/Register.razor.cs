using Gosuji.Client.Components.Shared;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages
{
    public partial class Register : CustomPage
    {
        [SupplyParameterFromForm]
        private VMRegister input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }

        private CStatusMessage statusMessage;

        private string? message;
        private bool isMessageError;
        string? backupCode;

        public async Task RegisterUser()
        {
            APIResponse<string> response = await userService.Register(input);

            if (response.IsSuccess)
            {
                input = new();
                statusMessage.SetMessage("Registration successful. A confirmation email has been sent. Please use the link in the email to confirm your account.");
                backupCode = response.Data;
                return;
            }
            else
            {
                statusMessage.HandleAPIResponse(response);
            }
        }

        public async Task BackupCodeToClipboard()
        {
            await js.InvokeVoidAsync("navigator.clipboard.writeText", backupCode);
        }
    }
}
