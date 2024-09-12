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

        public TrainerService(string userId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            UserId = userId;
            pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;
            Game = new();

            kataGoPool.InstanceReturning += PoolInstanceReturning;
        }

        private async void PoolInstanceReturning(string userId)
        {
            if (userId != UserId)
            {
                return;
            }

            KataGo = null;
            KataGo = await pool.Get(UserId);
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

            KataGo = await pool.Get(UserId);

            await KataGo.Restart();
            KataGo.SetBoardsize(trainerSettingConfig.Boardsize);
            KataGo.SetRuleset(nullableTrainerSettings.Ruleset);
            KataGo.SetHandicap(trainerSettingConfig.Handicap);
            KataGo.SetKomi(nullableTrainerSettings.Komi);
        }

        public async Task ClearBoard()
        {
            KataGo.ClearBoard();
        }

        public async Task Restart()
        {
            await KataGo.Restart();
        }

        public async Task SetBoardsize(int boardsize)
        {
            KataGo.SetBoardsize(boardsize);
        }

        public async Task SetRuleset(string ruleset)
        {
            KataGo.SetRuleset(ruleset);
        }

        public async Task SetKomi(double komi)
        {
            KataGo.SetKomi(komi);
        }

        public async Task SetHandicap(int handicap)
        {
            KataGo.SetHandicap(handicap);
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            return KataGo.AnalyzeMove(move);
        }

        public async Task<MoveSuggestionList> Analyze(EMoveColor color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            return KataGo.Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
        }

        public async Task Play(Move move)
        {
            KataGo.Play(move);
        }

        public async Task PlayRange(Move[] moves)
        {
            KataGo.PlayRange(moves);
        }

        private async Task<KataGo> GetKataGo()
        {
            if (KataGo == null)
            {
                KataGo = await pool.Get(UserId);
            }

            return KataGo;
        }

        public async ValueTask DisposeAsync()
        {
            //await Save();
        }
    }
}
