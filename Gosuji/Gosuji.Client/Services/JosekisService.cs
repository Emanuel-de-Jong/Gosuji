using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gosuji.Client.Services
{
    public class JosekisService : BaseHubService
    {
        public JosekisService(IConfiguration configuration, UserService userService)
            :base(configuration, userService, "josekishub")
        {
        }

        public async Task<APIResponse<int>> StartSession()
        {
            string uri = "StartSession";
            return await HubResponseHandler.TryCatch<int>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
        }

        public async Task<APIResponse> StopSession(int sessionId)
        {
            string uri = "StopSession";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId));
        }

        public async Task<APIResponse<JosekisNode>> Current(int sessionId)
        {
            string uri = "Current";
            return await HubResponseHandler.TryCatch<JosekisNode>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId));
        }

        public async Task<APIResponse> ToParent(int sessionId)
        {
            string uri = "ToParent";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId));
        }

        public async Task<APIResponse<int>> ToLastBranch(int sessionId)
        {
            string uri = "ToLastBranch";
            return await HubResponseHandler.TryCatch<int>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId));
        }

        public async Task<APIResponse> ToFirst(int sessionId)
        {
            string uri = "ToFirst";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId));
        }

        public async Task<APIResponse<bool>> ToChild(int sessionId, JosekisNode childToGo)
        {
            string uri = "ToChild";
            return await HubResponseHandler.TryCatch<bool>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri, sessionId,
                childToGo));
        }
    }
}
