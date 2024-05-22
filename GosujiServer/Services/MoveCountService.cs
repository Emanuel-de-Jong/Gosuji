using GosujiServer.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GosujiServer.Services
{
    public class MoveCountService
    {
        private AuthenticationStateProvider authenticationStateProvider;
        private DbService dbService;

        private string? userId;

        public MoveCountService(AuthenticationStateProvider _authenticationStateProvider, DbService _dbService)
        {
            authenticationStateProvider = _authenticationStateProvider;
            dbService = _dbService;

            GetUser();
        }

        private async Task GetUser()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }

        public async Task<UserMoveCount> Get()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();

            UserMoveCount? moveCount = await dbContext.UserMoveCounts.Where(mc => mc.UserId == userId).OrderBy(mc => mc.CreateDate).FirstOrDefaultAsync();
            if (moveCount == null || (DateTimeOffset.UtcNow - moveCount.CreateDate).TotalDays > 7)
            {
                moveCount = new UserMoveCount(userId);
                await dbContext.UserMoveCounts.AddAsync(moveCount);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.DisposeAsync();

            return moveCount;
        }
    }
}
