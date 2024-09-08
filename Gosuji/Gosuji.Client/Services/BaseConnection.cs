using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace Gosuji.Client.Services
{
    public abstract class BaseConnection
    {
        public HubConnection HubConnection { get; private set; }
        public bool IsConnected => HubConnection.State == HubConnectionState.Connected;

        public BaseConnection(IConfiguration configuration, UserAPI userAPI, string uri)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{configuration["BackendUrl"]}/{uri}", options =>
                {
                    options.AccessTokenProvider = async () => await userAPI.GetToken();
                    options.Transports = HttpTransportType.WebSockets;
                })
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task<APIResponse> Start()
        {
            try
            {
                await HubConnection.StartAsync();
            }
            catch (Exception exception)
            {
                return HubResponseHandler.HandleException(exception, "BaseHubService.Start");
            }

            return new APIResponse(HttpStatusCode.OK);
        }

        public async Task<APIResponse> Stop()
        {
            try
            {
                await HubConnection.StopAsync();
            }
            catch (Exception exception)
            {
                return HubResponseHandler.HandleException(exception, "BaseHubService.Stop");
            }

            return new APIResponse(HttpStatusCode.OK);
        }
    }
}
