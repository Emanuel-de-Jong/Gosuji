using Gosuji.API.Data;
using Gosuji.API.Services;
using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    [Authorize]
    public class TrainerHub : CustomHubBase
    {
        private static ConcurrentDictionary<string, TrainerService> trainerServices = new();
        private static IDbContextFactory<ApplicationDbContext>? dbContextFactory;

        private SanitizeService sanitizeService;
        private KataGoPool pool;

        public TrainerHub(SanitizeService sanitizeService, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            this.sanitizeService = sanitizeService;
            pool = kataGoPool;

            TrainerHub.dbContextFactory ??= dbContextFactory;
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            TrainerService service = new(GetUserId(), pool, dbContextFactory);
            trainerServices.TryAdd(connectionId, service);

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            if (trainerServices.TryRemove(connectionId, out TrainerService service))
            {
                await service.DisposeAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<HubResponse> GetVersion()
        {
            return OkData(await trainerServices[Context.ConnectionId].GetVersion());
        }

        public async Task<HubResponse> Return()
        {
            await trainerServices[Context.ConnectionId].Return();
            return Ok;
        }

        public async Task<HubResponse> UserHasInstance()
        {
            return OkData(trainerServices[Context.ConnectionId].UserHasInstance());
        }

        public async Task<HubResponse> ClearBoard()
        {
            await trainerServices[Context.ConnectionId].ClearBoard();
            return Ok;
        }

        public async Task<HubResponse> Restart()
        {
            await trainerServices[Context.ConnectionId].Restart();
            return Ok;
        }

        public async Task<HubResponse> SetBoardsize([RegularExpression("^(9|13|19)$")] int boardsize)
        {
            await trainerServices[Context.ConnectionId].SetBoardsize(boardsize);
            return Ok;
        }

        public async Task<HubResponse> SetRuleset(string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            await trainerServices[Context.ConnectionId].SetRuleset(ruleset);
            return Ok;
        }

        public async Task<HubResponse> SetKomi([Range(-150, 150)] double komi)
        {
            await trainerServices[Context.ConnectionId].SetKomi(komi);
            return Ok;
        }

        public async Task<HubResponse> SetHandicap([Range(0, 9)] int handicap)
        {
            await trainerServices[Context.ConnectionId].SetHandicap(handicap);
            return Ok;
        }

        public async Task<HubResponse> AnalyzeMove(Move move)
        {
            return OkData(trainerServices[Context.ConnectionId].AnalyzeMove(move));
        }

        public async Task<HubResponse> Analyze(EMoveColor color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] double minVisitsPerc,
            [Required, Range(0, 100)] double maxVisitDiffPerc)
        {
            return OkData(trainerServices[Context.ConnectionId].Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        public async Task<HubResponse> Play(Move move)
        {
            await trainerServices[Context.ConnectionId].Play(move);
            return Ok;
        }

        public async Task<HubResponse> PlayRange(Move[] moves)
        {
            await trainerServices[Context.ConnectionId].PlayRange(moves);
            return Ok;
        }
    }
}
