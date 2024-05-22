using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Pages;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace GosujiServer.Shared.CMS
{
    public partial class AbuseDetect : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private DbService dbService { get; set; }

        private List<User> users;
        private Dictionary<string, UserSubscription> subscriptions;
        private Dictionary<string, long> totalVisits;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            users = await dbContext.Users.ToListAsync();
            subscriptions = await dbContext.UserSubscriptions.Include(us => us.SubscriptionType).ToDictionaryAsync(us => us.UserId);

            totalVisits = new();
            foreach (UserMoveCount moveCount in await dbContext.UserMoveCounts.ToListAsync())
            {
                if (!totalVisits.ContainsKey(moveCount.UserId))
                {
                    totalVisits[moveCount.UserId] = moveCount.Visits;
                }
                else
                {
                    totalVisits[moveCount.UserId] += moveCount.Visits;
                }
            }

            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("abuseDetect.init");
            }
        }
    }
}
