using Gosuji.Client.Components.Account;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

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

            builder.Services.AddLocalization();

            WebAssemblyHost host = builder.Build();

            const string defaultCulture = "en";

            IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
            string? result = await js.InvokeAsync<string>("blazorCulture.get");
            CultureInfo culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

            if (result == null)
            {
                await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }
    }
}
