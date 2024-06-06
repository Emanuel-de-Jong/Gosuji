using Gosuji.Client.Components.Pages;
using Gosuji.Client;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace Gosuji.Components.Pages.CMS
{
    public partial class Stats : ComponentBase
    {
        private static int DAY_COUNT = 14;
        private static int MONTH_COUNT = 12;

        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;

        private List<DateTime> graphDays = [];
        private List<DateTime> graphMonths = [];

        private List<string> dayLabels = [];
        private List<string> monthLabels = [];

        private List<UserActivity> userActivities;
        private List<User> users;

        private int[] dayUserChartUsers = new int[DAY_COUNT];
        private int[] dayUserChartNewUsers = new int[DAY_COUNT];
        private int[] monthUserChartUsers = new int[MONTH_COUNT];
        private int[] monthUserChartNewUsers = new int[MONTH_COUNT];

        protected override async Task OnInitializedAsync()
        {
            DateTime dateNow = DateTimeOffset.UtcNow.Date;
            for (int i = DAY_COUNT - 1; i >= 0; i--)
            {
                graphDays.Add(dateNow.AddDays(-i));
            }

            dateNow = new(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1);
            for (int i = MONTH_COUNT - 1; i >= 0; i--)
            {
                graphMonths.Add(dateNow.AddMonths(-i));
            }

            foreach (DateTime day in graphDays)
            {
                dayLabels.Add(day.ToString("dd-MM"));
            }

            foreach (DateTime month in graphMonths)
            {
                monthLabels.Add(month.ToString("MMMM"));
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            userActivities = (await dbContext.UserActivities.ToListAsync()).Where(ua => ua.CreateDate.Date >= graphMonths.First()).ToList();
            users = (await dbContext.Users.ToListAsync()).Where(u => u.CreateDate.Date >= graphMonths.First()).ToList();

            List<UserActivity> daysUserActivities = userActivities.Where(ua => ua.CreateDate >= graphDays.First()).ToList();
            for (int i = 0; i < graphDays.Count; i++)
            {
                DateTime time = graphDays[i];

                List<UserActivity> dayActivities = daysUserActivities.Where(ua => ua.CreateDate.Date == time || ua.EndDate == time).ToList();
                HashSet<string> dayUsers = dayActivities.Select(ua => ua.UserId).ToHashSet();

                dayUserChartUsers[i] = dayUsers.Count;
            }

            List<User> daysUsers = users.Where(u => u.CreateDate.Date >= graphDays.First()).ToList();
            for (int i = 0; i < graphDays.Count; i++)
            {
                DateTime time = graphDays[i];

                List<User> dayUsers = daysUsers.Where(u => u.CreateDate.Date == time).ToList();

                dayUserChartNewUsers[i] = dayUsers.Count;
            }

            for (int i = 0; i < graphMonths.Count; i++)
            {
                DateTime time = graphMonths[i];

                List<UserActivity> monthActivities = userActivities.Where(ua =>
                    (ua.CreateDate.Year == time.Year && ua.CreateDate.Month == time.Month) ||
                    (ua.EndDate?.Year == time.Year && ua.EndDate?.Month == time.Month)).ToList();
                HashSet<string> monthUsers = monthActivities.Select(ua => ua.UserId).ToHashSet();

                monthUserChartNewUsers[i] = monthUsers.Count;
            }

            for (int i = 0; i < graphMonths.Count; i++)
            {
                DateTime time = graphMonths[i];

                List<User> monthUsers = users.Where(u => u.CreateDate.Year == time.Year && u.CreateDate.Month == time.Month).ToList();

                monthUserChartUsers[i] = monthUsers.Count;
            }

            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", "css/cms.css");
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["ChartJS"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("stats.init",
                    dayLabels,
                    monthLabels,
                    dayUserChartUsers,
                    dayUserChartNewUsers,
                    monthUserChartUsers,
                    monthUserChartNewUsers);
            }
        }
    }
}
