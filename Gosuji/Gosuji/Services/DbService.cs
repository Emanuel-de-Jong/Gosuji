using GosujiServer.Data;
using Microsoft.EntityFrameworkCore;

namespace GosujiServer.Services
{
    public class DbService
    {
        public ApplicationDbContext GetContext()
        {
            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
            //optionsBuilder.UseSqlServer(G.ConnectionString);
            optionsBuilder.UseSqlite(G.ConnectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        public Task<ApplicationDbContext> GetContextAsync()
        {
            return Task.FromResult(GetContext());
        }
    }
}
