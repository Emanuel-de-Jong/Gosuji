using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Gosuji.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    [EnableRateLimiting(G.ControllerRateLimitPolicyName)]
    public class KataGoController : CustomControllerBase
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

        [HttpGet]
        public async Task<ActionResult> Return()
        {
            await pool.Return(GetUserId());
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<bool>> UserHasInstance()
        {
            return Ok(pool.UserHasInstance(GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult> ClearBoard()
        {
            (await pool.Get(GetUserId())).ClearBoard();
            return Ok();
        }

        [HttpGet]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> Restart()
        {
            await (await pool.Get(GetUserId())).Restart();
            return Ok();
        }

        [HttpGet("{boardsize}")]
        public async Task<ActionResult> SetBoardsize([RegularExpression("^(9|13|19)$")] int boardsize)
        {
            (await pool.Get(GetUserId())).SetBoardsize(boardsize);
            return Ok();
        }

        [HttpGet("{ruleset}")]
        public async Task<ActionResult> SetRuleset(string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(GetUserId())).SetRuleset(ruleset);
            return Ok();
        }

        [HttpGet("{komi}")]
        public async Task<ActionResult> SetKomi([Range(-150, 150)] double komi)
        {
            (await pool.Get(GetUserId())).SetKomi(komi);
            return Ok();
        }

        [HttpGet("{handicap}")]
        public async Task<ActionResult> SetHandicap([Range(0, 9)] int handicap)
        {
            (await pool.Get(GetUserId())).SetHandicap(handicap);
            return Ok();
        }

        [HttpGet("{color}/{coord}")]
        public async Task<ActionResult<MoveSuggestion>> AnalyzeMove([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            return Ok((await pool.Get(GetUserId())).AnalyzeMove(color, coord));
        }

        [HttpGet("{color}")]
        public async Task<ActionResult<List<MoveSuggestion>>> Analyze([RegularExpression(@"(B|W)")] string color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] double minVisitsPerc,
            [Required, Range(0, 100)] double maxVisitDiffPerc)
        {
            return Ok((await pool.Get(GetUserId())).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        [HttpGet("{color}/{coord}")]
        public async Task<ActionResult> Play([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            (await pool.Get(GetUserId())).Play(color, coord);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> PlayRange(Moves moves)
        {
            (await pool.Get(GetUserId())).PlayRange(moves);
            return Ok();
        }
    }
}
