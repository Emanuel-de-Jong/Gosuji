using Gosuji.Client;
using Gosuji.Client.Services;
using Gosuji.Components;
using Gosuji.Components.Account;
using Gosuji.Data;
using Gosuji.Helpers;
using Gosuji.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.RateLimiting;

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

            builder.Services.AddSingleton<RateLimitLogger>();

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy(G.RazorRateLimitPolicyName, context =>
                {
                    string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.AddPolicy(G.ControllerRateLimitPolicyName, context =>
                {
                    string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too Many Requests");

                    RateLimitLogger logger = context.HttpContext.RequestServices.GetRequiredService<RateLimitLogger>();
                    logger.LogViolation(context.HttpContext);
                };
            });


            builder.Services.AddControllers();

            builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

            builder.Services.AddLocalization();

            builder.Services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddHttpContextAccessor();

            // Preconfigure an HttpClient for web API calls
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                IHttpContextAccessor httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                HttpRequest request = httpContextAccessor.HttpContext.Request;
                string baseAddress = $"{request.Scheme}://{request.Host.Value}";
                return new HttpClient { BaseAddress = new Uri(baseAddress) };
            });

            // Custom services
            builder.Services.AddSingleton<SanitizeService>();
            builder.Services.AddSingleton<KataGoPoolService>();
            builder.Services.AddSingleton<DataService, DataService>();
            builder.Services.AddSingleton<KataGoService, KataGoService>();
            builder.Services.AddSingleton<JosekisService, JosekisService>();

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

            app.MapControllers()
                .RequireRateLimiting(G.ControllerRateLimitPolicyName);

            app.Run();
        }
    }
}
