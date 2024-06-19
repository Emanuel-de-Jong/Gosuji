using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services;
using Gosuji.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gosuji.MVC
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class KataGoController : ControllerBase, IKataGoService
    {
        private SanitizeService sanitizeService;
        private KataGoPoolService pool;

        public KataGoController(SanitizeService _sanitizeService, KataGoPoolService kataGoPoolService)
        {
            sanitizeService = _sanitizeService;
            pool = kataGoPoolService;
        }

        [HttpGet]
        public async Task<KataGoVersion> GetVersion()
        {
            return await pool.GetVersion();
        }

        [HttpGet("{userId}")]
        public async Task Return(string userId)
        {
            await pool.Return(userId);
        }

        [HttpGet("{userId}")]
        public async Task<bool> UserHasInstance(string userId)
        {
            return pool.UserHasInstance(userId);
        }

        [HttpGet("{userId}")]
        public async Task ClearBoard(string userId)
        {
            (await pool.Get(userId)).ClearBoard();
        }

        [HttpGet("{userId}")]
        public async Task Restart(string userId)
        {
            await (await pool.Get(userId)).Restart();
        }

        [HttpGet("{userId}/{boardsize}")]
        public async Task SetBoardsize(string userId, int boardsize)
        {
            (await pool.Get(userId)).SetBoardsize(boardsize);
        }

        [HttpGet("{userId}/{ruleset}")]
        public async Task SetRuleset(string userId, string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(userId)).SetRuleset(ruleset);
        }

        [HttpGet("{userId}/{komi}")]
        public async Task SetKomi(string userId, float komi)
        {
            (await pool.Get(userId)).SetKomi(komi);
        }

        [HttpGet("{userId}/{handicap}")]
        public async Task SetHandicap(string userId, int handicap)
        {
            (await pool.Get(userId)).SetHandicap(handicap);
        }

        [HttpGet("{userId}/{color}/{coord}")]
        public async Task<MoveSuggestion> AnalyzeMove(string userId, string color, string coord)
        {
            color = sanitizeService.Sanitize(color);
            coord = sanitizeService.Sanitize(coord);
            return (await pool.Get(userId)).AnalyzeMove(color, coord);
        }

        [HttpGet("{userId}/{color}")]
        public async Task<List<MoveSuggestion>> Analyze(string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc)
        {
            color = sanitizeService.Sanitize(color);
            return (await pool.Get(userId)).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
        }

        [HttpGet("{userId}/{color}/{coord}")]
        public async Task Play(string userId, string color, string coord)
        {
            color = sanitizeService.Sanitize(color);
            coord = sanitizeService.Sanitize(coord);
            (await pool.Get(userId)).Play(color, coord);
        }

        [HttpPost("{userId}")]
        public async Task PlayRange(string userId, Moves moves)
        {
            sanitizeService.Sanitize(moves);

            (await pool.Get(userId)).PlayRange(moves);
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}
