using Gosuji.API.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.CMS.Components.Shared
{
    public partial class CUserChart : ComponentBase
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

        private int[] dayChartUsers;
        private int[] dayChartActiveUsers;
        private int[] dayChartNewUsers;
        private int[] monthChartUsers;
        private int[] monthChartActiveUsers;
        private int[] monthChartNewUsers;

        protected override async Task OnInitializedAsync()
        {
            int dayCount = GraphDays.Count;
            int monthCount = GraphMonths.Count;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            List<UserActivity> monthUserActivities = (await dbContext.UserActivities.ToListAsync()).Where(ua => ua.CreateDate.Date >= GraphMonths.First()).ToList();
            List<User> users = await dbContext.Users.ToListAsync();

            await dbContext.DisposeAsync();

            int dayUserCount = users.Count(u => u.CreateDate.Date < GraphDays.First());
            int monthUserCount = users.Count(u => u.CreateDate.Date < GraphMonths.First());

            // dayChartActiveUsers
            dayChartActiveUsers = new int[dayCount];
            List<UserActivity> daysUserActivities = monthUserActivities.Where(ua => ua.CreateDate >= GraphDays.First()).ToList();
            for (int i = 0; i < GraphDays.Count; i++)
            {
                DateTime time = GraphDays[i];

                List<UserActivity> tempActivities = daysUserActivities.Where(ua => ua.CreateDate.Date == time || ua.EndDate == time).ToList();
                HashSet<string> tempUserIds = tempActivities.Select(ua => ua.UserId).ToHashSet();

                dayChartActiveUsers[i] = tempUserIds.Count;
            }

            List<User> monthUsers = users.Where(u => u.CreateDate.Date >= GraphMonths.First()).ToList();

            // dayChartNewUsers
            dayChartNewUsers = new int[dayCount];
            List<User> daysUsers = monthUsers.Where(u => u.CreateDate.Date >= GraphDays.First()).ToList();
            for (int i = 0; i < GraphDays.Count; i++)
            {
                DateTime time = GraphDays[i];
                dayChartNewUsers[i] = daysUsers.Count(u => u.CreateDate.Date == time);
            }

            // dayChartUsers
            dayChartUsers = new int[dayCount];
            for (int i = 0; i < dayChartNewUsers.Length; i++)
            {
                dayUserCount += dayChartNewUsers[i];
                dayChartUsers[i] = dayUserCount;
            }

            // monthChartActiveUsers
            monthChartActiveUsers = new int[monthCount];
            for (int i = 0; i < GraphMonths.Count; i++)
            {
                DateTime time = GraphMonths[i];

                List<UserActivity> tempActivities = monthUserActivities.Where(ua =>
                    (ua.CreateDate.Year == time.Year && ua.CreateDate.Month == time.Month) ||
                    (ua.EndDate?.Year == time.Year && ua.EndDate?.Month == time.Month)).ToList();
                HashSet<string> tempUserIds = tempActivities.Select(ua => ua.UserId).ToHashSet();

                monthChartActiveUsers[i] = tempUserIds.Count;
            }

            // monthChartNewUsers
            monthChartNewUsers = new int[monthCount];
            for (int i = 0; i < GraphMonths.Count; i++)
            {
                DateTime time = GraphMonths[i];
                monthChartNewUsers[i] = monthUsers.Count(u => u.CreateDate.Year == time.Year && u.CreateDate.Month == time.Month);
            }

            // monthChartUsers
            monthChartUsers = new int[monthCount];
            for (int i = 0; i < monthChartNewUsers.Length; i++)
            {
                monthUserCount += monthChartNewUsers[i];
                monthChartUsers[i] = monthUserCount;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await js.InvokeVoidAsync("cUserChart.init",
                    DayLabels,
                    MonthLabels,
                    dayChartUsers,
                    dayChartActiveUsers,
                    dayChartNewUsers,
                    monthChartUsers,
                    monthChartActiveUsers,
                    monthChartNewUsers);
            }
        }
    }
}
