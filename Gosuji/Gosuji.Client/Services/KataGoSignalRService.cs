using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.KataGo;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gosuji.Client.Services
{
    public class KataGoSignalRService
    {
        public HubConnection HubConnection { get; private set; }

        public KataGoSignalRService(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
        }

        public async Task<APIResponse<string>> Test()
        {
            return await SignalRResponseHandler.Invoke<string>(HubConnection, "Test", "Input");
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
