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

        public async Task Test()
        {
            string output = await HubConnection.InvokeAsync<string>("Test", "Input");
            Console.WriteLine(output);
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
