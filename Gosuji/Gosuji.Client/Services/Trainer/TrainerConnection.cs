using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Services.TrainerService;
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

        public async Task<APIResponse<MoveTree>> LoadGame(string gameId)
        {
            string uri = "LoadGame";
            return await HubResponseHandler.TryCatch<MoveTree>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                gameId));
        }

        public async Task<APIResponse<bool>> Init(TrainerSettingConfig trainerSettingConfig,
            TreeNode<Move>? thirdPartyMoves, string? name)
        {
            string uri = "Init";
            return await HubResponseHandler.TryCatch<bool>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig, thirdPartyMoves, name));
        }

        public async Task<APIResponse> UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            string uri = "UpdateTrainerSettingConfig";
            return await HubResponseHandler.TryCatch(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                trainerSettingConfig));
        }

        [JSInvokable]
        public async Task<AnalyzeResponse?> Analyze(EMoveOrigin moveOrigin, EMoveColor color, bool isMainBranch, Move[]? moves)
        {
            string uri = "Analyze";
            APIResponse<AnalyzeResponse> response = await HubResponseHandler.TryCatch<AnalyzeResponse>(uri,
                HubConnection.InvokeAsync<HubResponse>(uri,
                moveOrigin, color, isMainBranch, moves));
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
