using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;

namespace Gosuji.Client.Services
{
    public class KataGoSignalRService
    {
        public HubConnection HubConnection { get; private set; }

        public KataGoSignalRService(IConfiguration configuration, UserService userService)
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

        public async Task<APIResponse<string>> Test()
        {
            string uri = "Test";
            return await SignalRResponseHandler.TryCatch(uri, HubConnection.InvokeAsync<string>(uri,
                "Input"));
        }

        public async Task Start()
        {
            await HubConnection.StartAsync();
        }

        public async Task Stop()
        {
            await HubConnection.StopAsync();
        }
    }
}
