using Gosuji.Client.Data;
using Gosuji.Client;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS.Stats
{
    public partial class CUserChart : ComponentBase
    {
        private static int DAY_COUNT = 14;
        private static int MONTH_COUNT = 12;

        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;

        private List<string> dayLabels = [];
        private List<string> monthLabels = [];

        private int[] dayUserChartUsers = new int[DAY_COUNT];
        private int[] dayUserChartActiveUsers = new int[DAY_COUNT];
        private int[] dayUserChartNewUsers = new int[DAY_COUNT];
        private int[] monthUserChartUsers = new int[MONTH_COUNT];
        private int[] monthUserChartActiveUsers = new int[MONTH_COUNT];
        private int[] monthUserChartNewUsers = new int[MONTH_COUNT];

        protected override async Task OnInitializedAsync()
        {
            List<DateTime> graphDays = [];
            DateTime dateNow = DateTimeOffset.UtcNow.Date;
            for (int i = DAY_COUNT - 1; i >= 0; i--)
            {
                graphDays.Add(dateNow.AddDays(-i));
            }

            List<DateTime> graphMonths = [];
            dateNow = new(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1);
            for (int i = MONTH_COUNT - 1; i >= 0; i--)
            {
                graphMonths.Add(dateNow.AddMonths(-i));
            }

            // dayLabels
            foreach (DateTime day in graphDays)
            {
                dayLabels.Add(day.ToString("dd-MM"));
            }

            // monthLabels
            foreach (DateTime month in graphMonths)
            {
                monthLabels.Add(month.ToString("MMMM"));
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            List<UserActivity> monthUserActivities = (await dbContext.UserActivities.ToListAsync()).Where(ua => ua.CreateDate.Date >= graphMonths.First()).ToList();
            List<User> users = await dbContext.Users.ToListAsync();

            int dayUserCount = users.Count(u => u.CreateDate.Date < graphDays.First());
            int monthUserCount = users.Count(u => u.CreateDate.Date < graphMonths.First());

            await dbContext.DisposeAsync();

            // dayUserChartActiveUsers
            List<UserActivity> daysUserActivities = monthUserActivities.Where(ua => ua.CreateDate >= graphDays.First()).ToList();
            for (int i = 0; i < graphDays.Count; i++)
            {
                DateTime time = graphDays[i];

                List<UserActivity> tempActivities = daysUserActivities.Where(ua => ua.CreateDate.Date == time || ua.EndDate == time).ToList();
                HashSet<string> tempUserIds = tempActivities.Select(ua => ua.UserId).ToHashSet();

                dayUserChartActiveUsers[i] = tempUserIds.Count;
            }

            // dayUserChartNewUsers
            List<User> monthUsers = users.Where(u => u.CreateDate.Date >= graphMonths.First()).ToList();
            List<User> daysUsers = monthUsers.Where(u => u.CreateDate.Date >= graphDays.First()).ToList();
            for (int i = 0; i < graphDays.Count; i++)
            {
                DateTime time = graphDays[i];

                List<User> tempUsers = daysUsers.Where(u => u.CreateDate.Date == time).ToList();

                dayUserChartNewUsers[i] = tempUsers.Count;
            }

            // dayUserChartUsers
            for (int i = 0; i < dayUserChartNewUsers.Length; i++)
            {
                dayUserCount += dayUserChartNewUsers[i];
                dayUserChartUsers[i] = dayUserCount;
            }

            // monthUserChartActiveUsers
            for (int i = 0; i < graphMonths.Count; i++)
            {
                DateTime time = graphMonths[i];

                List<UserActivity> tempActivities = monthUserActivities.Where(ua =>
                    (ua.CreateDate.Year == time.Year && ua.CreateDate.Month == time.Month) ||
                    (ua.EndDate?.Year == time.Year && ua.EndDate?.Month == time.Month)).ToList();
                HashSet<string> tempUserIds = tempActivities.Select(ua => ua.UserId).ToHashSet();

                monthUserChartActiveUsers[i] = tempUserIds.Count;
            }

            // monthUserChartNewUsers
            for (int i = 0; i < graphMonths.Count; i++)
            {
                DateTime time = graphMonths[i];

                List<User> tempUsers = monthUsers.Where(u => u.CreateDate.Year == time.Year && u.CreateDate.Month == time.Month).ToList();

                monthUserChartNewUsers[i] = tempUsers.Count;
            }

            // monthUserChartUsers
            for (int i = 0; i < monthUserChartNewUsers.Length; i++)
            {
                monthUserCount += monthUserChartNewUsers[i];
                monthUserChartUsers[i] = monthUserCount;
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
                await jsRef.InvokeVoidAsync("cUserChart.init",
                    dayLabels,
                    monthLabels,
                    dayUserChartUsers,
                    dayUserChartActiveUsers,
                    dayUserChartNewUsers,
                    monthUserChartUsers,
                    monthUserChartActiveUsers,
                    monthUserChartNewUsers);
            }
        }
    }
}
