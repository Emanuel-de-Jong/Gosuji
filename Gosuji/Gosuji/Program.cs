using Gosuji.Client.Services;
using Gosuji.Components;
using Gosuji.Components.Account;
using Gosuji.Data;
using Gosuji.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace Gosuji
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            builder.Services.AddControllers();

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

            builder.Services.AddLocalization();

            builder.Services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddHttpContextAccessor();

            // Custom services
            builder.Services.AddSingleton<IDataService, DataService>();
            builder.Services.AddSingleton<IKataGoService, KataGoService>();
            builder.Services.AddSingleton<IJosekisService, JosekisService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            string[] supportedCultures = { "en", "zh", "ko", "ja" };
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.Use(async (context, next) =>
            {
                IRequestCultureFeature? requestCulture = context.Features.Get<IRequestCultureFeature>();
                string defaultCulture = supportedCultures[0];

                if (requestCulture?.RequestCulture.Culture != null)
                {
                    CultureInfo userLanguage = requestCulture.RequestCulture.Culture;
                    if (supportedCultures.Contains(userLanguage.Name))
                    {
                        defaultCulture = userLanguage.Name;
                    }
                }
                else
                {
                    // Use IP to determine location and set culture
                    string? ip = context.Connection.RemoteIpAddress?.ToString();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        using HttpClient httpClient = new();
                        string response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ip}");
                        LocationInfo? locationInfo = JsonSerializer.Deserialize<LocationInfo>(response);

                        if (locationInfo != null && !string.IsNullOrEmpty(locationInfo.CountryCode))
                        {
                            // Map country code to culture
                            defaultCulture = locationInfo.CountryCode switch
                            {
                                "US" => "en",
                                "CN" => "zh",
                                "KR" => "ko",
                                "JP" => "ja",
                                _ => defaultCulture,
                            };
                        }
                    }
                }

                CultureInfo cultureInfo = new(defaultCulture);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                await next.Invoke();
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client.Components._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.MapControllers();

            // Endpoints
            DataService.CreateEndpoints(app);
            JosekisService.CreateEndpoints(app);
            KataGoService.CreateEndpoints(app);

            app.Run();
        }
    }

    public class LocationInfo
    {
        public string CountryCode { get; set; }
    }
}
