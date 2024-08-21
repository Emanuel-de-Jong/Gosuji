using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.KataGo;
using Microsoft.AspNetCore.SignalR.Client;
using System;

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
