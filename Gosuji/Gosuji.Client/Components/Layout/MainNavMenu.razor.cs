using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using System.Security.Claims;

namespace Gosuji.Client.Components.Layout
{
    public partial class MainNavMenu : ComponentBase
    {
        private static readonly string BASE_LANGUAGE_SRC = "resources/imgs/flags/";

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        private ClaimsPrincipal claimsPrincipal;
        private string currentLanguageSrc;

        protected override async Task OnInitializedAsync()
        {
            claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            currentLanguageSrc = BASE_LANGUAGE_SRC + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".svg";
        }

        private void ChangeLanguage(string language)
        {
            CultureInfo culture = new(language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            currentLanguageSrc = BASE_LANGUAGE_SRC + language + ".svg";
        }
    }
}
