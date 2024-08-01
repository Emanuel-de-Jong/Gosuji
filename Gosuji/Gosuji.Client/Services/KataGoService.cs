using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.KataGo;
using Microsoft.JSInterop;

namespace Gosuji.Client.Services
{
    public class KataGoService
    {
        private static string MAP_GROUP = "/api/KataGo";

        private HttpClient http;

        public KataGoService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("Auth");
        }

        public async Task<APIResponse<KataGoVersion>> GetVersion()
        {
            return await HttpResponseHandler.Get<KataGoVersion>(http,
                $"{MAP_GROUP}/GetVersion");
        }

        public async Task<APIResponse> Return()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Return");
        }

        public async Task<APIResponse<bool>> UserHasInstance()
        {
            return await HttpResponseHandler.Get<bool>(http,
                $"{MAP_GROUP}/UserHasInstance");
        }

        [JSInvokable]
        public async Task<APIResponse> ClearBoard()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ClearBoard");
        }

        [JSInvokable]
        public async Task<APIResponse> Restart()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Restart");
        }

        [JSInvokable]
        public async Task<APIResponse> SetBoardsize(int boardsize)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetBoardsize/{boardsize}");
        }

        [JSInvokable]
        public async Task<APIResponse> SetRuleset(string ruleset)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetRuleset/{ruleset}");
        }

        [JSInvokable]
        public async Task<APIResponse> SetKomi(double komi)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetKomi/{komi}");
        }

        [JSInvokable]
        public async Task<APIResponse> SetHandicap(int handicap)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetHandicap/{handicap}");
        }

        [JSInvokable]
        public async Task<APIResponse<MoveSuggestion>> AnalyzeMove(string color, string coord)
        {
            return await HttpResponseHandler.Get<MoveSuggestion>(http,
                $"{MAP_GROUP}/AnalyzeMove/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<APIResponse<List<MoveSuggestion>>> Analyze(string color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            return await HttpResponseHandler.Get<List<MoveSuggestion>>(http,
                $"{MAP_GROUP}/Analyze/{color}" +
                $"?maxVisits={maxVisits}" +
                $"&minVisitsPerc={minVisitsPerc}" +
                $"&maxVisitDiffPerc={maxVisitDiffPerc}");
        }

        [JSInvokable]
        public async Task<APIResponse> Play(string color, string coord)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Play/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<APIResponse> PlayRange(Moves moves)
        {
            return await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PlayRange", moves);
        }
    }
}
