using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Contact : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private IDataService dataService { get; set; }
        [Inject]
        private ITranslateService translateService { get; set; }

        [SupplyParameterFromForm]
        private InputModel input { get; set; } = new();

        private string? userId;
        private bool isNotLoggedIn => userId == null;

        protected override async Task OnInitializedAsync()
        {
            await translateService.Init();

            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
                UserId = userId,
                Subject = input.Subject,
                Message = input.Message,
                FeedbackType = input.FeedbackType
            };

            await dataService.PostFeedback(feedback);

            input = new();
        }

        private sealed class InputModel
        {
            [Required]
            public string Subject { get; set; }
            public string? Message { get; set; }
            [Required]
            [EnumDataType(typeof(EFeedbackType))]
            public EFeedbackType FeedbackType { get; set; } = EFeedbackType.Support;
        }
    }
}
