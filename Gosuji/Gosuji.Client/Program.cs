using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Gosuji.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            // Preconfigure an HttpClient for web API calls
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Custom services
            builder.Services.AddSingleton<IDataService, ClientDataService>();
            builder.Services.AddSingleton<IKataGoService, ClientKataGoService>();
            builder.Services.AddSingleton<IJosekisService, ClientJosekisService>();
            builder.Services.AddSingleton<ITranslateService, TranslateService>();

            await builder.Build().RunAsync();
        }
    }
}
