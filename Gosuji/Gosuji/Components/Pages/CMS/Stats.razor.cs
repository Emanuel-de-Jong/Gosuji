using Microsoft.AspNetCore.Components;

namespace Gosuji.Components.Pages.CMS
{
    public partial class Stats : ComponentBase
    {
        private static int DAY_COUNT = 14;
        private static int MONTH_COUNT = 13;

        private List<DateTime> graphDays = [];
        private List<DateTime> graphMonths = [];

        private List<string> dayLabels = [];
        private List<string> monthLabels = [];

        protected override async Task OnInitializedAsync()
        {
            // graphDays
            DateTime dateNow = DateTimeOffset.UtcNow.Date;
            for (int i = DAY_COUNT - 1; i >= 0; i--)
            {
                graphDays.Add(dateNow.AddDays(-i));
            }

            // graphMonths
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
        }
    }
}
