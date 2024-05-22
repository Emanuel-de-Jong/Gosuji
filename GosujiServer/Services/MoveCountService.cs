using GosujiServer.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GosujiServer.Services
{
    public class MoveCountService
    {
        private DbService dbService;

        public MoveCountService(DbService _dbService)
        {
            dbService = _dbService;
        }

        public async Task<UserMoveCount> Get(string userId)
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
