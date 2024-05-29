using Gosuji.Client.Models.Josekis;
using System.Net.Http.Json;

namespace Gosuji.Client.Services
{
    public class ClientJosekisService(HttpClient http) : IJosekisService
    {
        private static string MAP_GROUP = "/api/JosekisService";

        public async Task AddSession(int sessionId)
        {
            await http.GetAsync($"{MAP_GROUP}/AddSession/{sessionId}");
        }

        public async Task RemoveSession(int sessionId)
        {
            await http.GetAsync($"{MAP_GROUP}/RemoveSession/{sessionId}");
        }

        public async Task<JosekisNode> Current(int sessionId)
        {
            return await http.GetFromJsonAsync<JosekisNode>($"{MAP_GROUP}/Current/{sessionId}");
        }

        public async Task ToParent(int sessionId)
        {
            await http.GetAsync($"{MAP_GROUP}/AddSession/{sessionId}");
        }

        public async Task<int> ToLastBranch(int sessionId)
        {
            return int.Parse(await (await http.GetAsync($"{MAP_GROUP}/ToLastBranch/{sessionId}")).Content.ReadAsStringAsync());
        }

        public async Task ToFirst(int sessionId)
        {
            await http.GetAsync($"{MAP_GROUP}/ToFirst/{sessionId}");
        }

        public async Task<bool> ToChild(int sessionId, JosekisNode childToGo)
        {
            return bool.Parse(await (await http.PostAsJsonAsync($"{MAP_GROUP}/ToChild/{sessionId}", childToGo)).Content.ReadAsStringAsync());
        }
    }
}
