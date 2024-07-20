using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Gosuji.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            IConfigurationSection jwtSettings = builder.Configuration.GetSection("Jwt");
            byte[] jwtKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
            };
            builder.Services.AddSingleton(tokenValidationParameters);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

            builder.Services.AddCors(
                options => options.AddPolicy(
                    "wasm",
                    policy => policy.WithOrigins([
                        builder.Configuration["BackendUrl"] ?? "https://localhost:5001",
                        builder.Configuration["FrontendUrl"] ?? "https://localhost:5002"])
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()));

            builder.Services.AddSingleton<JwtService>();

            RateLimitSetup.AddRateLimiters(builder);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gosuji API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddSingleton<SanitizeService>();
            builder.Services.AddSingleton<KataGoPoolService>();

            WebApplication app = builder.Build();

            app.UseRateLimiter();

            if (builder.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gosuji API V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            app.UseHsts();

            // Create routes for the identity endpoints
            //app.MapIdentityApi<User>();

            app.UseCors("wasm");

            app.UseHttpsRedirection();

            // Enable authentication and authorization after CORS Middleware
            // processing (UseCors) in case the Authorization Middleware tries
            // to initiate a challenge before the CORS Middleware has a chance
            // to set the appropriate headers.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                //TestDataGen testDataGen = new(scope.ServiceProvider);
                //testDataGen.ClearDb();
                //testDataGen.GenerateTestData();

                //RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                //await roleManager.CreateAsync(new IdentityRole("TestRole"));
                //UserManager<User> userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                //User user = await userManager.FindByNameAsync("Graviton");
                //await userManager.AddToRoleAsync(user, "TestRole");
            }

            app.Run();
        }
    }
}
