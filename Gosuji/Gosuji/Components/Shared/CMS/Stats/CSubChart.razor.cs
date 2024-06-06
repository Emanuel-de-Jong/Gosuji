using Gosuji.Client;
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



            await dbContext.DisposeAsync();

            dayChartSubs = new int[dayCount];
            dayChartNewSubs = new int[dayCount];
            monthChartSubs = new int[monthCount];
            monthChartNewSubs = new int[monthCount];
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
