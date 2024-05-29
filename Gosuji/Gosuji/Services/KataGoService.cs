using GosujiServer.Areas.Identity.Data;
using GosujiServer.Controllers;
using GosujiServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Timers;

namespace GosujiServer.Services
{
    public class KataGoService : IDisposable
    {
        private const int MIN_INSTANCES = 3;
        private const int MAX_INSTANCES = 8;

        private DbService dbService;

        private System.Timers.Timer cashInTimer;

        private Stack<KataGo> freeInstances = new();
        private Dictionary<string, KataGo> instances = new();

        private KataGoVersion? version;

        public KataGoService(DbService _dbService)
        {
            dbService = _dbService;

            cashInTimer = new(12 * 60 * 60 * 1000); // 12 hours
            cashInTimer.AutoReset = true;
            cashInTimer.Elapsed += (object? sender, ElapsedEventArgs e) => CashInTimerElapsed();
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

        public KataGo? Get(string userId)
        {
            if (instances.ContainsKey(userId))
            {
                return null;
            }

            if (freeInstances.Count == 0)
            {
                ManageFreeInstances();
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
            instance.Restart();
            instance.TotalVisits = 0;

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

        private async Task CashIn(string userId)
        {
            UserMoveCount? moveCount = await MoveCountManager.Get(dbService, userId);
            moveCount.KataGoVisits += instances[userId].TotalVisits;

            ApplicationDbContext dbContext = await dbService.GetContextAsync();

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
