using Gosuji.Client.Models.KataGo;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gosuji.Client.Services
{
    public class KataGoSignalRService
    {
        private readonly HubConnection hubConnection;

        public KataGoSignalRService(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
        }

        public async Task Test()
        {
            string output = await hubConnection.InvokeAsync<string>("Test", "Input");
            Console.WriteLine(output);
        }

        public async Task Start()
        {
            await hubConnection.StartAsync();
        }

        public async Task Stop()
        {
            await hubConnection.StopAsync();
        }
    }
}
