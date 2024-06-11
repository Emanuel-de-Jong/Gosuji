using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Contact : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private IDataService dataService { get; set; }

        private string? userId;
        private bool isNotLoggedIn => userId == null;

        private string? subject;
        private string? message;
        private EFeedbackType feedbackType = EFeedbackType.Support;

        protected override async Task OnInitializedAsync()
        {
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
                Subject = subject,
                Message = message,
                FeedbackType = feedbackType
            };

            await dataService.PostFeedback(feedback);
        }
    }
}
