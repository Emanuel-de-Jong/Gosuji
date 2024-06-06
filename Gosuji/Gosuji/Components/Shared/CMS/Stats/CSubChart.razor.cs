using Gosuji.Client;
using Gosuji.Client.Components.Pages;
using Gosuji.Client.Data;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS.Stats
{
    public partial class CSubChart : ComponentBase
    {
        [Parameter]
        public List<DateTime> GraphDays { get; set; }
        [Parameter]
        public List<DateTime> GraphMonths { get; set; }
        [Parameter]
        public List<string> DayLabels { get; set; }
        [Parameter]
        public List<string> MonthLabels { get; set; }

        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;

        private int[] dayChartSubs;
        private int[] dayChartNewSubs;
        private int[] monthChartSubs;
        private int[] monthChartNewSubs;

        protected override async Task OnInitializedAsync()
        {
            int dayCount = GraphDays.Count;
            int monthCount = GraphMonths.Count;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            List<UserSubscription> subs = (await dbContext.Users
                .Where(u => u.CurrentSubscriptionId != null)
                .Include(u => u.CurrentSubscription)
                .ToListAsync())
                .Select(u => u.CurrentSubscription)
                .ToList();

            await dbContext.DisposeAsync();

            int daySubCount = subs.Count(s => s.CreateDate.Date < GraphDays.First());
            int monthSubCount = subs.Count(s => s.CreateDate.Date < GraphMonths.First());

            List<UserSubscription> monthSubs = subs.Where(s => s.CreateDate.Date >= GraphMonths.First()).ToList();

            // dayChartNewSubs
            dayChartNewSubs = new int[dayCount];
            List<UserSubscription> daySubs = monthSubs.Where(s => s.CreateDate.Date >= GraphDays.First()).ToList();
            for (int i = 0; i < GraphDays.Count; i++)
            {
                DateTime time = GraphDays[i];
                dayChartNewSubs[i] = daySubs.Count(s => s.CreateDate.Date == time);
            }

            // dayChartSubs
            dayChartSubs = new int[dayCount];
            for (int i = 0; i < dayChartNewSubs.Length; i++)
            {
                daySubCount += dayChartNewSubs[i];
                dayChartSubs[i] = daySubCount;
            }

            // monthChartNewSubs
            monthChartNewSubs = new int[monthCount];
            for (int i = 0; i < GraphMonths.Count; i++)
            {
                DateTime time = GraphMonths[i];
                monthChartNewSubs[i] = monthSubs.Count(s => s.CreateDate.Year == time.Year && s.CreateDate.Month == time.Month);
            }

            // monthChartSubs
            monthChartSubs = new int[monthCount];
            for (int i = 0; i < monthChartNewSubs.Length; i++)
            {
                monthSubCount += monthChartNewSubs[i];
                monthChartSubs[i] = monthSubCount;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["ChartJS"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("cSubChart.init",
                    DayLabels,
                    MonthLabels,
                    dayChartSubs,
                    dayChartNewSubs,
                    monthChartSubs,
                    monthChartNewSubs);
            }
        }
    }
}
