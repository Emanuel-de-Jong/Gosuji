using Gosuji.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services
{
    public class RateLimitLogger
    {
        private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;

        public RateLimitLogger(IDbContextFactory<ApplicationDbContext> _dbContextFactory)
        {
            dbContextFactory = _dbContextFactory;
        }

        public async Task LogViolation(HttpContext context)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            RateLimitViolation violation = new()
            {
                Ip = context.Connection.RemoteIpAddress?.ToString() ?? "",
                Endpoint = context.Request.Path,
                Method = Enum.Parse<HTTPMethod>(context.Request.Method)
            };

            await dbContext.RateLimitViolations.AddAsync(violation);
            await dbContext.SaveChangesAsync();

            await dbContext.DisposeAsync();
        }
    }
}
