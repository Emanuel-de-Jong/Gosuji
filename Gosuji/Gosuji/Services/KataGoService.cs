using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Services
{
    public class KataGoService : IServerService, IKataGoService, IDisposable
    {
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        private KataGoPool pool;
        private KataGoVersion? version;

        public KataGoService(IDbContextFactory<ApplicationDbContext> _dbContextFactory)
        {
            dbContextFactory = _dbContextFactory;

            pool = new(dbContextFactory);
        }

        public static void CreateEndpoints(WebApplication app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/KataGoService");
            group.MapGet("/GetVersion", (IKataGoService service) => service.GetVersion());
            group.MapGet("/Return/{userId}", (string userId, IKataGoService service) => service.Return(userId));
            group.MapGet("/UserHasInstance/{userId}", (string userId, IKataGoService service) => service.UserHasInstance(userId));
            group.MapGet("/ClearBoard/{userId}", (string userId, IKataGoService service) => service.ClearBoard(userId));
            group.MapGet("/Restart/{userId}", (string userId, IKataGoService service) => service.Restart(userId));
            group.MapGet("/SetBoardsize/{userId}/{boardsize}", (string userId, int boardsize, IKataGoService service) => service.SetBoardsize(userId, boardsize));
            group.MapGet("/SetRuleset/{userId}/{ruleset}", (string userId, string ruleset, IKataGoService service) => service.SetRuleset(userId, ruleset));
            group.MapGet("/SetKomi/{userId}/{komi}", (string userId, float komi, IKataGoService service) => service.SetKomi(userId, komi));
            group.MapGet("/SetHandicap/{userId}/{handicap}", (string userId, int handicap, IKataGoService service) => service.SetHandicap(userId, handicap));
            group.MapGet("/AnalyzeMove/{userId}/{color}/{coord}", (string userId, string color, string coord, IKataGoService service) => service.AnalyzeMove(userId, color, coord));
            group.MapGet("/Analyze/{userId}/{color}/{maxVisits}/{minVisitsPerc}/{maxVisitDiffPerc}",
                (string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc, IKataGoService service) =>
                service.Analyze(userId, color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
            group.MapGet("/Play/{userId}/{color}/{coord}", (string userId, string color, string coord, IKataGoService service) => service.Play(userId, color, coord));
            group.MapPost("/PlayRange/{userId}", (string userId, Moves moves, IKataGoService service) => service.PlayRange(userId, moves));
            group.MapGet("/SGF/{userId}/{shouldWriteFile}", (string userId, bool shouldWriteFile, IKataGoService service) => service.SGF(userId, shouldWriteFile));
        }

        public async Task<KataGoVersion> GetVersion()
        {
            if (version == null)
            {
                ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

                version = await dbContext.KataGoVersions
                    .Where(k => k.Model == KataGoVersion.MODEL)
                    .Where(k => k.Version == KataGoVersion.VERSION)
                    .Where(k => k.Config == KataGoVersion.GetConfig())
                .FirstOrDefaultAsync();

                if (version == null)
                {
                    version = KataGoVersion.GetCurrent();
                    await dbContext.AddAsync(version);
                    await dbContext.SaveChangesAsync();
                }

                await dbContext.DisposeAsync();

                version.Config = "";
            }

            return version;
        }

        public async Task Return(string userId)
        {
            await pool.Return(userId);
        }

        public async Task<bool> UserHasInstance(string userId)
        {
            return pool.UserHasInstance(userId);
        }

        [JSInvokable]
        public async Task ClearBoard(string userId)
        {
            pool.Get(userId).ClearBoard();
        }

        [JSInvokable]
        public async Task Restart(string userId)
        {
            await pool.Get(userId).Restart();
        }

        [JSInvokable]
        public async Task SetBoardsize(string userId, int boardsize)
        {
            pool.Get(userId).SetBoardsize(boardsize);
        }

        [JSInvokable]
        public async Task SetRuleset(string userId, string ruleset)
        {
            pool.Get(userId).SetRuleset(ruleset);
        }

        [JSInvokable]
        public async Task SetKomi(string userId, float komi)
        {
            pool.Get(userId).SetKomi(komi);
        }

        [JSInvokable]
        public async Task SetHandicap(string userId, int handicap)
        {
            pool.Get(userId).SetHandicap(handicap);
        }

        [JSInvokable]
        public async Task<MoveSuggestion> AnalyzeMove(string userId, string color, string coord)
        {
            return pool.Get(userId).AnalyzeMove(color, coord);
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>> Analyze(string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc)
        {
            return pool.Get(userId).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
        }

        [JSInvokable]
        public async Task Play(string userId, string color, string coord)
        {
            pool.Get(userId).Play(color, coord);
        }

        [JSInvokable]
        public async Task PlayRange(string userId, Moves moves)
        {
            pool.Get(userId).PlayRange(moves);
        }

        [JSInvokable]
        public async Task<string> SGF(string userId, bool shouldWriteFile)
        {
            return pool.Get(userId).SGF(shouldWriteFile);
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}
