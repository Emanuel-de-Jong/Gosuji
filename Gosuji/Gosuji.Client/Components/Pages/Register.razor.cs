using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Components.Pages
{
    public partial class Register : ComponentBase
    {
        [SupplyParameterFromForm]
        private VMRegister input { get; set; } = new();

        [Inject]
        private UserService userService { get; set; }

        private string? message;
        private bool isMessageError;

        public async Task RegisterUser()
        {
            bool result = await userService.Register(input);

            if (result)
            {
                isMessageError = false;
                message = "Registration successful. A confirmation email has been sent. Please use the link in the email to confirm your account.";
                input = new();
            }
            else
            {
                isMessageError = true;
                message = "An error occurred while registering.";
            }
        }
    }
}
