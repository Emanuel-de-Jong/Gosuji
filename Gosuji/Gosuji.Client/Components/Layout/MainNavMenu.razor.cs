﻿using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Layout
{
    public partial class MainNavMenu : ComponentBase
    {
        private static readonly string BASE_LANGUAGE_SRC = "resources/imgs/flags/";

        [Inject]
        private DataAPI dataAPI { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }

        private Dictionary<string, Language>? languages;
        private bool isSettingConfigInitialized = false;

        protected override async Task OnInitializedAsync()
        {
            APIResponse<Dictionary<string, Language>> languagesResponse = await dataAPI.GetLanguages();
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
