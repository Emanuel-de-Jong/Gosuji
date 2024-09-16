using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client.Data;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services.TrainerService
{
    public class TrainerService : IAsyncDisposable
    {
        private KataGoPool pool;
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        public string UserId { get; set; }

        public Game Game { get; set; }
        public Subscription? Subscription { get; set; }

        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public KataGo? KataGo { get; set; }
        public MoveTree MoveTree { get; set; } = new();

        private bool isAnalyzing = false;

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

        public async Task<MoveSuggestionList> Analyze(EMoveType moveType, EMoveColor color)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            MoveTree.CurrentNode.MoveType = moveType;

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

            isAnalyzing = false;
            return suggestions;
        }

        public async Task Play(Move move, int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            MoveTree.Add(move);

            Game.RightStreak = rightStreak;
            Game.PerfectStreak = perfectStreak;
            Game.RightTopStreak = rightTopStreak != null ? rightTopStreak.Value : Game.RightTopStreak;
            Game.PerfectTopStreak = perfectTopStreak != null ? perfectTopStreak.Value : Game.PerfectTopStreak;

            (await GetKataGo()).Play(move);
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
