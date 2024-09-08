using Gosuji.Client.Components;
using Gosuji.Client.Helpers;
using Gosuji.Client.Services;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Gosuji.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            G.Log();
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddSingleton<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(builder.Configuration["FrontendUrl"]) });

            builder.Services.AddSingleton<UserAPI>();

            builder.Services.AddTransient<AuthMessageHandler>();

            builder.Services.AddHttpClient("Auth", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["BackendUrl"]);
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddLocalization();

            builder.Services.AddSingleton<DataAPI>();
            builder.Services.AddSingleton<TrainerConnection>();
            builder.Services.AddSingleton<JosekisConnection>();
            builder.Services.AddSingleton<SettingConfigService>();

            WebAssemblyHost host = builder.Build();

            UserAPI userAPI = host.Services.GetRequiredService<UserAPI>();
            IHttpClientFactory httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
            userAPI.HTTP = httpClientFactory.CreateClient("Auth");
            userAPI.AuthenticationStateProvider = host.Services.GetRequiredService<AuthenticationStateProvider>() as JwtAuthenticationStateProvider;

            await LocalizeClient.Setup(host);

            await host.RunAsync();
        }
    }
}
