using Gosuji.Client.Data;
using Gosuji.Client.Models;
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
        private DataService dataService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }

        private string currentLanguageSrc;
        private SettingConfig? settingConfig;
        private Dictionary<string, Language>? languages;
        private Language? currentLanguage;
        private bool isDarkMode = true;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                settingConfig = await dataService.GetSettingConfig();
                languages = await dataService.GetLanguages();
                currentLanguage = languages.Where(kv => kv.Value.Id == settingConfig.LanguageId).FirstOrDefault().Value;

                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != currentLanguage.Short)
                {
                    CultureInfo culture = new(currentLanguage.Short);

                    await js.InvokeVoidAsync("blazorCulture.set", culture!.Name);

                    string uri = new Uri(navigationManager.Uri)
                        .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                    string cultureEscaped = Uri.EscapeDataString(culture.Name);
                    string uriEscaped = Uri.EscapeDataString(uri);

                    navigationManager.NavigateTo(
                        $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
                        forceLoad: true);
                }
            }
            else
            {
                string? theme = await js.InvokeAsync<string?>("utils.getCookie", "theme");
                if (theme != null && int.TryParse(theme, out int themeNum))
                {
                    EThemeType themeType = (EThemeType)themeNum;
                    isDarkMode = themeType == EThemeType.DARK;
                }
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

            if (settingConfig != null)
            {
                settingConfig.LanguageId = languages[language].Id;
                await dataService.PutSettingConfig(settingConfig);
            }

            CultureInfo culture = new(language);

            await js.InvokeVoidAsync("blazorCulture.set", culture!.Name);

            string uri = new Uri(navigationManager.Uri)
                .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            string cultureEscaped = Uri.EscapeDataString(culture.Name);
            string uriEscaped = Uri.EscapeDataString(uri);

            navigationManager.NavigateTo(
                $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
                forceLoad: true);
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
            EThemeType theme = isDarkTheme ? EThemeType.DARK : EThemeType.LIGHT;
            await js.InvokeVoidAsync("theme.set", theme);
        }
    }
}
