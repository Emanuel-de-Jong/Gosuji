using Gosuji.API.Data;
using Gosuji.Client;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Gosuji.API.Controllers
{
    public class CustomHubBase : Hub
    {
        protected string GetUserId()
        {
            return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new HubException("User is not authenticated.");
        }

        protected async Task<User?> GetUser(ApplicationDbContext dbContext)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return null;
            }

            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        protected async Task<User?> GetUser(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return null;
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            User? user = await GetUser(dbContext);
            await dbContext.DisposeAsync();
            return user;
        }

        protected async Task<User?> GetUser(UserManager<User> userManager)
        {
            string? userId = GetUserId();
            if (userId == null)
            {
                return null;
            }

            return await userManager.FindByIdAsync(userId);
        }

        private static string ToJson<T>(T data)
        {
            return JsonSerializer.Serialize(data, G.JsonSerializerOptions);
        }

        public static readonly HubResponse Ok = new(HttpStatusCode.OK);

        public static HubResponse OkData<T>(T data)
        {
            return new(HttpStatusCode.OK, ToJson(data));
        }

        public static HubResponse BadRequest<T>(T data)
        {
            return new(HttpStatusCode.BadRequest, ToJson(data));
        }

        public static readonly HubResponse Forbid = new(HttpStatusCode.Forbidden);
    }
}
