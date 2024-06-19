using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services;
using Gosuji.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Controllers
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
        public async Task SetBoardsize(string userId, [RegularExpression(@"(9|13|19)")] string boardsize)
        {
            (await pool.Get(userId)).SetBoardsize(int.Parse(boardsize));
        }

        [HttpGet("{userId}/{ruleset}")]
        public async Task SetRuleset(string userId, string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(userId)).SetRuleset(ruleset);
        }

        [HttpGet("{userId}/{komi}")]
        public async Task SetKomi(string userId, [Range(-150, 150)] float komi)
        {
            (await pool.Get(userId)).SetKomi(komi);
        }

        [HttpGet("{userId}/{handicap}")]
        public async Task SetHandicap(string userId, [Range(0, 9)] int handicap)
        {
            (await pool.Get(userId)).SetHandicap(handicap);
        }

        [HttpGet("{userId}/{color}/{coord}")]
        public async Task<MoveSuggestion> AnalyzeMove(string userId, [RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            return (await pool.Get(userId)).AnalyzeMove(color, coord);
        }

        [HttpGet("{userId}/{color}")]
        public async Task<List<MoveSuggestion>> Analyze(string userId, [RegularExpression(@"(B|W)")] string color,
            [Range(2, 100_000)] int maxVisits,
            [Range(0, 100)] float minVisitsPerc,
            [Range(0, 100)] float maxVisitDiffPerc)
        {
            return (await pool.Get(userId)).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
        }

        [HttpGet("{userId}/{color}/{coord}")]
        public async Task Play(string userId, [RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            (await pool.Get(userId)).Play(color, coord);
        }

        [HttpPost("{userId}")]
        public async Task PlayRange(string userId, Moves moves)
        {
            (await pool.Get(userId)).PlayRange(moves);
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}
