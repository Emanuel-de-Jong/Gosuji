using Gosuji.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gosuji.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
    }
}
