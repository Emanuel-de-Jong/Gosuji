using GosujiServer.Controllers;
using GosujiServer.Data;
using System.Timers;

namespace GosujiServer.Services
{
    public class KataGoService : IDisposable
    {
        private const int MIN_INSTANCES = 3;
        private const int MAX_INSTANCES = 8;

        private DbService dbService;
        private MoveCountService moveCountService;

        private System.Timers.Timer cashInTimer;

        private Stack<KataGo> freeInstances = new();
        private Dictionary<string, KataGo> instances = new();

        private KataGoVersion? version;

        public KataGoService(DbService _dbService, MoveCountService _moveCountService)
        {
            dbService = _dbService;
            moveCountService = _moveCountService;

            cashInTimer = new(12 * 60 * 60 * 1000); // 12 hours
            cashInTimer.AutoReset = true;
            cashInTimer.Elapsed += (object? _, ElapsedEventArgs _) => CashIn(false);
            cashInTimer.Enabled = true;

            ManageFreeInstances();
        }

        public KataGo Get(string userId)
        {
            if (freeInstances.Count == 0)
            {
                ManageFreeInstances();
            }

            KataGo instance = freeInstances.Pop();
            instances[userId] = instance;

            ManageFreeInstances();

            return instance;
        }

        public void Return(string userId)
        {
            KataGo instance = instances[userId];
            instance.Restart();

            freeInstances.Push(instance);
            instances.Remove(userId);

            ManageFreeInstances();
        }

        private void ManageFreeInstances()
        {
            if (freeInstances.Count < MIN_INSTANCES)
            {
                for (int i = 0; i < MIN_INSTANCES - freeInstances.Count; i++)
                {
                    freeInstances.Push(new KataGo());
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

        public KataGoVersion GetVersion()
        {
            if (version == null)
            {
                ApplicationDbContext context = dbService.GetContext();

                version = context.KataGoVersions
                    .Where(k => k.Model == KataGoVersion.MODEL)
                    .Where(k => k.Version == KataGoVersion.VERSION)
                    .Where(k => k.Config == KataGoVersion.GetConfig())
                    .FirstOrDefault();

                if (version == null)
                {
                    version = new KataGoVersion();
                    context.Add(version);
                    context.SaveChanges();
                }

                context.Dispose();
            }

            return version;
        }

        private async Task CashIn(bool forceAll)
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();

            foreach (KeyValuePair<string, KataGo> pair in instances)
            {
                if (!forceAll && (DateTimeOffset.UtcNow - pair.Value.LastStartTime).Hours <= 6)
                {
                    continue;
                }

                UserMoveCount? moveCount = await moveCountService.Get();
                moveCount.KataGoVisits += pair.Value.TotalVisits;

                dbContext.UserMoveCounts.Update(moveCount);
                await dbContext.SaveChangesAsync();

                instances.Remove(pair.Key);
            }

            await dbContext.DisposeAsync();
        }

        public void Dispose()
        {
            CashIn(true).Wait();
        }
    }
}
