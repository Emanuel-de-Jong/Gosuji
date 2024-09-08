using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.KataGo;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Gosuji.Client.Services
{
    public class TrainerConnection : BaseConnection
    {
        public TrainerConnection(IConfiguration configuration, UserAPI userAPI)
            : base(configuration, userAPI, "trainerhub")
        {
        }

        public async Task<APIResponse<KataGoVersion>> GetVersion()
        {
            string uri = "GetVersion";
            return await HubResponseHandler.TryCatch<KataGoVersion>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
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
            return await HubResponseHandler.TryCatch<bool>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
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
        public async Task<MoveSuggestion?> AnalyzeMove(Move move)
        {
            string uri = "AnalyzeMove";
            APIResponse<MoveSuggestion> response = await HubResponseHandler.TryCatch<MoveSuggestion>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                move));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<List<MoveSuggestion>?> Analyze(int color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            string uri = "Analyze";
            APIResponse<List<MoveSuggestion>> response = await HubResponseHandler.TryCatch<List<MoveSuggestion>>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<bool> Play(Move move)
        {
            string uri = "Play";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                move));
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
