using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.KataGo;
using Microsoft.JSInterop;
using System.Collections.Generic;

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
        public async Task<bool> ClearBoard()
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ClearBoard");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> Restart()
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Restart");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetBoardsize(int boardsize)
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetBoardsize/{boardsize}");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetRuleset(string ruleset)
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetRuleset/{ruleset}");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetKomi(double komi)
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetKomi/{komi}");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetHandicap(int handicap)
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/SetHandicap/{handicap}");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> AnalyzeMove(string color, string coord)
        {
            APIResponse<MoveSuggestion> response = await HttpResponseHandler.Get<MoveSuggestion>(http,
                $"{MAP_GROUP}/AnalyzeMove/{color}/{coord}");
            if (G.StatusMessage.HandleAPIResponse(response)) return null;
            return response.Data;
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(string color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            APIResponse<List<MoveSuggestion>> response = await HttpResponseHandler.Get<List<MoveSuggestion>>(http,
                $"{MAP_GROUP}/Analyze/{color}" +
                $"?maxVisits={maxVisits}" +
                $"&minVisitsPerc={minVisitsPerc}" +
                $"&maxVisitDiffPerc={maxVisitDiffPerc}");
            if (G.StatusMessage.HandleAPIResponse(response)) return null;
            return response.Data;
        }

        [JSInvokable]
        public async Task<bool> Play(string color, string coord)
        {
            APIResponse response = await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/Play/{color}/{coord}");
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> PlayRange(Moves moves)
        {
            APIResponse response = await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PlayRange", moves);
            return !G.StatusMessage.HandleAPIResponse(response);
        }
    }
}
