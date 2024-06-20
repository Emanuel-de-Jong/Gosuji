using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Gosuji.Client.Services
{
    public class KataGoService(HttpClient http)
    {
        private static string MAP_GROUP = "/api/KataGo";

        public async Task<KataGoVersion?> GetVersion()
        {
            return await http.GetFromJsonAsync<KataGoVersion>($"{MAP_GROUP}/GetVersion");
        }

        public async Task Return(string userId)
        {
            await http.GetAsync($"{MAP_GROUP}/Return/{userId}");
        }

        public async Task<bool?> UserHasInstance(string userId)
        {
            return bool.Parse(await (await http.GetAsync($"{MAP_GROUP}/UserHasInstance/{userId}")).Content.ReadAsStringAsync());
        }

        [JSInvokable]
        public async Task ClearBoard(string userId)
        {
            await http.GetAsync($"{MAP_GROUP}/ClearBoard/{userId}");
        }

        [JSInvokable]
        public async Task Restart(string userId)
        {
            await http.GetAsync($"{MAP_GROUP}/Restart/{userId}");
        }

        [JSInvokable]
        public async Task SetBoardsize(string userId, int boardsize)
        {
            await http.GetAsync($"{MAP_GROUP}/SetBoardsize/{userId}/{boardsize}");
        }

        [JSInvokable]
        public async Task SetRuleset(string userId, string ruleset)
        {
            await http.GetAsync($"{MAP_GROUP}/SetRuleset/{userId}/{ruleset}");
        }

        [JSInvokable]
        public async Task SetKomi(string userId, float komi)
        {
            await http.GetAsync($"{MAP_GROUP}/SetKomi/{userId}/{komi}");
        }

        [JSInvokable]
        public async Task SetHandicap(string userId, int handicap)
        {
            await http.GetAsync($"{MAP_GROUP}/SetHandicap/{userId}/{handicap}");
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> AnalyzeMove(string userId, string color, string coord)
        {
            return await http.GetFromJsonAsync<MoveSuggestion>($"{MAP_GROUP}/AnalyzeMove/{userId}/{color}/{coord}");
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc)
        {
            return await http.GetFromJsonAsync<List<MoveSuggestion>>($"{MAP_GROUP}/Analyze/{userId}/{color}" +
                $"?maxVisits={maxVisits}" +
                $"&minVisitsPerc={minVisitsPerc}" +
                $"&minVisitsPerc={maxVisitDiffPerc}");
        }

        [JSInvokable]
        public async Task Play(string userId, string color, string coord)
        {
            await http.GetAsync($"{MAP_GROUP}/Play/{userId}/{color}/{coord}");
        }

        [JSInvokable]
        public async Task PlayRange(string userId, Moves moves)
        {
            await http.PostAsJsonAsync($"{MAP_GROUP}/PlayRange/{userId}", moves);
        }
    }
}
