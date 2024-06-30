using Gosuji.Account;
using Gosuji.Client;
using Gosuji.Client.Services;
using Gosuji.Components;
using Gosuji.Data;
using Gosuji.Helpers;
using Gosuji.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthHandler>();

            RateLimitSetup.AddRateLimiters(builder);

            builder.Services.AddControllers();

            builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

            builder.Services.AddLocalization();

            builder.Services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddHttpContextAccessor();

            // Preconfigure an HttpClient for web API calls
            builder.Services.AddScoped<HttpClient>(sp =>
            {
                IHttpContextAccessor httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                HttpRequest Request = httpContextAccessor.HttpContext.Request;
                Uri baseAddress = new($"{Request.Scheme}://{Request.Host.Value}");

                HttpClientHandler handler = new();
                if (Request.Cookies != null)
                {
                    foreach (KeyValuePair<string, string> cookie in Request.Cookies)
                    {
                        handler.CookieContainer.Add(baseAddress, new Cookie(cookie.Key, cookie.Value));
                    }
                }

                return new(handler) { BaseAddress = baseAddress };
            });

            // Custom services
            builder.Services.AddSingleton<SanitizeService>();
            builder.Services.AddSingleton<KataGoPoolService>();
            builder.Services.AddScoped<DataService, DataService>();
            builder.Services.AddScoped<KataGoService, KataGoService>();
            builder.Services.AddScoped<JosekisService, JosekisService>();

            WebApplication app = builder.Build();

            app.UseRateLimiter();

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

            LocalizeServer.Setup(app);

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .RequireRateLimiting(G.RazorRateLimitPolicyName)
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client.Components._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.MapControllers();

            //using (IServiceScope scope = app.Services.CreateScope())
            //{
            //    TestDataGen testDataGen = new(scope.ServiceProvider);
            //    testDataGen.ClearDb();
            //    testDataGen.GenerateTestData();
            //}

            app.Run();
        }
    }
}
