using Gosuji.Client.Attributes;
using Gosuji.Client.Components.Shared;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages
{
    public partial class ChangeEmail : CustomPage
    {
        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private CStatusMessage statusMessage;

        private async Task TryChangeEmail()
        {
            VMChangeEmail vmChangeEmail = new()
            {
                UserName = input.UserName,
                Password = input.Password,
                BackupCode = input.BackupCode,
                NewEmail = input.NewEmail
            };

            APIResponse response = await userService.ChangeEmail(vmChangeEmail);
            if (response.IsSuccess)
            {
                statusMessage.SetMessage("A confirmation email has been sent to your new email. Please use the link in the email to confirm the change.");
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
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Password { get; set; }

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
    }
}
