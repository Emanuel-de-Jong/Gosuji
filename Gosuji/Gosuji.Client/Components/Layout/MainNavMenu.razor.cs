using Gosuji.Client.Data;
using Gosuji.Client.Services;
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
        [Inject]
        private IDataService dataService { get; set; }

        private ClaimsPrincipal claimsPrincipal;
        private string currentLanguageSrc;
        private SettingConfig? settingConfig;
        private Dictionary<string, Language>? languages;

        protected override async Task OnInitializedAsync()
        {
            claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                settingConfig = await dataService.GetSettingConfig(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                languages = await dataService.GetLanguages();

                CultureInfo culture = new(settingConfig.Language.Short);
                CultureInfo.CurrentCulture = culture;
            }

            currentLanguageSrc = BASE_LANGUAGE_SRC + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".svg";
        }

        private async Task ChangeLanguage(string language)
        {
            CultureInfo culture = new(language);
            CultureInfo.CurrentCulture = culture;
            currentLanguageSrc = BASE_LANGUAGE_SRC + language + ".svg";

            settingConfig.LanguageId = languages[language].Id;
            await dataService.PutSettingConfig(settingConfig);
        }
    }
}
