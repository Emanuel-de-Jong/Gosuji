using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
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
        [Inject]
        private IJSRuntime js { get; set; }

        private ClaimsPrincipal claimsPrincipal;
        private string currentLanguageSrc;
        private SettingConfig? settingConfig;
        private Dictionary<string, Language>? languages;
        private Language? currentLanguage;

        protected override async Task OnInitializedAsync()
        {
            claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                settingConfig = await dataService.GetSettingConfig(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                languages = await dataService.GetLanguages();
                currentLanguage = languages.Where(kv => kv.Value.Id == settingConfig.LanguageId).FirstOrDefault().Value;

                CultureInfo culture = new(currentLanguage.Short);
                CultureInfo.CurrentCulture = culture;
            }

            currentLanguageSrc = BASE_LANGUAGE_SRC + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".svg";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (settingConfig != null)
                {
                    await SetTheme(settingConfig.IsDarkMode);
                }
            }
        }

        private async Task ChangeLanguage(string language)
        {
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == language)
            {
                return;
            }

            CultureInfo culture = new(language);
            CultureInfo.CurrentCulture = culture;
            currentLanguageSrc = BASE_LANGUAGE_SRC + language + ".svg";

            if (settingConfig != null)
            {
                currentLanguage = languages[language];
                settingConfig.LanguageId = languages[language].Id;
                await dataService.PutSettingConfig(settingConfig);
            }
        }

        private async Task ChangeVolume(ChangeEventArgs e)
        {
            int volume = Convert.ToInt32(e.Value);

            if (settingConfig != null)
            {
                if (settingConfig.Volume == volume)
                {
                    return;
                }

                settingConfig.Volume = volume;
                await dataService.PutSettingConfig(settingConfig);
            }
        }

        private async Task ChangeIsDarkMode(ChangeEventArgs e)
        {
            bool isDarkMode = (bool)e.Value;
            await SetTheme(isDarkMode);

            if (settingConfig != null)
            {
                if (settingConfig.IsDarkMode == isDarkMode)
                {
                    return;
                }

                settingConfig.IsDarkMode = isDarkMode;
                await dataService.PutSettingConfig(settingConfig);
            }
        }

        private async Task SetTheme(bool isDarkTheme)
        {
            int theme = await js.InvokeAsync<int>("theme.nameToNum", isDarkTheme ? "DARK" : "LIGHT");
            await js.InvokeVoidAsync("theme.set", theme);
        }
    }
}
