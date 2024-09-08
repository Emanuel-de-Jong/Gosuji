using Gosuji.Client.Data;
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
        private TrainerService trainerService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private DataService dataService { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }
        [Inject]
        private IStringLocalizer<General> tl { get; set; }

        [SupplyParameterFromForm]
        private PresetModel addPresetModel { get; set; } = new();

        private bool isJSInitialized = false;
        private IJSObjectReference jsRef;
        private string? userName;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<TrainerService>? trainerServiceRef;

        private Dictionary<long, Preset>? presets;
        private UserState? userState;
        private Preset? currentPreset;
        private TrainerSettingConfig? trainerSettingConfig;
        private KataGoVisits? kataGoVisits;

        private Game? game;
        private GameStat? gameStat;
        private GameStat? openingStat;
        private GameStat? midgameStat;
        private GameStat? endgameStat;
        private KataGoVersion? kataGoVersion;

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
            trainerServiceRef = DotNetObjectReference.Create(trainerService);

            APIResponse<Dictionary<long, Preset>> presetsResponse = await dataService.GetPresets();
            if (G.StatusMessage.HandleAPIResponse(presetsResponse)) return;
            presets = presetsResponse.Data;

            APIResponse<UserState> userStateResponse = await dataService.GetUserState();
            if (G.StatusMessage.HandleAPIResponse(userStateResponse)) return;
            userState = userStateResponse.Data;

            currentPreset = presets[userState.LastPresetId];

            APIResponse<TrainerSettingConfig> trainerSettingConfigResponse = await dataService.GetTrainerSettingConfig(currentPreset.TrainerSettingConfigId);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            trainerSettingConfig = trainerSettingConfigResponse.Data;

            kataGoVisits = new()
            {
                SuggestionVisits = trainerSettingConfig.SuggestionVisits != null ? trainerSettingConfig.SuggestionVisits.Value : 200,
                OpponentVisits = trainerSettingConfig.OpponentVisits != null ? trainerSettingConfig.OpponentVisits.Value : 200,
                PreVisits = trainerSettingConfig.PreVisits != null ? trainerSettingConfig.PreVisits.Value : 200,
                SelfplayVisits = trainerSettingConfig.SelfplayVisits != null ? trainerSettingConfig.SelfplayVisits.Value : 200,
            };
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

            if (kataGoVisits != null && !isJSInitialized)
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
                APIResponse<Game> response = await dataService.GetGame(GameId, true);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
                game = response.Data;

                trainerSettingConfig = game.TrainerSettingConfig;

                gameLoadInfo = new(game);
            }

            await jsRef.InvokeVoidAsync("trainerPage.init",
                trainerRef,
                trainerServiceRef,
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
            if (trainerService.IsConnected)
            {
                return true;
            }

            APIResponse startResponse = await trainerService.Start();
            if (G.StatusMessage.HandleAPIResponse(startResponse)) return false;

            APIResponse<bool> userHasInstanceResponse = await trainerService.UserHasInstance();
            if (G.StatusMessage.HandleAPIResponse(userHasInstanceResponse)) return false;
            bool userHasInstance = userHasInstanceResponse.Data;

            if (userHasInstance)
            {
                G.StatusMessage.SetMessage("You already use this page somewhere else!", false);
                return false;
            }

            APIResponse<KataGoVersion> getVersionResponse = await trainerService.GetVersion();
            if (G.StatusMessage.HandleAPIResponse(getVersionResponse)) return false;
            kataGoVersion = getVersionResponse.Data;

            await jsRef.InvokeVoidAsync("trainerG.setKataGoVersion", kataGoVersion);

            return true;
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
            APIResponse<TrainerSettingConfig> trainerSettingConfigResponse = await dataService.GetTrainerSettingConfig(lastPreset.TrainerSettingConfigId);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            trainerSettingConfig = trainerSettingConfigResponse.Data;

            userState.LastPresetId = presetId;
            currentPreset = lastPreset;
            APIResponse response = await dataService.PutUserState(userState);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
        }

        private async Task SavePreset()
        {
            APIResponse<long> trainerSettingConfigResponse = await dataService.PostTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            long? trainerSettingConfigId = trainerSettingConfigResponse.Data;

            if (currentPreset.TrainerSettingConfigId != trainerSettingConfigId)
            {
                currentPreset.TrainerSettingConfigId = trainerSettingConfigId.Value;

                APIResponse response = await dataService.PutPreset(currentPreset);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
            }
        }

        private async Task DeletePreset()
        {
            long oldSelectedPresetId = currentPreset.Id;
            await SelectPreset(presets.Values.Where(p => p.Order == 1).FirstOrDefault().Id);

            presets.Remove(oldSelectedPresetId);

            APIResponse response = await dataService.DeletePreset(oldSelectedPresetId);
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
            APIResponse<long> trainerSettingConfigResponse = await dataService.PostTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(trainerSettingConfigResponse)) return;
            long trainerSettingConfigId = trainerSettingConfigResponse.Data;

            Preset newPreset = new()
            {
                Name = addPresetModel.Name,
                TrainerSettingConfigId = trainerSettingConfigId
            };

            APIResponse<long> presetResponse = await dataService.PostPreset(newPreset);
            if (G.StatusMessage.HandleAPIResponse(presetResponse)) return;
            long newPresetId = presetResponse.Data;

            newPreset.Id = newPresetId;
            presets.Add(newPreset.Id, newPreset);

            userState.LastPresetId = newPreset.Id;
            currentPreset = newPreset;

            APIResponse response = await dataService.PutUserState(userState);
            if (G.StatusMessage.HandleAPIResponse(response)) return;

            addPresetModel = new();
        }

        public async Task SetSuggestionVisits(ChangeEventArgs e)
        {
            if(!int.TryParse(e.Value?.ToString(), out int visits))
            {
               return;
            }
            kataGoVisits.SuggestionVisits = visits;
            trainerSettingConfig.SuggestionVisits = visits;
        }

        public async Task SetOpponentVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            kataGoVisits.OpponentVisits = visits;
            trainerSettingConfig.OpponentVisits = visits;
        }

        public async Task SetPreVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            kataGoVisits.PreVisits = visits;
            trainerSettingConfig.PreVisits = visits;
        }

        public async Task SetSelfplayVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            kataGoVisits.SelfplayVisits = visits;
            trainerSettingConfig.SelfplayVisits = visits;
        }

        public async ValueTask DisposeAsync()
        {
            trainerRef?.Dispose();
            trainerServiceRef?.Dispose();

            if (trainerService.IsConnected)
            {
                await trainerService.Return();
                await trainerService.Stop();
            }
        }
    }

    public class KataGoVisits
    {
        public int SuggestionVisits { get; set; }
        public int OpponentVisits { get; set; }
        public int PreVisits { get; set; }
        public int SelfplayVisits { get; set; }
    }
}
