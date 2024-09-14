using Gosuji.Client.Data;
using Gosuji.Client.Helpers;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Trainer : CustomPage, IAsyncDisposable
    {
        [Parameter]
        public string? GameId { get; set; }

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private TrainerConnection trainerConnection { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private DataAPI dataAPI { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }
        [Inject]
        private IStringLocalizer<General> tl { get; set; }

        [SupplyParameterFromForm]
        private PresetModel addPresetModel { get; set; } = new();

        private bool isInitialized = false;
        private bool isJSInitialized = false;
        private IJSObjectReference jsRef;
        private string? userName;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<TrainerConnection>? trainerConnectionRef;

        private Dictionary<long, Preset>? presets;
        private UserState? userState;
        private Preset? currentPreset;
        private TrainerSettingConfig? trainerSettingConfig;
        private NullableTrainerSettings? nullableTrainerSettings;

        private Game? game;
        private GameStat? gameStat;
        private GameStat? openingStat;
        private GameStat? midgameStat;
        private GameStat? endgameStat;

        private MoveSuggestion[]? suggestions;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo("register");
            }

            userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

            trainerRef = DotNetObjectReference.Create(this);
            trainerConnectionRef = DotNetObjectReference.Create(trainerConnection);

            APIResponse<Dictionary<long, Preset>> presetsResponse = await dataAPI.GetPresets();
            if (G.StatusMessage.HandleAPIResponse(presetsResponse)) return;
            presets = presetsResponse.Data;

            APIResponse<UserState> userStateResponse = await dataAPI.GetUserState();
            if (G.StatusMessage.HandleAPIResponse(userStateResponse)) return;
            userState = userStateResponse.Data;

            currentPreset = presets[userState.LastPresetId];

            APIResponse<TrainerSettingConfig> trainerSettingConfigResponse = await dataAPI.GetTrainerSettingConfig(currentPreset.TrainerSettingConfigId);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            trainerSettingConfig = trainerSettingConfigResponse.Data;

            nullableTrainerSettings = new(trainerSettingConfig, settingConfigService.SettingConfig.LanguageId);

            isInitialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (userName == null)
            {
                return;
            }

            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["ChartJS"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            if (isInitialized && !isJSInitialized)
            {
                isJSInitialized = true;

                jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/trainer/bundle.js");
                await InitJS();
            }
        }

        public async Task InitJS()
        {
            GameLoadInfo? gameLoadInfo = null;
            if (GameId != null)
            {
                APIResponse<Game> response = await dataAPI.GetGame(GameId, true);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
                game = response.Data;

                trainerSettingConfig = game.TrainerSettingConfig;

                gameLoadInfo = new(game);
            }

            await jsRef.InvokeVoidAsync("trainerPage.init",
                trainerRef,
                trainerConnectionRef,
                userName,
                settingConfigService.SettingConfig.CalcStoneVolume(),
                settingConfigService.SettingConfig.IsPreMoveStoneSound,
                settingConfigService.SettingConfig.IsSelfplayStoneSound,
                gameLoadInfo);

            settingConfigService.StoneVolumeChanged += async (int volume) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setStoneVolume", settingConfigService.SettingConfig.CalcStoneVolume());
            settingConfigService.IsPreMoveStoneSoundChanged += async (bool isStoneSound) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setIsPreMoveStoneSound", isStoneSound);
            settingConfigService.IsSelfplayStoneSoundChanged += async (bool isStoneSound) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setIsSelfplayStoneSound", isStoneSound);
        }

        [JSInvokable]
        public async Task<bool> Start()
        {
            if (trainerConnection.IsConnected)
            {
                return true;
            }

            APIResponse startResponse = await trainerConnection.Start();
            if (G.StatusMessage.HandleAPIResponse(startResponse)) return false;

            APIResponse<bool> userHasInstanceResponse = await trainerConnection.UserHasInstance();
            if (G.StatusMessage.HandleAPIResponse(userHasInstanceResponse)) return false;
            bool userHasInstance = userHasInstanceResponse.Data;

            if (userHasInstance)
            {
                G.StatusMessage.SetMessage("You already use this page somewhere else!", false);
                return false;
            }

            return true;
        }

        [JSInvokable]
        public async Task<bool> InitTrainerConnection(string ruleset, double komi, bool isThirdParty)
        {
            NullableTrainerSettings ntsWithGameValues = ReflectionHelper.DeepClone(nullableTrainerSettings);
            ntsWithGameValues.Ruleset = ruleset;
            ntsWithGameValues.Komi = komi;

            APIResponse response = await trainerConnection.Init(trainerSettingConfig, ntsWithGameValues, isThirdParty);
            if (G.StatusMessage.HandleAPIResponse(response)) return false;

            return true;
        }

        [JSInvokable]
        public async Task UpdateTrainerSettingConfigTrainerConnection(string propertyName, string value)
        {
            ReflectionHelper.SetProperty(nullableTrainerSettings, propertyName, value);

            bool isSetSuccess = ReflectionHelper.SetProperty(trainerSettingConfig, propertyName, value);
            if (!isSetSuccess)
            {
                G.StatusMessage.SetMessage($"{propertyName} is not in TrainerSettingConfig.", false);
                return;
            }

            APIResponse response = await trainerConnection.UpdateTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
        }

        [JSInvokable]
        public async Task SetSuggestions(MoveSuggestion[]? suggestions)
        {
            if (this.suggestions != null || (this.suggestions == null && suggestions != null))
            {
                this.suggestions = suggestions;
                StateHasChanged();
            }
        }

        private async Task SelectPreset(ChangeEventArgs e)
        {
            await SelectPreset(long.Parse(e.Value.ToString()));
        }

        private async Task SelectPreset(long presetId)
        {
            Preset lastPreset = presets[presetId];
            APIResponse<TrainerSettingConfig> trainerSettingConfigResponse = await dataAPI.GetTrainerSettingConfig(lastPreset.TrainerSettingConfigId);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            trainerSettingConfig = trainerSettingConfigResponse.Data;

            nullableTrainerSettings.Init(trainerSettingConfig, settingConfigService.SettingConfig.LanguageId);

            await jsRef.InvokeVoidAsync("settings.syncWithCS", trainerSettingConfig, nullableTrainerSettings);

            userState.LastPresetId = presetId;
            currentPreset = lastPreset;
            APIResponse response = await dataAPI.PutUserState(userState);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
        }

        private async Task SavePreset()
        {
            APIResponse<long> trainerSettingConfigResponse = await dataAPI.PostTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            long? trainerSettingConfigId = trainerSettingConfigResponse.Data;

            if (currentPreset.TrainerSettingConfigId != trainerSettingConfigId)
            {
                currentPreset.TrainerSettingConfigId = trainerSettingConfigId.Value;

                APIResponse response = await dataAPI.PutPreset(currentPreset);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
            }
        }

        private async Task DeletePreset()
        {
            long oldSelectedPresetId = currentPreset.Id;
            await SelectPreset(presets.Values.Where(p => p.Order == 1).FirstOrDefault().Id);

            presets.Remove(oldSelectedPresetId);

            APIResponse response = await dataAPI.DeletePreset(oldSelectedPresetId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
        }

        private sealed class PresetModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(22, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(1, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Name { get; set; }
        }

        private async Task AddPreset()
        {
            APIResponse<long> trainerSettingConfigResponse = await dataAPI.PostTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            long trainerSettingConfigId = trainerSettingConfigResponse.Data;

            Preset newPreset = new()
            {
                Name = addPresetModel.Name,
                TrainerSettingConfigId = trainerSettingConfigId
            };

            APIResponse<long> presetResponse = await dataAPI.PostPreset(newPreset);
            if (G.StatusMessage.HandleAPIResponse(presetResponse)) return;
            long newPresetId = presetResponse.Data;

            newPreset.Id = newPresetId;
            presets.Add(newPreset.Id, newPreset);

            userState.LastPresetId = newPreset.Id;
            currentPreset = newPreset;

            APIResponse response = await dataAPI.PutUserState(userState);
            if (G.StatusMessage.HandleAPIResponse(response)) return;

            addPresetModel = new();
        }

        [JSInvokable]
        public async Task<double> GetDefaultKomi(string ruleset)
        {
            return trainerSettingConfig.GetDefaultKomi(ruleset);
        }

        public async ValueTask DisposeAsync()
        {
            trainerRef?.Dispose();
            trainerConnectionRef?.Dispose();

            if (trainerConnection.IsConnected)
            {
                await trainerConnection.Stop();
            }
        }
    }
}
