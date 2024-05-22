using GosujiServer.Areas.Identity.Data;
using GosujiServer.Controllers;
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
        private Dictionary<string, long> totalKataGoVisits;
        private Dictionary<string, long> weekKataGoVisits;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            users = await dbContext.Users.ToListAsync();
            subscriptions = await dbContext.UserSubscriptions.Include(us => us.SubscriptionType).ToDictionaryAsync(us => us.UserId);

            totalKataGoVisits = new();
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

            weekKataGoVisits = new();
            foreach (User user in users)
            {
                weekKataGoVisits[user.Id] = await MoveCountManager.GetWeekKataGoVisits(dbService, user.Id);
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
