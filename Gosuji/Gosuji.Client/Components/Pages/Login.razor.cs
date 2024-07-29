using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [SupplyParameterFromQuery(Name = G.ReturnUriName)]
        public string? ReturnUri { get; set; }
        [SupplyParameterFromForm]
        private VMLogin input { get; set; } = new();

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private IStringLocalizer<General> tl { get; set; }
        [Inject]
        private IStringLocalizer<APIResponse> tlApi { get; set; }
        [Inject]
        private UserService userService { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo("profile");
            }
        }

        public async Task LoginUser()
        {
            APIResponse<string> apiResponse = await userService.Login(input);
            if (apiResponse.IsSuccess)
            {
                navigationManager.NavigateTo(string.IsNullOrEmpty(ReturnUri) ? "/" : ReturnUri);
            }
            else
            {
                errorMessage = tlApi[apiResponse.Message];
            }
        }
    }
}
