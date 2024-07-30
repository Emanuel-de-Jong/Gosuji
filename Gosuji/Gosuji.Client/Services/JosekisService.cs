using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.Josekis;

namespace Gosuji.Client.Services
{
    public class JosekisService
    {
        private static string MAP_GROUP = "/api/Josekis";

        private HttpClient http;

        public JosekisService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("Auth");
        }

        public async Task<bool> AddSession(int sessionId)
        {
            return (await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/AddSession/{sessionId}")).IsSuccess;
        }

        public async Task<bool> RemoveSession(int sessionId)
        {
            return (await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/RemoveSession/{sessionId}")).IsSuccess;
        }

        public async Task<JosekisNode?> Current(int sessionId)
        {
            return (await HttpResponseHandler.Get<JosekisNode>(http,
                $"{MAP_GROUP}/Current/{sessionId}")).Data;
        }

        public async Task<bool> ToParent(int sessionId)
        {
            return (await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ToParent/{sessionId}")).IsSuccess;
        }

        public async Task<int?> ToLastBranch(int sessionId)
        {
            return (await HttpResponseHandler.Get<int>(http,
                $"{MAP_GROUP}/ToLastBranch/{sessionId}")).Data;
        }

        public async Task<bool> ToFirst(int sessionId)
        {
            return (await HttpResponseHandler.Get(http,
                $"{MAP_GROUP}/ToFirst/{sessionId}")).IsSuccess;
        }

        public async Task<bool?> ToChild(int sessionId, JosekisNode childToGo)
        {
            return (await HttpResponseHandler.Post<bool>(http,
                $"{MAP_GROUP}/ToChild/{sessionId}", childToGo)).Data;
        }
    }
}
