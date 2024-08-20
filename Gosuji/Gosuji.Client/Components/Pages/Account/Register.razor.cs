using Gosuji.Client.Components.Shared;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class Register : CustomPage
    {
        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

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
            VMRegister vmRegister = new()
            {
                UserName = input.UserName,
                Email = input.Email,
                Password = input.Password,
                IsGetChangelogEmail = input.IsGetChangelogEmail
            };

            APIResponse<string> response = await userService.Register(vmRegister);
            if (!statusMessage.HandleAPIResponse(response))
            {
                input = new();
                statusMessage.SetMessage("Registration successful. A confirmation email has been sent. Please use the link in the email to confirm your account.");
                backupCode = response.Data;
            }
        }

        public async Task BackupCodeToClipboard()
        {
            await js.InvokeVoidAsync("navigator.clipboard.writeText", backupCode);
        }

        private sealed class InputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(2, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(30, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
                ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Email { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(6, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
                ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Password { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare("Password", ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmPassword { get; set; }

            [Range(typeof(bool), "true", "true", ErrorMessageResourceName = "CheckboxCheckedError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public bool AcceptToS { get; set; } = false;

            public bool IsGetChangelogEmail { get; set; } = false;
        }
    }
}
