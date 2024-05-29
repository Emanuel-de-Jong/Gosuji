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

            DateTimeOffset lastMonday = DateTimeOffset.UtcNow.AddDays(-(int)(DateTimeOffset.UtcNow.DayOfWeek - DayOfWeek.Monday + 7) % 7);
            if (moveCount == null || moveCount.CreateDate < lastMonday)
            {
                moveCount = new UserMoveCount(userId);
                await dbContext.UserMoveCounts.AddAsync(moveCount);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.DisposeAsync();

            return moveCount;
        }

        // Doesn't create a new UserMoveCount.
        public static async Task<long> GetWeekKataGoVisits(DbService dbService, string userId)
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();

            List<UserMoveCount> moveCounts = await dbContext.UserMoveCounts.Where(mc => mc.UserId == userId).ToListAsync();
            UserMoveCount? moveCount = moveCounts.OrderBy(mc => mc.CreateDate).LastOrDefault();

            DateTimeOffset lastMonday = DateTimeOffset.UtcNow.AddDays(-(int)(DateTimeOffset.UtcNow.DayOfWeek - DayOfWeek.Monday + 7) % 7);
            if (moveCount == null || moveCount.CreateDate < lastMonday)
            {
                return 0;
            }

            await dbContext.DisposeAsync();

            return moveCount.KataGoVisits;
        }
    }
}
