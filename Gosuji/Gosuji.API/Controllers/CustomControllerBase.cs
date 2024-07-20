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
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
        }

        protected async Task<User?> GetUser(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            User? user = await GetUser(dbContext);
            await dbContext.DisposeAsync();
            return user;
        }

        protected async Task<User?> GetUser(UserManager<User> userManager)
        {
            return await userManager.GetUserAsync(User);
        }
    }
}
