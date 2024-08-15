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
                new HttpClient { BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:5002") });

            builder.Services.AddSingleton<UserService>();

            builder.Services.AddTransient<AuthMessageHandler>();

            builder.Services.AddHttpClient("Auth", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddLocalization();

            builder.Services.AddSingleton<DataService>();
            builder.Services.AddSingleton<KataGoService>();
            builder.Services.AddSingleton<JosekisService>();
            builder.Services.AddSingleton<TestService>();
            builder.Services.AddTransient<SettingConfigService>();

            WebAssemblyHost host = builder.Build();

            UserService userService = host.Services.GetRequiredService<UserService>();
            IHttpClientFactory httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
            userService.HTTP = httpClientFactory.CreateClient("Auth");
            userService.AuthenticationStateProvider = host.Services.GetRequiredService<AuthenticationStateProvider>() as JwtAuthenticationStateProvider;

            await LocalizeClient.Setup(host);

            await host.RunAsync();
        }
    }
}
