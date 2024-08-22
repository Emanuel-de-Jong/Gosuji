using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net;

namespace Gosuji.Client.Services
{
    public class SettingConfigService
    {
        public const string LANGUAGE_ID_STORAGE_NAME = "languageId";
        private const string THEME_STORAGE_NAME = "theme";
        private const string MASTER_VOLUME_STORAGE_NAME = "masterVolume";
        private const string STONE_VOLUME_STORAGE_NAME = "stoneVolume";
        private const string PRE_MOVE_SOUND_STORAGE_NAME = "isPreMoveStoneSound";
        private const string SELFPLAY_SOUND_STORAGE_NAME = "isSelfplayStoneSound";

        private DataService dataService;
        private IJSRuntime js;
        private NavigationManager navigationManager;

        public bool IsUser { get; private set; } = false;
        public SettingConfig SettingConfig { get; private set; }

        public SettingConfigService(DataService dataService, IJSRuntime js, NavigationManager navigationManager)
        {
            this.dataService = dataService;
            this.js = js;
            this.navigationManager = navigationManager;
        }

        public async Task InitSettingConfig()
        {
            string? languageId = await js.InvokeAsync<string>("utils.getLocal", LANGUAGE_ID_STORAGE_NAME);
            string? theme = await js.InvokeAsync<string>("utils.getLocal", THEME_STORAGE_NAME);
            string? masterVolume = await js.InvokeAsync<string>("utils.getLocal", MASTER_VOLUME_STORAGE_NAME);
            string? stoneVolume = await js.InvokeAsync<string>("utils.getLocal", STONE_VOLUME_STORAGE_NAME);
            string? isPreMoveStoneSound = await js.InvokeAsync<string>("utils.getLocal", PRE_MOVE_SOUND_STORAGE_NAME);
            string? isSelfplayStoneSound = await js.InvokeAsync<string>("utils.getLocal", SELFPLAY_SOUND_STORAGE_NAME);

            SettingConfig = new();
            SettingConfig.LanguageId = !string.IsNullOrWhiteSpace(languageId) ? languageId : SettingConfig.LanguageId;
            SettingConfig.Theme = !string.IsNullOrWhiteSpace(theme) ? (EThemeType)int.Parse(theme) : SettingConfig.Theme;
            SettingConfig.MasterVolume = !string.IsNullOrWhiteSpace(masterVolume) ? int.Parse(masterVolume) : SettingConfig.MasterVolume;
            SettingConfig.StoneVolume = !string.IsNullOrWhiteSpace(stoneVolume) ? int.Parse(stoneVolume) : SettingConfig.StoneVolume;
            SettingConfig.IsPreMoveStoneSound = !string.IsNullOrWhiteSpace(isPreMoveStoneSound) ? bool.Parse(isPreMoveStoneSound) : SettingConfig.IsPreMoveStoneSound;
            SettingConfig.IsSelfplayStoneSound = !string.IsNullOrWhiteSpace(isSelfplayStoneSound) ? bool.Parse(isSelfplayStoneSound) : SettingConfig.IsSelfplayStoneSound;
        }

        public async Task<bool> SettingConfigFromDb()
        {
            APIResponse<SettingConfig> settingConfigResponse = await dataService.GetSettingConfig();

            if (G.StatusMessage.HandleAPIResponse(settingConfigResponse)) return false;

            if (settingConfigResponse.Data == null)
            {
                settingConfigResponse.StatusCode = HttpStatusCode.BadRequest;
                settingConfigResponse.Message = "SettingConfig is null";
                G.StatusMessage.HandleAPIResponse(settingConfigResponse);
                return false;
            }

            SettingConfig = settingConfigResponse.Data;
            IsUser = true;
            return true;
        }

        private async Task UpdateSettingConfig()
        {
            await js.InvokeVoidAsync("utils.setLocal", LANGUAGE_ID_STORAGE_NAME, SettingConfig.LanguageId);

            if (IsUser)
            {
                APIResponse response = await dataService.PutSettingConfig(SettingConfig);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
            }
            else
            {
                await js.InvokeVoidAsync("utils.setLocal", MASTER_VOLUME_STORAGE_NAME, SettingConfig.MasterVolume);
                await js.InvokeVoidAsync("utils.setLocal", STONE_VOLUME_STORAGE_NAME, SettingConfig.StoneVolume);
                await js.InvokeVoidAsync("utils.setLocal", PRE_MOVE_SOUND_STORAGE_NAME, SettingConfig.IsPreMoveStoneSound);
                await js.InvokeVoidAsync("utils.setLocal", SELFPLAY_SOUND_STORAGE_NAME, SettingConfig.IsSelfplayStoneSound);
            }
        }

        public async Task ChangeIsDarkMode(ChangeEventArgs e)
        {
            bool isDarkMode = (bool)e.Value;
            await ChangeTheme(isDarkMode ? EThemeType.DARK : EThemeType.LIGHT);
        }

        public async Task ChangeTheme(EThemeType theme)
        {
            await js.InvokeVoidAsync("theme.set", theme);

            if (SettingConfig.Theme == theme) return;

            SettingConfig.Theme = theme;
            await UpdateSettingConfig();
        }

        public async Task ChangeLanguage(string language)
        {
            if (SettingConfig.LanguageId != language)
            {
                SettingConfig.LanguageId = language;
                await UpdateSettingConfig();
            }

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != language)
            {
                await js.InvokeVoidAsync("utils.setLocal", LANGUAGE_ID_STORAGE_NAME, SettingConfig.LanguageId);
                navigationManager.NavigateTo(navigationManager.Uri, true);
            }
        }

        public async Task ChangeMasterVolume(ChangeEventArgs e)
        {
            int volume = Convert.ToInt32(e.Value);
            await ChangeMasterVolume(volume);
        }

        public async Task ChangeMasterVolume(int volume)
        {
            if (SettingConfig.MasterVolume == volume) return;
            SettingConfig.MasterVolume = volume;
            await UpdateSettingConfig();
        }

        public async Task ChangeStoneVolume(ChangeEventArgs e)
        {
            int volume = Convert.ToInt32(e.Value);
            await ChangeStoneVolume(volume);
        }

        public async Task ChangeStoneVolume(int volume)
        {
            if (SettingConfig.StoneVolume == volume) return;
            SettingConfig.StoneVolume = volume;
            await UpdateSettingConfig();
        }

        public async Task ChangeIsPreMoveStoneSound(ChangeEventArgs e)
        {
            bool val = Convert.ToBoolean(e.Value);
            await ChangeIsPreMoveStoneSound(val);
        }

        public async Task ChangeIsPreMoveStoneSound(bool isStoneSound)
        {
            if (SettingConfig.IsPreMoveStoneSound == isStoneSound) return;
            SettingConfig.IsPreMoveStoneSound = isStoneSound;
            await UpdateSettingConfig();
        }

        public async Task ChangeIsSelfplayStoneSound(ChangeEventArgs e)
        {
            bool val = Convert.ToBoolean(e.Value);
            await ChangeIsSelfplayStoneSound(val);
        }

        public async Task ChangeIsSelfplayStoneSound(bool isStoneSound)
        {
            if (SettingConfig.IsSelfplayStoneSound == isStoneSound) return;
            SettingConfig.IsSelfplayStoneSound = isStoneSound;
            await UpdateSettingConfig();
        }

        public async Task ChangeIsGetChangelogEmail(ChangeEventArgs e)
        {
            bool val = Convert.ToBoolean(e.Value);
            await ChangeIsGetChangelogEmail(val);
        }

        public async Task ChangeIsGetChangelogEmail(bool isGetChangelogEmail)
        {
            if (SettingConfig.IsGetChangelogEmail == isGetChangelogEmail) return;
            SettingConfig.IsGetChangelogEmail = isGetChangelogEmail;
            await UpdateSettingConfig();
        }
    }
}
