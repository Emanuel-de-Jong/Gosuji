using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services
{
    public class KataGoPool : IDisposable
    {
        private const int MIN_INSTANCES = 1;
        private const int MAX_INSTANCES = 8;

        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        private System.Timers.Timer cashInTimer;

        private Stack<KataGo> freeInstances = new();
        private Dictionary<string, KataGo> instances = [];

        private KataGoVersion? version;

        // TEMP START
        private KataGo? tempInstance;
        // TEMP END

        public KataGoPool(IDbContextFactory<ApplicationDbContext> _dbContextFactory)
        {
            dbContextFactory = _dbContextFactory;

            cashInTimer = new(12 * 60 * 60 * 1000); // 12 hours
            cashInTimer.AutoReset = true;
            cashInTimer.Elapsed += (sender, e) => CashInTimerElapsed();
            cashInTimer.Enabled = true;

            // TEMP START
            return;
            // TEMP END

            ManageFreeInstances();
        }

        public async Task<KataGoVersion> GetVersion()
        {
            if (version == null)
            {
                ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

                version = await dbContext.KataGoVersions
                    .Where(k => k.Model == KataGoVersion.MODEL)
                    .Where(k => k.Version == KataGoVersion.VERSION)
                    .Where(k => k.Config == KataGoVersion.GetConfig())
                    .FirstOrDefaultAsync();

                if (version == null)
                {
                    version = KataGoVersion.GetCurrent();
                    await dbContext.KataGoVersions.AddAsync(version);
                    await dbContext.SaveChangesAsync();
                }

                await dbContext.DisposeAsync();

                version.Config = "";
            }

            return version;
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
            // TEMP START
            if (tempInstance == null)
            {
                tempInstance = new();
                await tempInstance.Start();
            }
            return tempInstance;
            // TEMP END

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
            UserMoveCount? moveCount = await MoveCountHelper.Get(dbContextFactory, userId);
            moveCount.KataGoVisits += instances[userId].TotalVisits;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            dbContext.Update(moveCount);
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
