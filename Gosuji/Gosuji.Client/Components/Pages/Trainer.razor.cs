using Gosuji.Client.Data;
using Gosuji.Client.Helpers.GameDecoder;
using Gosuji.Client.Models;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Trainer : ComponentBase, IDisposable
    {
        [Parameter]
        public long? GameId { get; set; }

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private KataGoService kataGoService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private DataService dataService { get; set; }

        private IJSObjectReference jsRef;
        private string? userName;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<KataGoService>? kataGoServiceRef;

        private UserState? userState;
        private Dictionary<long, Preset>? presets;
        private Preset? selectedPreset;

        private Game? game;
        private TrainerSettingConfig? trainerSettingConfig;
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
                return;
            }

            userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

            trainerRef = DotNetObjectReference.Create(this);
            kataGoServiceRef = DotNetObjectReference.Create(kataGoService);

            userState = await dataService.GetUserState();
            presets = await dataService.GetPresets();
            selectedPreset = presets[userState.LastPresetId];
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

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/trainer/bundle.js?v=03-06-24");

            if (firstRender)
            {
                if (userName == null)
                {
                    return;
                }

                if ((await kataGoService.UserHasInstance()).Value)
                {
                    await js.InvokeVoidAsync("alert", "You already use this page somewhere else!");
                    return;
                }

                await InitJS();
            }
        }

        public async Task InitJS()
        {
            kataGoVersion = await kataGoService.GetVersion();

            if (GameId != null)
            {
                game = await dataService.GetGame(GameId.Value);
            }

            if (game != null)
            {
                RatioTree decodedRatios = GameDecoder.DecodeRatios(game.Ratios);
                Dictionary<short, Dictionary<short, SuggestionList>> decodedSuggestions = GameDecoder.DecodeSuggestions(game.Suggestions);
                Dictionary<short, Dictionary<short, EMoveType>> decodedMoveTypes = GameDecoder.DecodeMoveTypes(game.MoveTypes);
                Dictionary<short, Dictionary<short, Coord>> decodedChosenNotPlayedCoords = GameDecoder.DecodeChosenNotPlayedCoords(game.ChosenNotPlayedCoords);

                await jsRef.InvokeVoidAsync("trainerPage.init",
                    trainerRef,
                    kataGoServiceRef,
                    userName,
                    kataGoVersion,

                    game.Boardsize,
                    game.Handicap,
                    game.Color,
                    game.Komi,
                    game.Ruleset,
                    game.SGF,
                    decodedRatios.ToDict(),
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
                    kataGoVersion);
            }
        }

        private async Task SelectPreset(ChangeEventArgs e)
        {
            await SelectPreset(long.Parse(e.Value.ToString()));
        }

        private async Task SelectPreset(long presetId)
        {
            selectedPreset = presets[presetId];
            userState.LastPresetId = presetId;
            await dataService.PutUserState(userState);
        }

        private async Task DeletePreset()
        {
            if (selectedPreset.IsGeneral)
            {
                return;
            }

            long oldSelectedPresetId = selectedPreset.Id;
            await SelectPreset(presets.Values.Where(p => p.Order == 1).FirstOrDefault().Id);

            presets.Remove(oldSelectedPresetId);
            await dataService.DeletePreset(oldSelectedPresetId);
        }

        [JSInvokable]
        public async Task SaveTrainerSettingConfig(TrainerSettingConfig newConfig)
        {
            long? newId = await dataService.PostTrainerSettingConfig(newConfig);
            if (newId == null)
            {
                return;
            }

            newConfig.Id = newId.Value;
            trainerSettingConfig = newConfig;
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
                long? newGameStatId = await dataService.PostGameStat(newGameStat);
                if (newGameStatId == null)
                {
                    return null;
                }

                newGameStat.Id = newGameStatId.Value;
            }
            else
            {
                newGameStat.Id = gameStat.Id;
                await dataService.PutGameStat(newGameStat);
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
                long? newGameId = await dataService.PostGame(newGame);
                if (newGameId == null)
                {
                    return;
                }

                newGame.Id = newGameId.Value;
            }
            else
            {
                newGame.Id = game.Id;
                await dataService.PutGame(newGame);
            }
            game = newGame;
        }

        public void Dispose()
        {
            trainerRef?.Dispose();
            kataGoServiceRef?.Dispose();

            if (userName != null)
            {
                kataGoService.Return().Wait();
            }
        }
    }
}
