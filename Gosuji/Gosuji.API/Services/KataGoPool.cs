﻿using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services
{
    public class KataGoPool : IAsyncDisposable
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
            foreach (string userId in new Dictionary<string, KataGo>(instances).Keys)
            {
                KataGo instance = instances[userId];
                if ((DateTimeOffset.UtcNow - instance.LastStartTime).Hours <= 6)
                {
                    continue;
                }

                instances.Remove(userId);
                instance.IsPaused = true;

                await CashIn(userId);

                instance.Stop();
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

            KataGo instance = instances[userId];
            instances.Remove(userId);
            instance.IsPaused = true;

            await CashIn(userId);

            await instance.Restart();

            instance.IsPaused = false;

            freeInstances.Push(instance);

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

        public async Task CashIn(string userId)
        {
            if (!instances.ContainsKey(userId))
            {
                return;
            }

            KataGo instance = instances[userId];

            UserMoveCount? moveCount = await MoveCountHelper.Get(dbContextFactory, userId);
            moveCount.KataGoVisits += instance.TotalVisits;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            dbContext.Update(moveCount);
            await dbContext.SaveChangesAsync();

            instance.TotalVisits = 0;

            await dbContext.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            List<Task> tasks = [];
            foreach (string userId in instances.Keys)
            {
                tasks.Add(CashIn(userId));
            }

            await Task.WhenAll(tasks);
        }
    }
}
