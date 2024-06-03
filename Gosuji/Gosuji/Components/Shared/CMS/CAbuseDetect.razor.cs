using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS
{
    public partial class CAbuseDetect : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;
        private List<User>? users;
        private Dictionary<string, UserSubscription>? subscriptions;
        private Dictionary<string, long>? totalKataGoVisits;
        private Dictionary<string, long>? weekKataGoVisits;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            users = await dbContext.Users.ToListAsync();
            subscriptions = await dbContext.UserSubscriptions.Include(us => us.SubscriptionType).ToDictionaryAsync(us => us.UserId);

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
                weekKataGoVisits[user.Id] = await MoveCountManager.GetWeekKataGoVisits(dbContextFactory, user.Id);
            }

            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", G.CSSLibUrls["DataTablesBootstrap"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["JQuery"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["DataTablesBootstrap"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("abuseDetect.init");
            }
        }
    }
}
