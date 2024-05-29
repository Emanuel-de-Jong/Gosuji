using GosujiServer.Areas.Identity.Data;
using GosujiServer.Controllers;
using GosujiServer.Data;
using GosujiServer.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace GosujiServer.Pages
{
    public partial class Trainer : ComponentBase
    {
        [Parameter]
        public long? GameId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationStateTask { get; set; }

        [Inject]
        private UserManager<User>? userManager { get; set; }

        private ApplicationDbContext? context;

        private CKataGoWrapper? cKataGoWrapper;

        private DotNetObjectReference<Trainer>? trainerRef;
        private DotNetObjectReference<CKataGoWrapper>? cKataGoWrapperRef;

        private User? user;

        private Game? game;
        private TrainerSettingConfig? trainerSettingConfig;
        private GameStat? gameStat;
        private GameStat? openingStat;
        private GameStat? midgameStat;
        private GameStat? endgameStat;
        private KataGoVersion? kataGoVersion;

        public void Start()
        {
            context = dbService.GetContext();

            kataGoVersion = kataGoService.GetVersion();

            //GameId = 12;
            if (GameId != null)
            {
                game = context.Games.Find(GameId);
                Models.RatioTree decodedRatios = GameDecoder.DecodeRatios(game.Ratios);
                Dictionary<short, Dictionary<short, Models.SuggestionList>> decodedSuggestions = GameDecoder.DecodeSuggestions(game.Suggestions);
                Dictionary<short, Dictionary<short, Enums.EMoveType>> decodedMoveTypes = GameDecoder.DecodeMoveTypes(game.MoveTypes);
                Dictionary<short, Dictionary<short, Models.Coord>> decodedChosenNotPlayedCoords = GameDecoder.DecodeChosenNotPlayedCoords(game.ChosenNotPlayedCoords);

                JS.InvokeAsync<string>("init.init",
                    trainerRef,
                    cKataGoWrapperRef,
                    user.UserName,
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
                JS.InvokeAsync<string>("init.init",
                    trainerRef,
                    cKataGoWrapperRef,
                    user.UserName,
                    kataGoVersion);
            }

            context.Dispose();
        }


        [JSInvokable]
        public void SaveTrainerSettingConfig(
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
            if (G.Log) Console.WriteLine("Index.SaveTrainerSettingConfig");

            context = dbService.GetContext();

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

            TrainerSettingConfig? identicalConfig = context.TrainerSettingConfigs.Where(t => t.Hash == newConfig.Hash).FirstOrDefault();
            if (identicalConfig != null)
            {
                if (trainerSettingConfig != null)
                {
                    context.TrainerSettingConfigs.Remove(trainerSettingConfig);
                }

                trainerSettingConfig = identicalConfig;
            }
            else
            {
                if (trainerSettingConfig == null)
                {
                    trainerSettingConfig = newConfig;
                    context.TrainerSettingConfigs.Add(trainerSettingConfig);
                }
                else
                {
                    trainerSettingConfig.Update(newConfig);
                }
            }

            context.SaveChanges();
            context.Dispose();
        }

        [JSInvokable]
        public void SaveGameStats(GameStat newGameStat, GameStat newOpeningStat, GameStat newMidgameStat, GameStat newEndgameStat)
        {
            if (G.Log) Console.WriteLine("Index.SaveGameStats");

            context = dbService.GetContext();

            if (newGameStat.Total > 0)
            {
                if (gameStat == null)
                {
                    gameStat = newGameStat;
                    context.GameStats.Add(gameStat);
                }
                else
                {
                    gameStat.Update(newGameStat);
                }
            }


            if (newOpeningStat.Total > 0)
            {
                if (openingStat == null)
                {
                    openingStat = newOpeningStat;
                    context.GameStats.Add(openingStat);
                }
                else
                {
                    openingStat.Update(newOpeningStat);
                }
            }


            if (newMidgameStat.Total > 0)
            {
                if (midgameStat == null)
                {
                    midgameStat = newMidgameStat;
                    context.GameStats.Add(midgameStat);
                }
                else
                {
                    midgameStat.Update(newMidgameStat);
                }
            }


            if (newEndgameStat.Total > 0)
            {
                if (endgameStat == null)
                {
                    endgameStat = newEndgameStat;
                    context.GameStats.Add(endgameStat);
                }
                else
                {
                    endgameStat.Update(newEndgameStat);
                }
            }

            context.SaveChanges();
            context.Dispose();
        }

        [JSInvokable]
        public void SaveGame(
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
            if (G.Log) Console.WriteLine("Index.SaveGame");

            //var decodedRatios = GameDecoder.DecodeRatios(encodedRatios);
            //var decodedSuggestions = GameDecoder.DecodeSuggestions(encodedSuggestions);
            //var decodedMoveTypes = GameDecoder.DecodeMoveTypes(encodedMoveTypes);
            //var decodedChosenNotPlayedCoords = GameDecoder.DecodeChosenNotPlayedCoords(encodedChosenNotPlayedCoords);

            context = dbService.GetContext();

            Game newGame = new()
            {
                UserId = user.Id,
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

            if (game == null)
            {
                game = newGame;
                context.Games.Add(game);
            }
            else
            {
                game.Update(newGame);
            }

            context.SaveChanges();
            context.Dispose();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ClaimsPrincipal userClaimsPrincipal = (await authenticationStateTask).User;
                if (userClaimsPrincipal.Identity.IsAuthenticated)
                {
                    user = await userManager.GetUserAsync(userClaimsPrincipal);
                }

                if (user == null)
                {
                    return;
                }

                trainerRef = DotNetObjectReference.Create(this);
                cKataGoWrapperRef = DotNetObjectReference.Create(cKataGoWrapper);

                Start();
            }
        }

        public void Dispose()
        {
            trainerRef?.Dispose();
            cKataGoWrapperRef?.Dispose();
        }
    }
}
