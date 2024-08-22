using Gosuji.Client.Data;
using Gosuji.Client.Helpers.GameDecoder;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Trainer : CustomPage, IAsyncDisposable
    {
        [Parameter]
        public long? GameId { get; set; }

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private KataGoService kataGoService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private DataService dataService { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }

        [SupplyParameterFromForm]
        private PresetModel addPresetModel { get; set; } = new();

        private bool isJSInitialized = false;
        private IJSObjectReference jsRef;
        private string? userName;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<KataGoService>? kataGoServiceRef;

        private Dictionary<long, Preset>? presets;
        private UserState? userState;
        private Preset? currentPreset;
        private TrainerSettingConfig? trainerSettingConfig;

        private Game? game;
        private GameStat? gameStat;
        private GameStat? openingStat;
        private GameStat? midgameStat;
        private GameStat? endgameStat;
        private KataGoVersion? kataGoVersion;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo("register");
            }

            userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

            await settingConfigService.SettingConfigFromDb();

            trainerRef = DotNetObjectReference.Create(this);
            kataGoServiceRef = DotNetObjectReference.Create(kataGoService);

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

            if (trainerSettingConfig != null && !isJSInitialized)
            {
                isJSInitialized = true;

                jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/trainer/bundle.js");

                APIResponse startResponse = await kataGoService.Start();
                if (G.StatusMessage.HandleAPIResponse(startResponse)) return;

                APIResponse<bool> response = await kataGoService.UserHasInstance();
                if (G.StatusMessage.HandleAPIResponse(response)) return;
                bool userHasInstance = response.Data;

                if (userHasInstance)
                {
                    await js.InvokeVoidAsync("alert", "You already use this page somewhere else!");
                    return;
                }

                await InitJS();
            }
        }

        public async Task InitJS()
        {
            APIResponse<KataGoVersion> response = await kataGoService.GetVersion();
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            kataGoVersion = response.Data;

            if (GameId != null)
            {
                game = (await dataService.GetGame(GameId.Value)).Data;
            }

            if (game != null)
            {
                Dictionary<short, Dictionary<short, ERatio>> decodedRatios = GameDecoder.DecodeRatios(game.Ratios).ToDict();
                Dictionary<short, Dictionary<short, SuggestionList>> decodedSuggestions = GameDecoder.DecodeSuggestions(game.Suggestions);
                Dictionary<short, Dictionary<short, EMoveType>> decodedMoveTypes = GameDecoder.DecodeMoveTypes(game.MoveTypes);
                Dictionary<short, Dictionary<short, Coord>> decodedChosenNotPlayedCoords = GameDecoder.DecodeChosenNotPlayedCoords(game.ChosenNotPlayedCoords);

                await jsRef.InvokeVoidAsync("trainerPage.init",
                    trainerRef,
                    kataGoServiceRef,
                    userName,
                    kataGoVersion,
                    settingConfigService.SettingConfig.CalcStoneVolume(),

                    game.Boardsize,
                    game.Handicap,
                    game.Color,
                    game.Komi,
                    game.Ruleset,
                    game.SGF,
                    decodedRatios,
                    decodedSuggestions,
                    decodedMoveTypes,
                    decodedChosenNotPlayedCoords);
            }
            else
            {
                await jsRef.InvokeVoidAsync("trainerPage.init",
                    trainerRef,
                    kataGoServiceRef,
                    userName,
                    kataGoVersion,
                    settingConfigService.SettingConfig.CalcStoneVolume());
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

        [JSInvokable]
        public async Task SaveTrainerSettingConfig()
        {
            APIResponse<long> response = await dataService.PostTrainerSettingConfig(trainerSettingConfig);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            long newId = response.Data;

            trainerSettingConfig.Id = newId;
        }

        [JSInvokable]
        public async Task SaveGameStats(GameStat newGameStat, GameStat newOpeningStat, GameStat newMidgameStat, GameStat newEndgameStat)
        {
            Task[] tasks =
            {
                Task.Run(async () => {
                    GameStat? updatedGameStat = await UpdateGameStat(gameStat, newGameStat);
                    if (updatedGameStat != null)
                    {
                        gameStat = updatedGameStat;
                    }
                }),
                Task.Run(async () => {
                    GameStat? updatedGameStat = await UpdateGameStat(openingStat, newOpeningStat);
                    if (updatedGameStat != null)
                    {
                        openingStat = updatedGameStat;
                    }
                }),
                Task.Run(async () => {
                    GameStat? updatedGameStat = await UpdateGameStat(midgameStat, newMidgameStat);
                    if (updatedGameStat != null)
                    {
                        midgameStat = updatedGameStat;
                    }
                }),
                Task.Run(async () => {
                    GameStat? updatedGameStat = await UpdateGameStat(endgameStat, newEndgameStat);
                    if (updatedGameStat != null)
                    {
                        endgameStat = updatedGameStat;
                    }
                }),
            };

            await Task.WhenAll(tasks);
        }

        private async Task<GameStat?> UpdateGameStat(GameStat? gameStat, GameStat newGameStat)
        {
            if (newGameStat.Total == 0)
            {
                return null;
            }

            if (gameStat != null && gameStat.Equal(newGameStat))
            {
                return null;
            }

            if (gameStat == null)
            {
                APIResponse<long> response = await dataService.PostGameStat(newGameStat);
                if (G.StatusMessage.HandleAPIResponse(response)) return null;
                long newGameStatId = response.Data;

                newGameStat.Id = newGameStatId;
            }
            else
            {
                newGameStat.Id = gameStat.Id;

                APIResponse response = await dataService.PutGameStat(newGameStat);
                if (G.StatusMessage.HandleAPIResponse(response)) return null;
            }

            return newGameStat;
        }

        [JSInvokable]
        public async Task SaveGame(Game newGame)
        {
            if (userName == null)
            {
                return;
            }

            newGame.TrainerSettingConfigId = trainerSettingConfig.Id;
            newGame.KataGoVersionId = kataGoVersion.Id;
            newGame.GameStatId = gameStat?.Id;
            newGame.OpeningStatId = openingStat?.Id;
            newGame.MidgameStatId = midgameStat?.Id;
            newGame.EndgameStatId = endgameStat?.Id;
            newGame.Name = "GameX";

            if (newGame.SGF == game?.SGF)
            {
                return;
            }

            if (game == null)
            {
                APIResponse<long> response = await dataService.PostGame(newGame);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
                long newGameId = response.Data;

                newGame.Id = newGameId;
            }
            else
            {
                newGame.Id = game.Id;

                APIResponse response = await dataService.PutGame(newGame);
                if (G.StatusMessage.HandleAPIResponse(response)) return;
            }
            game = newGame;
        }

        public async ValueTask DisposeAsync()
        {
            trainerRef?.Dispose();
            kataGoServiceRef?.Dispose();

            if (userName != null)
            {
                await kataGoService.Return();
                await kataGoService.Stop();
            }
        }
    }
}
