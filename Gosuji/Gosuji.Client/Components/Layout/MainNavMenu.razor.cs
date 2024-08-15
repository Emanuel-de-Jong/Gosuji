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

        private Dictionary<string, Language>? languages;
        private bool isSettingConfigInitialized = false;

        protected override async Task OnInitializedAsync()
        {
            await settingConfigService.InitSettingConfig();

            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                if (!await settingConfigService.SettingConfigFromDb()) return;

                await settingConfigService.ChangeLanguage(settingConfigService.SettingConfig.LanguageId);
            }

            APIResponse<Dictionary<string, Language>> languagesResponse = await dataService.GetLanguages();
            if (G.StatusMessage.HandleAPIResponse(languagesResponse)) return;
            languages = languagesResponse.Data;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isSettingConfigInitialized && settingConfigService.SettingConfig != null)
            {
                isSettingConfigInitialized = true;
                await settingConfigService.ChangeTheme(settingConfigService.SettingConfig.Theme);
            }
        }
    }
}
