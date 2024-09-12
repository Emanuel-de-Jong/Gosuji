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
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public NullableTrainerSettings? NullableTrainerSettings { get; set; }
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

        private async Task StartKataGo()
        {
            await (await GetKataGo()).Restart();
            (await GetKataGo()).SetBoardsize(TrainerSettingConfig.Boardsize);
            (await GetKataGo()).SetRuleset(NullableTrainerSettings.Ruleset);
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).SetKomi(NullableTrainerSettings.Komi);
        }

        public async Task Return()
        {
            await pool.Return(UserId);
        }

        public async Task<bool> UserHasInstance()
        {
            return pool.UserHasInstance(UserId);
        }

        public async Task Init(TrainerSettingConfig trainerSettingConfig, NullableTrainerSettings nullableTrainerSettings,
            bool isThirdPartySGF)
        {
            TrainerSettingConfig = trainerSettingConfig;
            NullableTrainerSettings = nullableTrainerSettings;
            Game.IsThirdPartySGF = isThirdPartySGF;

            await StartKataGo();
        }

        public async Task ClearBoard()
        {
            (await GetKataGo()).ClearBoard();
        }

        public async Task Restart()
        {
            await (await GetKataGo()).Restart();
        }

        public async Task SetBoardsize(int boardsize)
        {
            (await GetKataGo()).SetBoardsize(boardsize);
        }

        public async Task SetRuleset(string ruleset)
        {
            (await GetKataGo()).SetRuleset(ruleset);
        }

        public async Task SetKomi(double komi)
        {
            (await GetKataGo()).SetKomi(komi);
        }

        public async Task SetHandicap(int handicap)
        {
            (await GetKataGo()).SetHandicap(handicap);
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            if (isAnalyzing)
            {
                return null;
            }

            isAnalyzing = true;
            MoveSuggestion suggestion = (await GetKataGo()).AnalyzeMove(move);
            isAnalyzing = false;
            return suggestion;
        }

        public async Task<MoveSuggestionList> Analyze(EMoveColor color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            if (isAnalyzing)
            {
                return null;
            }

            isAnalyzing = true;
            MoveSuggestionList suggestions = (await GetKataGo()).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
            isAnalyzing = false;
            return suggestions;
        }

        public async Task Play(Move move)
        {
            (await GetKataGo()).Play(move);
        }

        public async Task PlayRange(Move[] moves)
        {
            (await GetKataGo()).PlayRange(moves);
        }

        private async Task<KataGo> GetKataGo()
        {
            if (KataGo == null || KataGo.IsPaused)
            {
                KataGo = await pool.Get(UserId);
                await StartKataGo();
            }

            return KataGo;
        }

        public async ValueTask DisposeAsync()
        {
            //await Save();
        }
    }
}
