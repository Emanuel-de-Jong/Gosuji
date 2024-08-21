using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Drawing;

namespace Gosuji.Client.Services
{
    public class KataGoService
    {
        public HubConnection HubConnection { get; private set; }

        public KataGoService(IConfiguration configuration, UserService userService)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{configuration["BackendUrl"]}/katagohub", options =>
                {
                    options.AccessTokenProvider = async () => await userService.GetToken();
                    options.Transports = HttpTransportType.WebSockets;
                })
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task Start()
        {
            await HubConnection.StartAsync();
        }

        public async Task Stop()
        {
            await HubConnection.StopAsync();
        }

        public async Task<APIResponse<KataGoVersion>> GetVersion()
        {
            string uri = "GetVersion";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse<KataGoVersion>>(uri));
        }

        public async Task<APIResponse> Return()
        {
            string uri = "Return";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
        }

        public async Task<APIResponse<bool>> UserHasInstance()
        {
            string uri = "UserHasInstance";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse<bool>>(uri));
        }

        [JSInvokable]
        public async Task<bool> ClearBoard()
        {
            string uri = "ClearBoard";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> Restart()
        {
            string uri = "Restart";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetBoardsize(int boardsize)
        {
            string uri = "SetBoardsize";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                boardsize));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetRuleset(string ruleset)
        {
            string uri = "SetRuleset";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                ruleset));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetKomi(double komi)
        {
            string uri = "SetKomi";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                komi));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> SetHandicap(int handicap)
        {
            string uri = "SetHandicap";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                handicap));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> AnalyzeMove(string color, string coord)
        {
            string uri = "AnalyzeMove";
            APIResponse<MoveSuggestion> response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse<MoveSuggestion>>(uri,
                color, coord));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(string color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            string uri = "Analyze";
            APIResponse<List<MoveSuggestion>> response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse<List<MoveSuggestion>>>(uri,
                color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<bool> Play(string color, string coord)
        {
            string uri = "Play";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                color, coord));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<bool> PlayRange(Moves moves)
        {
            string uri = "PlayRange";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moves));
            return !G.StatusMessage.HandleAPIResponse(response);
        }
    }
}
