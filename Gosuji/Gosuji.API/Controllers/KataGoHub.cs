using Gosuji.API.Services;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.KataGo;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    public class KataGoHub : CustomHubBase
    {
        private SanitizeService sanitizeService;
        private KataGoPoolService pool;

        public KataGoHub(SanitizeService _sanitizeService, KataGoPoolService kataGoPoolService)
        {
            sanitizeService = _sanitizeService;
            pool = kataGoPoolService;
        }

        public async Task<HubResponse<KataGoVersion>> GetVersion()
        {
            return Ok(await pool.GetVersion());
        }

        public async Task<HubResponse> Return()
        {
            await pool.Return(GetUserId());
            return Ok();
        }

        public async Task<HubResponse<bool>> UserHasInstance()
        {
            return Ok(pool.UserHasInstance(GetUserId()));
        }

        public async Task<HubResponse> ClearBoard()
        {
            (await pool.Get(GetUserId())).ClearBoard();
            return Ok();
        }

        public async Task<HubResponse> Restart()
        {
            await (await pool.Get(GetUserId())).Restart();
            return Ok();
        }

        public async Task<HubResponse> SetBoardsize([RegularExpression("^(9|13|19)$")] int boardsize)
        {
            (await pool.Get(GetUserId())).SetBoardsize(boardsize);
            return Ok();
        }

        public async Task<HubResponse> SetRuleset(string ruleset)
        {
            ruleset = sanitizeService.Sanitize(ruleset);
            (await pool.Get(GetUserId())).SetRuleset(ruleset);
            return Ok();
        }

        public async Task<HubResponse> SetKomi([Range(-150, 150)] double komi)
        {
            (await pool.Get(GetUserId())).SetKomi(komi);
            return Ok();
        }

        public async Task<HubResponse> SetHandicap([Range(0, 9)] int handicap)
        {
            (await pool.Get(GetUserId())).SetHandicap(handicap);
            return Ok();
        }

        public async Task<HubResponse<MoveSuggestion>> AnalyzeMove([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            return Ok((await pool.Get(GetUserId())).AnalyzeMove(color, coord));
        }

        public async Task<HubResponse<List<MoveSuggestion>>> Analyze([RegularExpression(@"(B|W)")] string color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] double minVisitsPerc,
            [Required, Range(0, 100)] double maxVisitDiffPerc)
        {
            return Ok((await pool.Get(GetUserId())).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        public async Task<HubResponse> Play([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            (await pool.Get(GetUserId())).Play(color, coord);
            return Ok();
        }

        public async Task<HubResponse> PlayRange(Moves moves)
        {
            (await pool.Get(GetUserId())).PlayRange(moves);
            return Ok();
        }
    }
}
