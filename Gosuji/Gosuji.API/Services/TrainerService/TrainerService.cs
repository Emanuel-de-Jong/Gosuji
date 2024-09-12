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

        public Game? Game { get; set; }
        public TrainerSettingConfig? SettingConfig { get; set; }
        public MoveTree MoveTree { get; set; } = new();

        public TrainerService(string userId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            UserId = userId;
            pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;

            Game = new();
        }

        public async Task Return()
        {
            await pool.Return(UserId);
        }

        public async Task<bool> UserHasInstance()
        {
            return pool.UserHasInstance(UserId);
        }

        public async Task Init()
        {
        }

        public async Task ClearBoard()
        {
            (await KataGo()).ClearBoard();
        }

        public async Task Restart()
        {
            await (await KataGo()).Restart();
        }

        public async Task SetBoardsize(int boardsize)
        {
            (await KataGo()).SetBoardsize(boardsize);
        }

        public async Task SetRuleset(string ruleset)
        {
            (await KataGo()).SetRuleset(ruleset);
        }

        public async Task SetKomi(double komi)
        {
            (await KataGo()).SetKomi(komi);
        }

        public async Task SetHandicap(int handicap)
        {
            (await KataGo()).SetHandicap(handicap);
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            return (await KataGo()).AnalyzeMove(move);
        }

        public async Task<MoveSuggestionList> Analyze(EMoveColor color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            return (await KataGo()).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
        }

        public async Task Play(Move move)
        {
            (await KataGo()).Play(move);
        }

        public async Task PlayRange(Move[] moves)
        {
            (await KataGo()).PlayRange(moves);
        }

        private async Task<KataGo> KataGo()
        {
            return await pool.Get(UserId);
        }

        public async ValueTask DisposeAsync()
        {
            //await Save();
        }
    }
}
