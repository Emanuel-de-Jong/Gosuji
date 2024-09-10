using Gosuji.API.Data;
using Gosuji.Client.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services.TrainerService
{
    public class TrainerService : IAsyncDisposable
    {
        private KataGoPool pool;
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        public string ConnectionId { get; set; }

        public Game? Game { get; set; }
        public TrainerSettingConfig? SettingConfig { get; set; }
        public MoveTree MoveTree { get; set; } = new();

        public TrainerService(string connectionId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            ConnectionId = connectionId;
            pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;

            Game = new();
        }

        public async Task Save()
        {

        }

        public async ValueTask DisposeAsync()
        {
            await Save();
        }
    }
}
