using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace Gosuji.Client.Services.Trainer
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

        public async Task<APIResponse> Init(TrainerSettingConfig trainerSettingConfig, bool isThirdPartySGF, string? name)
        {
            string uri = "Init";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig, isThirdPartySGF, name));
        }

        public async Task<APIResponse> UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            string uri = "UpdateTrainerSettingConfig";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig));
        }

        [JSInvokable]
        public async Task<AnalyzeResponse?> Analyze(EMoveType moveType, EMoveColor color, bool isMainBranch)
        {
            string uri = "Analyze";
            APIResponse<AnalyzeResponse> response = await HubResponseHandler.TryCatch<AnalyzeResponse>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moveType, color, isMainBranch));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }

        [JSInvokable]
        public async Task<AnalyzeResponse?> AnalyzeAfterJump(Move[] moves, EMoveType moveType, EMoveColor color, bool isMainBranch)
        {
            string uri = "AnalyzeAfterJump";
            APIResponse<AnalyzeResponse> response = await HubResponseHandler.TryCatch<AnalyzeResponse>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moves, moveType, color, isMainBranch));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
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
        public async Task<bool> PlayPlayer(Move move, EPlayerResult playerResult, Coord? chosenNotPlayedCoord,
            int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            string uri = "PlayPlayer";
            APIResponse response = await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                move, playerResult, chosenNotPlayedCoord,
                rightStreak, perfectStreak, rightTopStreak, perfectTopStreak));
            return !G.StatusMessage.HandleAPIResponse(response);
        }

        [JSInvokable]
        public async Task<MoveSuggestion?> PlayForcedCorner(Move move)
        {
            string uri = "PlayForcedCorner";
            APIResponse<MoveSuggestion> response = await HubResponseHandler.TryCatch<MoveSuggestion>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                move));
            return G.StatusMessage.HandleAPIResponse(response) ? null : response.Data;
        }
    }
}
