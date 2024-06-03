using Gosuji.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.Controllers
{
    public class KataGoPool : IDisposable
    {
        private const int MIN_INSTANCES = 1;
        private const int MAX_INSTANCES = 8;

        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        private System.Timers.Timer cashInTimer;

        private Stack<KataGo> freeInstances = new();
        private Dictionary<string, KataGo> instances = [];

        public KataGoPool(IDbContextFactory<ApplicationDbContext> _dbContextFactory)
        {
            dbContextFactory = _dbContextFactory;

            cashInTimer = new(12 * 60 * 60 * 1000); // 12 hours
            cashInTimer.AutoReset = true;
            cashInTimer.Elapsed += (sender, e) => CashInTimerElapsed();
            cashInTimer.Enabled = true;

            ManageFreeInstances();
        }

        private async Task CashInTimerElapsed()
        {
            foreach (KeyValuePair<string, KataGo> pair in new Dictionary<string, KataGo>(instances))
            {
                if ((DateTimeOffset.UtcNow - pair.Value.LastStartTime).Hours <= 6)
                {
                    continue;
                }

                await CashIn(pair.Key);

                pair.Value.Stop();
                instances.Remove(pair.Key);
            }
        }

        public async Task<KataGo> Get(string userId)
        {
            if (instances.ContainsKey(userId))
            {
                return instances[userId];
            }

            if (freeInstances.Count == 0)
            {
                await ManageFreeInstances();
            }

            KataGo instance = freeInstances.Pop();
            instances[userId] = instance;

            ManageFreeInstances();

            return instance;
        }

        public async Task Return(string userId)
        {
            if (!instances.ContainsKey(userId))
            {
                return;
            }

            await CashIn(userId);

            KataGo instance = instances[userId];
            await instance.Restart();
            instance.TotalVisits = 0;

            freeInstances.Push(instance);
            instances.Remove(userId);

            ManageFreeInstances();
        }

        public bool UserHasInstance(string userId)
        {
            return instances.ContainsKey(userId);
        }

        private async Task ManageFreeInstances()
        {
            if (freeInstances.Count < MIN_INSTANCES)
            {
                for (int i = 0; i < MIN_INSTANCES - freeInstances.Count; i++)
                {
                    KataGo newInstance = new();
                    freeInstances.Push(newInstance);

                    await newInstance.Start();
                }
            }
            else if (freeInstances.Count > MAX_INSTANCES)
            {
                for (int i = 0; i < freeInstances.Count - MAX_INSTANCES; i++)
                {
                    freeInstances.Pop();
                }
            }
        }

        private async Task CashIn(string userId)
        {
            UserMoveCount? moveCount = await MoveCountManager.Get(dbContextFactory, userId);
            moveCount.KataGoVisits += instances[userId].TotalVisits;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            dbContext.UserMoveCounts.Update(moveCount);
            await dbContext.SaveChangesAsync();

            await dbContext.DisposeAsync();
        }

        public void Dispose()
        {
            foreach (string userId in instances.Keys)
            {
                CashIn(userId).Wait();
            }
        }
    }
}
