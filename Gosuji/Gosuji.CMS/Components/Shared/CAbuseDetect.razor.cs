using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.CMS.Components.Shared
{
    internal class RateLimitViolator
    {
        public string Ip { get; set; }
        public int Violations { get; set; }
        public string MostCommonEndpoint { get; set; }
    }

    public partial class CAbuseDetect : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private List<User>? users;
        private Dictionary<string, long>? totalKataGoVisits;
        private Dictionary<string, long>? weekKataGoVisits;

        private List<RateLimitViolator> rateLimitViolators;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            users = await dbContext.Users.Include(u => u.CurrentSubscription).ToListAsync();

            totalKataGoVisits = [];
            foreach (UserMoveCount moveCount in await dbContext.UserMoveCounts.ToListAsync())
            {
                if (!totalKataGoVisits.ContainsKey(moveCount.UserId))
                {
                    totalKataGoVisits[moveCount.UserId] = moveCount.KataGoVisits;
                }
                else
                {
                    totalKataGoVisits[moveCount.UserId] += moveCount.KataGoVisits;
                }
            }

            weekKataGoVisits = [];
            foreach (User user in users)
            {
                weekKataGoVisits[user.Id] = await MoveCountHelper.GetWeekKataGoVisits(dbContextFactory, user.Id);
            }

            rateLimitViolators = await dbContext.RateLimitViolations
                .GroupBy(v => v.Ip)
                .Select(g => new RateLimitViolator
                {
                    Ip = g.Key,
                    Violations = g.Count(),
                    MostCommonEndpoint = g.GroupBy(v => v.Endpoint)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault()
                }).ToListAsync();

            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeVoidAsync("cAbuseDetect.init");
            }
        }
    }
}
