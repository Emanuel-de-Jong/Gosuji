using Gosuji.Client.Services;
using Gosuji.Components;
using Gosuji.Components.Account;
using Gosuji.Controllers;
using Gosuji.Data;
using Gosuji.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            builder.Services.AddSingleton(serviceProvider =>
            {
                return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromSeconds(10),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 10
                        });
                });
            });

            builder.Services.AddLocalization();

            builder.Services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddHttpContextAccessor();

            // Preconfigure an HttpClient for web API calls
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var request = httpContextAccessor.HttpContext.Request;
                var baseAddress = $"{request.Scheme}://{request.Host.Value}";
                return new HttpClient { BaseAddress = new Uri(baseAddress) };
            });

            // Custom services
            builder.Services.AddSingleton<IDataService, DataService>();
            builder.Services.AddSingleton<IKataGoService, KataGoService>();
            builder.Services.AddSingleton<IJosekisService, JosekisService>();
            builder.Services.AddSingleton<KataGoPoolService>();

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

            LocalizeServer.Setup(app);

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.Use(async (context, next) =>
            {
                PartitionedRateLimiter<HttpContext> rateLimiter = context.RequestServices.GetRequiredService<PartitionedRateLimiter<HttpContext>>();

                RateLimitLease lease = await rateLimiter.AcquireAsync(context);
                if (lease.IsAcquired)
                {
                    await next.Invoke();
                    return;
                }

                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too Many Requests");

                IDbContextFactory<ApplicationDbContext> dbContextFactory = context.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

                RateLimitViolation violation = new()
                {
                    Ip = context.Connection.RemoteIpAddress?.ToString() ?? "",
                    Endpoint = context.Request.Path,
                    Method = Enum.Parse<HTTPMethod>(context.Request.Method)
                };

                await dbContext.RateLimitViolations.AddAsync(violation);
                await dbContext.SaveChangesAsync();
                await dbContext.DisposeAsync();
            });

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client.Components._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.MapControllers();

            // Endpoints
            DataService.CreateEndpoints(app);

            Sanitizer.Init();

            app.Run();
        }
    }
}
