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

        public async Task<bool> Return(string userId)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Return/{userId}");
        }

        public async Task<bool?> UserHasInstance(string userId)
        {
            return await HttpResponseHandler.Get<bool>(http,
                $"{MAP_GROUP}/UserHasInstance/{userId}");
        }

        [JSInvokable]
        public async Task<bool> ClearBoard(string userId)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ClearBoard/{userId}");
        }

        [JSInvokable]
        public async Task<bool> Restart(string userId)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Restart/{userId}");
        }

        [JSInvokable]
        public async Task<bool> SetBoardsize(string userId, int boardsize)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetBoardsize/{userId}/{boardsize}");
        }

        [JSInvokable]
        public async Task<bool> SetRuleset(string userId, string ruleset)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetRuleset/{userId}/{ruleset}");
        }

        [JSInvokable]
        public async Task<bool> SetKomi(string userId, float komi)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetKomi/{userId}/{komi}");
        }

        [JSInvokable]
        public async Task<bool> SetHandicap(string userId, int handicap)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetHandicap/{userId}/{handicap}");
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> AnalyzeMove(string userId, string color, string coord)
        {
            return await HttpResponseHandler.Get<MoveSuggestion>(http,
                $"{MAP_GROUP}/AnalyzeMove/{userId}/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc)
        {
            return await HttpResponseHandler.Get<List<MoveSuggestion>>(http,
                $"{MAP_GROUP}/Analyze/{userId}/{color}" +
                $"?maxVisits={maxVisits}" +
                $"&minVisitsPerc={minVisitsPerc}" +
                $"&minVisitsPerc={maxVisitDiffPerc}");
        }

        [JSInvokable]
        public async Task<bool> Play(string userId, string color, string coord)
        {
            return await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Play/{userId}/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<bool> PlayRange(string userId, Moves moves)
        {
            return await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PlayRange/{userId}", moves);
        }
    }
}
