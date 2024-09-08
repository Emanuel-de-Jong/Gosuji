using Gosuji.API.Services;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.KataGo;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    [Authorize]
    public class TrainerHub : CustomHubBase
    {
        private static ConcurrentDictionary<string, TrainerInstance> trainerServices = new();

        private SanitizeService sanitizeService;
        private KataGoPoolService pool;

        public TrainerHub(SanitizeService _sanitizeService, KataGoPoolService kataGoPoolService)
        {
            sanitizeService = _sanitizeService;
            pool = kataGoPoolService;
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var service = new TrainerInstance(connectionId);
            trainerServices.TryAdd(connectionId, service);

            return base.OnConnectedAsync();
        }

        public async Task<HubResponse> GetVersion()
        {
            return OkData(await pool.GetVersion());
        }

        public async Task<HubResponse> Return()
        {
            await pool.Return(GetUserId());
            return Ok;
        }

        public async Task<HubResponse> UserHasInstance()
        {
            return OkData(pool.UserHasInstance(GetUserId()));
        }

        public async Task<HubResponse> ClearBoard()
        {
            (await pool.Get(GetUserId())).ClearBoard();
            return Ok;
        }

        public async Task<HubResponse> Restart()
        {
            await (await pool.Get(GetUserId())).Restart();
            return Ok;
        }

        public async Task<HubResponse> SetBoardsize([RegularExpression("^(9|13|19)$")] int boardsize)
        {
            (await pool.Get(GetUserId())).SetBoardsize(boardsize);
            return Ok;
        }

        public async Task<HubResponse> SetRuleset(string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(GetUserId())).SetRuleset(ruleset);
            return Ok;
        }

        public async Task<HubResponse> SetKomi([Range(-150, 150)] double komi)
        {
            (await pool.Get(GetUserId())).SetKomi(komi);
            return Ok;
        }

        public async Task<HubResponse> SetHandicap([Range(0, 9)] int handicap)
        {
            (await pool.Get(GetUserId())).SetHandicap(handicap);
            return Ok;
        }

        public async Task<HubResponse> AnalyzeMove(Move move)
        {
            return OkData((await pool.Get(GetUserId())).AnalyzeMove(move));
        }

        public async Task<HubResponse> Analyze(int color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] double minVisitsPerc,
            [Required, Range(0, 100)] double maxVisitDiffPerc)
        {
            return OkData((await pool.Get(GetUserId())).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        public async Task<HubResponse> Play(Move move)
        {
            (await pool.Get(GetUserId())).Play(move);
            return Ok;
        }

        public async Task<HubResponse> PlayRange(Moves moves)
        {
            (await pool.Get(GetUserId())).PlayRange(moves);
            return Ok;
        }
    }
}
