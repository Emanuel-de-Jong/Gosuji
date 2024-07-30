using Gosuji.Client.Data;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Contact : CustomPage
    {
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private DataService dataService { get; set; }

        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

        private bool isNotLoggedIn = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            G.StatusMessage.SetMessage("Contact");

            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                isNotLoggedIn = false;
            }
        }

        private async Task SendFeedback()
        {
            if (isNotLoggedIn)
            {
                return;
            }

            Feedback feedback = new()
            {
                Subject = input.Subject,
                Message = input.Message,
                FeedbackType = input.FeedbackType
            };

            await dataService.PostFeedback(feedback);

            input = new();
        }

        private sealed class InputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(250, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(3, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Subject { get; set; }

            [MaxLength(1000, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string? Message { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [EnumDataType(typeof(EFeedbackType))]
            public EFeedbackType FeedbackType { get; set; } = EFeedbackType.Support;
        }
    }
}
