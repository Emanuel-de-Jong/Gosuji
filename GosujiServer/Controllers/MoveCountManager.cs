using GosujiServer.Data;
using GosujiServer.Services;
using Microsoft.EntityFrameworkCore;

namespace GosujiServer.Controllers
{
    public class MoveCountManager
    {
        public static async Task<UserMoveCount> Get(DbService dbService, string userId)
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();

            List<UserMoveCount> moveCounts = await dbContext.UserMoveCounts.Where(mc => mc.UserId == userId).ToListAsync();

            UserMoveCount? moveCount = moveCounts.OrderBy(mc => mc.CreateDate).LastOrDefault();
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
