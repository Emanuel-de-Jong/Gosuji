using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Services.Trainer;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services.TrainerService
{
    public class TrainerService : IAsyncDisposable
    {
        public string UserId { get; set; }
        private KataGoPool pool;
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        public Game Game { get; set; }
        public MoveTree MoveTree { get; set; } = new();
        public Subscription? Subscription { get; set; }

        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public KataGo? KataGo { get; set; }

        private Random rnd = new();
        private bool isAnalyzing = false;
        private bool shouldBeImperfectSuggestion = false;

        public TrainerService(string userId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            UserId = userId;
            pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;
            Game = new();
        }

        public async Task SetSubscription()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(s => s.UserId == UserId);
            await dbContext.DisposeAsync();
        }

        private async Task StartKataGo()
        {
            await (await GetKataGo()).Restart();
            (await GetKataGo()).SetBoardsize(TrainerSettingConfig.Boardsize);
            (await GetKataGo()).SetRuleset(TrainerSettingConfig.GetRuleset);
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).SetKomi(TrainerSettingConfig.GetKomi);
        }

        public async Task<bool> UserHasInstance()
        {
            return pool.UserHasInstance(UserId);
        }

        public async Task Init(TrainerSettingConfig trainerSettingConfig, bool isThirdPartySGF)
        {
            TrainerSettingConfig = trainerSettingConfig;
            Game.IsThirdPartySGF = isThirdPartySGF;

            TrainerSettingConfig.SubscriptionType = Subscription?.SubscriptionType;

            await StartKataGo();
        }

        public async Task UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            TrainerSettingConfig = trainerSettingConfig;
            TrainerSettingConfig.SubscriptionType = Subscription?.SubscriptionType;
        }

        public async Task SyncBoard(Move[] moves)
        {
            (await GetKataGo()).ClearBoard();
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).PlayRange(moves);

            MoveTree.CurrentNode = MoveTree.RootNode;
            foreach (Move move in moves)
            {
                MoveTree.Add(move);
            }
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            MoveSuggestion suggestion = (await GetKataGo()).AnalyzeMove(move);

            MoveTree.CurrentNode.Suggestions ??= new MoveSuggestionList();
            MoveTree.CurrentNode.Suggestions.AnalyzeMoveSuggestion = suggestion;

            isAnalyzing = false;
            return suggestion;
        }

        public async Task<AnalyzeResponse> Analyze(EMoveType moveType, EMoveColor color, bool isMainBranch)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            int maxVisits = 0;
            double minVisitsPerc = 0;
            double maxVisitDiffPerc = 100;
            int moveOptions = 1;
            switch (moveType)
            {
                case EMoveType.PLAYER:
                    maxVisits = TrainerSettingConfig.GetSuggestionVisits;
                    minVisitsPerc = TrainerSettingConfig.MinVisitsPercSwitch ? TrainerSettingConfig.MinVisitsPerc : minVisitsPerc;
                    maxVisitDiffPerc = TrainerSettingConfig.MaxVisitDiffPercSwitch ? TrainerSettingConfig.MaxVisitDiffPerc : maxVisitDiffPerc;
                    moveOptions = TrainerSettingConfig.SuggestionOptions;
                    break;
                case EMoveType.OPPONENT:
                    maxVisits = TrainerSettingConfig.GetOpponentVisits;
                    minVisitsPerc = 10;
                    maxVisitDiffPerc = 50;
                    moveOptions = TrainerSettingConfig.OpponentOptions;
                    break;
                case EMoveType.PRE:
                    maxVisits = TrainerSettingConfig.GetPreVisits;
                    minVisitsPerc = 10;
                    maxVisitDiffPerc = 50;
                    moveOptions = TrainerSettingConfig.PreOptions;
                    break;
                case EMoveType.SELFPLAY:
                    maxVisits = TrainerSettingConfig.GetSelfplayVisits;
                    break;
            }

            MoveSuggestionList suggestions = (await GetKataGo()).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc, moveOptions);
            MoveTree.CurrentNode.Suggestions = suggestions;

            double? result = GetResult(suggestions);
            MoveTree.CurrentNode.Result = result;

            if (result == null)
            {
                CalcPlayIndex(suggestions, moveType);

                if (suggestions.PlayIndex != null)
                {
                    Move move = new(color, suggestions.Suggestions[suggestions.PlayIndex.Value].Coord);
                    MoveTree.Add(move);
                    MoveTree.CurrentNode.MoveType = moveType;

                    (await GetKataGo()).Play(move);
                }
            }
            else
            {
                MoveTree.Add(Move.PASS);
                MoveTree.CurrentNode.MoveType = EMoveType.PASS;
            }

            MoveTree.MainBranch = isMainBranch ? MoveTree.CurrentNode : MoveTree.MainBranch;

            isAnalyzing = false;
            return new AnalyzeResponse(suggestions, result);
        }

        public async Task<AnalyzeResponse> AnalyzeAfterJump(Move[] moves, EMoveType moveType, EMoveColor color, bool isMainBranch)
        {
            await SyncBoard(moves);
            return await Analyze(moveType, color, isMainBranch);
        }

        private double? GetResult(MoveSuggestionList moveSuggestionList)
        {
            MoveSuggestion? passSuggestion = moveSuggestionList.PassSuggestion;
            if (passSuggestion == null)
            {
                return null;
            }

            double scoreLead = passSuggestion.Score.ScoreLead;
            // Round to nearest x.0 or x.5
            scoreLead = Math.Round(scoreLead * 2, MidpointRounding.AwayFromZero) / 2;
            return Math.Round(scoreLead, 1);
        }

        private void CalcPlayIndex(MoveSuggestionList suggestions, EMoveType moveType)
        {
            if (moveType == EMoveType.PLAYER)
            {
                return;
            }

            int playIndex = 0;
            if (moveType is EMoveType.OPPONENT or EMoveType.PRE)
            {
                if (!shouldBeImperfectSuggestion)
                {
                    if ((moveType == EMoveType.OPPONENT &&
                        TrainerSettingConfig.OpponentOptionPercSwitch &&
                        rnd.Next(1, 101) <= TrainerSettingConfig.OpponentOptionPerc)
                        ||
                        (moveType == EMoveType.PRE &&
                        TrainerSettingConfig.PreOptionPercSwitch &&
                        rnd.Next(1, 101) <= TrainerSettingConfig.PreOptionPerc))
                    {
                        shouldBeImperfectSuggestion = true;
                    }
                }

                if (shouldBeImperfectSuggestion)
                {
                    List<MoveSuggestion> imperfectSuggestions = suggestions.Suggestions.Where(s => s.Grade != "A").ToList();

                    if (imperfectSuggestions.Count != 0)
                    {
                        shouldBeImperfectSuggestion = false;

                        MoveSuggestion suggestion = imperfectSuggestions[rnd.Next(imperfectSuggestions.Count)];
                        for (int i = 0; i < suggestions.Suggestions.Count; i++)
                        {
                            if (suggestions.Suggestions[i] == suggestion)
                            {
                                playIndex = i;
                                break;
                            }
                        }
                    }
                }
            }

            suggestions.PlayIndex = playIndex;
        }

        private async Task Play(Move move, EMoveType moveType)
        {
            MoveTree.Add(move);
            MoveTree.CurrentNode.MoveType = moveType;

            if (MoveTree.MainBranch == MoveTree.CurrentNode.Parent)
            {
                MoveTree.MainBranch = MoveTree.CurrentNode;
            }

            (await GetKataGo()).Play(move);
        }

        public async Task PlayPlayer(Move move, EPlayerResult playerResult, Coord? chosenNotPlayedCoord,
            int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            await Play(move, EMoveType.PLAYER);

            MoveTree.CurrentNode.PlayerResult = playerResult;
            MoveTree.CurrentNode.Parent.ChosenNotPlayedCoord = chosenNotPlayedCoord;

            Game.RightStreak = rightStreak;
            Game.PerfectStreak = perfectStreak;
            Game.RightTopStreak = rightTopStreak != null ? rightTopStreak.Value : Game.RightTopStreak;
            Game.PerfectTopStreak = perfectTopStreak != null ? perfectTopStreak.Value : Game.PerfectTopStreak;
        }

        public async Task<MoveSuggestion> PlayForcedCorner(Move move)
        {
            MoveSuggestion suggestion = await AnalyzeMove(move);
            await Play(move, EMoveType.FORCED_CORNER);
            return suggestion;
        }

        private async Task<KataGo> GetKataGo()
        {
            if (KataGo == null || KataGo.IsPaused)
            {
                KataGo = await pool.Get(UserId);
                await StartKataGo();
                // TODO: Play till the current move
            }

            return KataGo;
        }

        public async ValueTask DisposeAsync()
        {
            //await Save();
            await pool.Return(UserId);
        }
    }
}
