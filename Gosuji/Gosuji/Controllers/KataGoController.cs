using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services;
using Gosuji.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    [EnableRateLimiting(G.ControllerRateLimitPolicyName)]
    public class KataGoController : ControllerBase
    {
        private SanitizeService sanitizeService;
        private KataGoPoolService pool;

        public KataGoController(SanitizeService _sanitizeService, KataGoPoolService kataGoPoolService)
        {
            sanitizeService = _sanitizeService;
            pool = kataGoPoolService;
        }

        [HttpGet]
        public async Task<ActionResult<KataGoVersion>> GetVersion()
        {
            return Ok(await pool.GetVersion());
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> Return(string userId)
        {
            await pool.Return(userId);
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<bool>> UserHasInstance(string userId)
        {
            return Ok(pool.UserHasInstance(userId));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> ClearBoard(string userId)
        {
            (await pool.Get(userId)).ClearBoard();
            return Ok();
        }

        [HttpGet("{userId}")]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> Restart(string userId)
        {
            await (await pool.Get(userId)).Restart();
            return Ok();
        }

        [HttpGet("{userId}/{boardsize}")]
        public async Task<ActionResult> SetBoardsize(string userId, [RegularExpression("^(9|13|19)$")] int boardsize)
        {
            (await pool.Get(userId)).SetBoardsize(boardsize);
            return Ok();
        }

        [HttpGet("{userId}/{ruleset}")]
        public async Task<ActionResult> SetRuleset(string userId, string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(userId)).SetRuleset(ruleset);
            return Ok();
        }

        [HttpGet("{userId}/{komi}")]
        public async Task<ActionResult> SetKomi(string userId, [Range(-150, 150)] float komi)
        {
            (await pool.Get(userId)).SetKomi(komi);
            return Ok();
        }

        [HttpGet("{userId}/{handicap}")]
        public async Task<ActionResult> SetHandicap(string userId, [Range(0, 9)] int handicap)
        {
            (await pool.Get(userId)).SetHandicap(handicap);
            return Ok();
        }

        [HttpGet("{userId}/{color}/{coord}")]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<MoveSuggestion>> AnalyzeMove(string userId, [RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            return Ok((await pool.Get(userId)).AnalyzeMove(color, coord));
        }

        [HttpGet("{userId}/{color}")]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<List<MoveSuggestion>>> Analyze(string userId, [RegularExpression(@"(B|W)")] string color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] float minVisitsPerc,
            [Required, Range(0, 100)] float maxVisitDiffPerc)
        {
            return Ok((await pool.Get(userId)).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        [HttpGet("{userId}/{color}/{coord}")]
        public async Task<ActionResult> Play(string userId, [RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            (await pool.Get(userId)).Play(color, coord);
            return Ok();
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> PlayRange(string userId, Moves moves)
        {
            (await pool.Get(userId)).PlayRange(moves);
            return Ok();
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}
