using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
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
        private DataService dataService { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }

        private string? currentLanguageSrc;
        private Dictionary<string, Language>? languages;
        private Language? currentLanguage;

        protected override async Task OnInitializedAsync()
        {
            await settingConfigService.InitSettingConfig();

            APIResponse<Dictionary<string, Language>> languagesResponse = await dataService.GetLanguages();
            if (G.StatusMessage.HandleAPIResponse(languagesResponse)) return;
            languages = languagesResponse.Data;


            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                if (!await settingConfigService.SettingConfigFromDb()) return;

                await settingConfigService.ChangeLanguage(settingConfigService.SettingConfig.LanguageId);
            }

            currentLanguage = languages[settingConfigService.SettingConfig.LanguageId];
            currentLanguageSrc = BASE_LANGUAGE_SRC + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".svg";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await settingConfigService.ChangeTheme(settingConfigService.SettingConfig.Theme);
            }
        }
    }
}
