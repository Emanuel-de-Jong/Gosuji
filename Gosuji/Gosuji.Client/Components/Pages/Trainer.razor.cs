using Gosuji.Client.Data;
using Gosuji.Client.Helpers;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Gosuji.Client.Services.Trainer;
using Gosuji.Client.Services.TrainerService;
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
        private GameLoadInfo? gameLoadInfo;

        private string sgfRuleset;
        private double sgfKomi;

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

            currentPreset = presets[userState.LastPresetId.Value];

            await SetTrainerSettingConfig(currentPreset.TrainerSettingConfigId);

            if (GameId != null)
            {
                if (!await StartTrainerConnection())
                {
                   return;
                }

                APIResponse<LoadGameResponse> loadGameResponse = await trainerConnection.LoadGame(GameId);
                if (G.StatusMessage.HandleAPIResponse(loadGameResponse)) return;
                SetTrainerSettingConfig(loadGameResponse.Data.TrainerSettingConfig);
                gameLoadInfo = new(loadGameResponse.Data.Game, loadGameResponse.Data.MoveTree);
            }

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

        private async Task<bool> StartTrainerConnection()
        {
            if (!trainerConnection.IsConnected)
            {
                APIResponse startResponse = await trainerConnection.Start();
                if (G.StatusMessage.HandleAPIResponse(startResponse)) return false;
            }

            return true;
        }

        public async Task InitJS()
        {
            await jsRef.InvokeVoidAsync("trainerPage.init",
                trainerRef,
                trainerConnectionRef,
                userName,
                settingConfigService.SettingConfig.CalcStoneVolume(),
                settingConfigService.SettingConfig.IsPreMoveStoneSound,
                settingConfigService.SettingConfig.IsSelfplayStoneSound,
                trainerSettingConfig,
                gameLoadInfo);

            settingConfigService.StoneVolumeChanged += async (int volume) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setStoneVolume", settingConfigService.SettingConfig.CalcStoneVolume());
            settingConfigService.IsPreMoveStoneSoundChanged += async (bool isStoneSound) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setIsPreMoveStoneSound", isStoneSound);
            settingConfigService.IsSelfplayStoneSoundChanged += async (bool isStoneSound) =>
                await jsRef.InvokeVoidAsync("trainerG.board.setIsSelfplayStoneSound", isStoneSound);
        }

        private async Task SetTrainerSettingConfig(long id)
        {
            APIResponse<TrainerSettingConfig> trainerSettingConfigResponse = await dataAPI.GetTrainerSettingConfig(id);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            SetTrainerSettingConfig(trainerSettingConfigResponse.Data);
        }

        private void SetTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            trainerSettingConfig.Language = Enum.Parse<ELanguage>(settingConfigService.SettingConfig.LanguageId);
            trainerSettingConfig.SubscriptionType = settingConfigService.Subscription?.SubscriptionType;
            this.trainerSettingConfig = trainerSettingConfig;
        }

        [JSInvokable]
        public async Task<bool> InitTrainerConnection(string sgfRuleset, double sgfKomi,
            TreeNode<Move>? thirdPartyMoves, string? name)
        {
            if (!await StartTrainerConnection())
            {
                return false;
            }

            this.sgfRuleset = sgfRuleset;
            this.sgfKomi = sgfKomi;

            TrainerSettingConfig tscWithSGFSettings = ReflectionHelper.DeepClone(trainerSettingConfig);
            tscWithSGFSettings.SGFRuleset = sgfRuleset;
            tscWithSGFSettings.SGFKomi = sgfKomi;

            APIResponse<bool> initResponse = await trainerConnection.Init(tscWithSGFSettings, thirdPartyMoves, name);
            if (G.StatusMessage.HandleAPIResponse(initResponse)) return false;
            bool isOnlyInstance = initResponse.Data;

            if (!isOnlyInstance)
            {
                G.StatusMessage.SetMessage("You already use this page somewhere else!", false);
                return false;
            }

            return true;
        }

        [JSInvokable]
        public async Task UpdateTrainerSettingConfig(string propertyName, string value)
        {
            bool isSetSuccess = ReflectionHelper.SetProperty(trainerSettingConfig, propertyName, value);
            if (!isSetSuccess)
            {
                G.StatusMessage.SetMessage($"{propertyName} is not in TrainerSettingConfig.", false);
                return;
            }

            if (trainerConnection.IsConnected)
            {
                TrainerSettingConfig tscWithSGFSettings = ReflectionHelper.DeepClone(trainerSettingConfig);
                tscWithSGFSettings.SGFRuleset = sgfRuleset;
                tscWithSGFSettings.SGFKomi = sgfKomi;

                APIResponse response = await trainerConnection.UpdateTrainerSettingConfig(tscWithSGFSettings);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
            }
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

            await SetTrainerSettingConfig(lastPreset.TrainerSettingConfigId);

            await jsRef.InvokeVoidAsync("settings.syncWithCS", trainerSettingConfig);

            userState.LastPresetId = presetId;
            currentPreset = lastPreset;
            APIResponse userStateResponse = await dataAPI.PutUserState(userState);
            if (G.StatusMessage.HandleAPIResponse(userStateResponse)) return;

            if (trainerConnection.IsConnected)
            {
                APIResponse updateTrainerSettingConfigResponse = await trainerConnection.UpdateTrainerSettingConfig(trainerSettingConfig);
                if (G.StatusMessage.HandleAPIResponse(updateTrainerSettingConfigResponse)) return;
            }
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
