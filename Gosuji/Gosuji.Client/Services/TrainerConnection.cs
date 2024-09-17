using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
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

        public async Task<APIResponse<bool>> UserHasInstance()
        {
            string uri = "UserHasInstance";
            return await HubResponseHandler.TryCatch<bool>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri));
        }

        public async Task<APIResponse> Init(TrainerSettingConfig trainerSettingConfig, bool isThirdPartySGF)
        {
            string uri = "Init";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig, isThirdPartySGF));
        }

        public async Task<APIResponse> UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            string uri = "UpdateTrainerSettingConfig";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig));
        }

        [JSInvokable]
        public async Task<bool> SyncBoard(Move[] moves)
        {
            string uri = "SyncBoard";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moves));
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
        public async Task<MoveSuggestionList?> Analyze(EMoveType moveType, EMoveColor color)
        {
            string uri = "Analyze";
            APIResponse<MoveSuggestionList> response = await HubResponseHandler.TryCatch<MoveSuggestionList>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moveType, color));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<bool> PlayUser(Move move, int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            string uri = "PlayUser";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                move, rightStreak, perfectStreak, rightTopStreak, perfectTopStreak));
            return !G.StatusMessage.HandleAPIResponse(response);
        }
    }
}
