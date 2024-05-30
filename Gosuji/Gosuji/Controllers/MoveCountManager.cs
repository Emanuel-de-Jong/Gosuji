using Gosuji.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.Controllers
{
    public class MoveCountManager
    {
        public static async Task<UserMoveCount> Get(IDbContextFactory<ApplicationDbContext> dbContextFactory, string userId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            List<UserMoveCount> moveCounts = await dbContext.UserMoveCounts.Where(mc => mc.UserId == userId).ToListAsync();
            UserMoveCount? moveCount = moveCounts.OrderBy(mc => mc.CreateDate).LastOrDefault();

            DateTimeOffset lastMonday = DateTimeOffset.UtcNow.AddDays(-(DateTimeOffset.UtcNow.DayOfWeek - DayOfWeek.Monday + 7) % 7);
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
        public static async Task<long> GetWeekKataGoVisits(IDbContextFactory<ApplicationDbContext> dbContextFactory, string userId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            List<UserMoveCount> moveCounts = await dbContext.UserMoveCounts.Where(mc => mc.UserId == userId).ToListAsync();
            UserMoveCount? moveCount = moveCounts.OrderBy(mc => mc.CreateDate).LastOrDefault();

            DateTimeOffset lastMonday = DateTimeOffset.UtcNow.AddDays(-(DateTimeOffset.UtcNow.DayOfWeek - DayOfWeek.Monday + 7) % 7);
            if (moveCount == null || moveCount.CreateDate < lastMonday)
            {
                return 0;
            }

            await dbContext.DisposeAsync();

            return moveCount.KataGoVisits;
        }
    }
}
