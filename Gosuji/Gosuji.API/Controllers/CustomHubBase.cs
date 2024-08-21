using Gosuji.API.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

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

        public static HubResponse Ok() => new(HttpStatusCode.OK);
        public static HubResponse<T> Ok<T>(T? data) => new(HttpStatusCode.OK, data);
        public static HubResponse Forbid() => new(HttpStatusCode.Forbidden);
    }
}
