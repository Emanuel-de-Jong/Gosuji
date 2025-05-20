using Gosuji.Client.Data;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace Gosuji.Client.Components.Pages
{
    public partial class Home : CustomPage
    {
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private DataAPI dataAPI { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private IStringLocalizer<General> tl { get; set; }

        private Changelog[]? changelogs;

        protected override async Task OnInitializedAsync()
        {
            if (G.IsFirstPage)
            {
                G.IsFirstPage = false;

                if (G.IsStandalone)
                {
                    navigationManager.NavigateTo("learn/trainer");
                    return;
                }
            }

            await base.OnInitializedAsync();

            changelogs = await dataAPI.GetChangelogs();
            if (changelogs != null)
            {
                changelogs = changelogs.OrderByDescending(c => c.Date).ToArray();
            }
        }
    }
}
