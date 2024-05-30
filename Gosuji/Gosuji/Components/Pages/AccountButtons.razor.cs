using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Components.Pages
{
    public partial class AccountButtons : ComponentBase
    {
        [Inject]
        private IHttpContextAccessor? httpContextAccessor { get; set; }
        [Inject]
        private IAntiforgery? antiforgery { get; set; }

        private string? antiforgeryToken;

        protected override async Task OnInitializedAsync()
        {
            antiforgeryToken = antiforgery.GetAndStoreTokens(httpContextAccessor.HttpContext).RequestToken;
        }
    }
}
