using Gosuji.Client.Data;
using Gosuji.Client.Helpers;
using Gosuji.Client.Models.KataGo;
using Microsoft.JSInterop;

namespace Gosuji.Client.Services
{
    public class KataGoService(HttpClient http)
    {
        private static string MAP_GROUP = "/api/KataGo";

        public async Task<KataGoVersion?> GetVersion()
        {
            return await HttpResponseHandler.Get<KataGoVersion>(http,
                $"{MAP_GROUP}/GetVersion");
        }

        public async Task<bool> Return()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Return");
        }

        public async Task<bool?> UserHasInstance()
        {
            return await HttpResponseHandler.Get<bool>(http,
                $"{MAP_GROUP}/UserHasInstance");
        }

        [JSInvokable]
        public async Task<bool> ClearBoard()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ClearBoard");
        }

        [JSInvokable]
        public async Task<bool> Restart()
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Restart");
        }

        [JSInvokable]
        public async Task<bool> SetBoardsize(int boardsize)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetBoardsize/{boardsize}");
        }

        [JSInvokable]
        public async Task<bool> SetRuleset(string ruleset)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetRuleset/{ruleset}");
        }

        [JSInvokable]
        public async Task<bool> SetKomi(float komi)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetKomi/{komi}");
        }

        [JSInvokable]
        public async Task<bool> SetHandicap(int handicap)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetHandicap/{handicap}");
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> AnalyzeMove(string color, string coord)
        {
            return await HttpResponseHandler.Get<MoveSuggestion>(http,
                $"{MAP_GROUP}/AnalyzeMove/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc)
        {
            return await HttpResponseHandler.Get<List<MoveSuggestion>>(http,
                $"{MAP_GROUP}/Analyze/{color}" +
                $"?maxVisits={maxVisits}" +
                $"&minVisitsPerc={minVisitsPerc}" +
                $"&maxVisitDiffPerc={maxVisitDiffPerc}");
        }

        [JSInvokable]
        public async Task<bool> Play(string color, string coord)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Play/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<bool> PlayRange(Moves moves)
        {
            return await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PlayRange", moves);
        }
    }
}
