using Gosuji.Client;
using Gosuji.Data;
using Gosuji.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS
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

        private IJSObjectReference jsRef;

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
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", G.CSSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["JQuery"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["DataTables"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("cAbuseDetect.init");
            }
        }
    }
}
