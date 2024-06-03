using Gosuji.Client.Controllers.GameDecoder;
using Gosuji.Client.Data;
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
        private IKataGoService kataGoService { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDataService dataService { get; set; }

        private IJSObjectReference jsRef;
        private string? userId;
        private string? userName;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<IKataGoService>? kataGoServiceRef;

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
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
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
                if (userId == null)
                {
                    return;
                }

                if (await kataGoService.UserHasInstance(userId))
                {
                    await js.InvokeVoidAsync("alert", "You already use this page somewhere else!");
                    return;
                }

                trainerRef = DotNetObjectReference.Create(this);
                kataGoServiceRef = DotNetObjectReference.Create(kataGoService);

                await Start();
            }
        }

        public async Task Start()
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

                jsRef.InvokeVoidAsync("init.init",
                    trainerRef,
                    kataGoServiceRef,
                    userId,
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
                jsRef.InvokeVoidAsync("init.init",
                    trainerRef,
                    kataGoServiceRef,
                    userId,
                    userName,
                    kataGoVersion);
            }
        }


        [JSInvokable]
        public async Task SaveTrainerSettingConfig(
            int boardsize,
            int handicap,
            int colorType,
            bool preMovesSwitch,
            int preMoves,
            int preVisits,
            int selfplayVisits,
            int suggestionVisits,
            int opponentVisits,
            bool disableAICorrection,

            string ruleset,
            string komiChangeStyle,
            float komi,

            int preOptions,
            float preOptionPerc,
            string forceOpponentCorners,
            bool cornerSwitch44,
            bool cornerSwitch34,
            bool cornerSwitch33,
            bool cornerSwitch45,
            bool cornerSwitch35,
            int cornerChance44,
            int cornerChance34,
            int cornerChance33,
            int cornerChance45,
            int cornerChance35,

            int suggestionOptions,
            bool showOptions,
            bool showWeakerOptions,
            bool minVisitsPercSwitch,
            float minVisitsPerc,
            bool maxVisitDiffPercSwitch,
            float maxVisitDiffPerc,

            bool opponentOptionsSwitch,
            int opponentOptions,
            float opponentOptionPerc,
            bool showOpponentOptions)
        {
            if (G.Log)
            {
                Console.WriteLine("Index.SaveTrainerSettingConfig");
            }

            TrainerSettingConfig newConfig = new()
            {
                Boardsize = boardsize,
                Handicap = handicap,
                ColorType = colorType,
                PreMovesSwitch = preMovesSwitch,
                PreMoves = preMoves,
                PreVisits = preVisits,
                SelfplayVisits = selfplayVisits,
                SuggestionVisits = suggestionVisits,
                OpponentVisits = opponentVisits,
                DisableAICorrection = disableAICorrection,

                Ruleset = ruleset,
                KomiChangeStyle = komiChangeStyle,
                Komi = komi,

                PreOptions = preOptions,
                PreOptionPerc = preOptionPerc,
                ForceOpponentCorners = forceOpponentCorners,
                CornerSwitch44 = cornerSwitch44,
                CornerSwitch34 = cornerSwitch34,
                CornerSwitch33 = cornerSwitch33,
                CornerSwitch45 = cornerSwitch45,
                CornerSwitch35 = cornerSwitch35,
                CornerChance44 = cornerChance44,
                CornerChance34 = cornerChance34,
                CornerChance33 = cornerChance33,
                CornerChance45 = cornerChance45,
                CornerChance35 = cornerChance35,

                SuggestionOptions = suggestionOptions,
                ShowOptions = showOptions,
                ShowWeakerOptions = showWeakerOptions,
                MinVisitsPercSwitch = minVisitsPercSwitch,
                MinVisitsPerc = minVisitsPerc,
                MaxVisitDiffPercSwitch = maxVisitDiffPercSwitch,
                MaxVisitDiffPerc = maxVisitDiffPerc,

                OpponentOptionsSwitch = opponentOptionsSwitch,
                OpponentOptions = opponentOptions,
                OpponentOptionPerc = opponentOptionPerc,
                ShowOpponentOptions = showOpponentOptions,
            };

            newConfig.SetHash();

            if (trainerSettingConfig?.Hash == newConfig.Hash)
            {
                return;
            }

            newConfig.Id = await dataService.PostTrainerSettingConfig(newConfig);
            trainerSettingConfig = newConfig;
        }

        [JSInvokable]
        public async Task SaveGameStats(GameStat newGameStat, GameStat newOpeningStat, GameStat newMidgameStat, GameStat newEndgameStat)
        {
            if (G.Log)
            {
                Console.WriteLine("Index.SaveGameStats");
            }

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
                newGameStat.Id = await dataService.PostGameStat(newGameStat);
            }
            else
            {
                newGameStat.Id = gameStat.Id;
                await dataService.PutGameStat(newGameStat);
            }

            return newGameStat;
        }

        [JSInvokable]
        public async Task SaveGame(
            int? result,
            int prevNodeX,
            int prevNodeY,
            int boardsize,
            int handicap,
            int color,
            string ruleset,
            float komi,
            string sgf,
            byte[] encodedRatios,
            byte[] encodedSuggestions,
            byte[] encodedMoveTypes,
            byte[] encodedChosenNotPlayedCoords,
            bool isFinished,
            bool isThirdPartySGF)
        {
            if (G.Log)
            {
                Console.WriteLine("Index.SaveGame");
            }

            if (userId == null)
            {
                return;
            }

            Game newGame = new()
            {
                UserId = userId,
                TrainerSettingConfigId = trainerSettingConfig.Id,
                KataGoVersionId = kataGoVersion.Id,
                GameStatId = gameStat?.Id,
                OpeningStatId = openingStat?.Id,
                MidgameStatId = midgameStat?.Id,
                EndgameStatId = endgameStat?.Id,
                Name = "GameX",
                Result = result,
                PrevNodeX = prevNodeX,
                PrevNodeY = prevNodeY,
                Boardsize = boardsize,
                Handicap = handicap,
                Color = color,
                Ruleset = ruleset,
                Komi = komi,
                SGF = sgf,
                Ratios = encodedRatios,
                Suggestions = encodedSuggestions,
                MoveTypes = encodedMoveTypes,
                ChosenNotPlayedCoords = encodedChosenNotPlayedCoords,
                IsFinished = isFinished,
                IsThirdPartySGF = isThirdPartySGF,
            };

            if (newGame.SGF == game?.SGF)
            {
                return;
            }

            if (game == null)
            {
                newGame.Id = await dataService.PostGame(newGame);
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

            if (userId != null)
            {
                kataGoService.Return(userId).Wait();
            }
        }
    }
}
