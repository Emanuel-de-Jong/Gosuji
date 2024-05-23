using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Security.Claims;

namespace GosujiServer.Pages
{
    public enum DaysChartDayTypes
    {
        NONE = 0,
        CAUGHT_UP = 1,
        PLAYED = 2,
    }

    public partial class Profile : ComponentBase
    {
        [Parameter]
        public string? Name { get; set; }

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private UserManager<User> userManager { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        public List<Game>? Games { get; set; }
        public List<Game>? FinishedGames { get; set; }

        private ApplicationDbContext? context;
        private User? user;
        private bool isGamesFilled = false;

        public async Task DownloadSGF(long gameId)
        {
            foreach (Game game in Games)
            {
                if (game.Id == gameId)
                {
                    await JS.InvokeVoidAsync("profilePage.downloadSGF", game.Name, game.SGF);
                    break;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (G.LogRouting) Console.WriteLine("Profile.razor");

            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo("Identity/Account/Login");
                return;
            }

            User user = await userManager.GetUserAsync(claimsPrincipal);
            if (user.UserName != Name && !await userManager.IsInRoleAsync(user, "Owner"))
            {
                navigationManager.NavigateTo("user/" + user.UserName);
                return;
            }

            context = dbService.GetContext();

            user = await context.Users.Where(u => u.UserName == Name).FirstOrDefaultAsync();
            Games = await context.Games
                .Where(g => g.UserId == user.Id)
                .Where(g => g.IsDeleted == false)
                .Include(g => g.GameStat)
                .Include(g => g.OpeningStat)
                .Include(g => g.MidgameStat)
                .Include(g => g.EndgameStat)
                .ToListAsync();

            await context.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("profilePage.init", GameStat.RIGHT_COLOR.ToCSS(), GameStat.PERFECT_COLOR.ToCSS());
            }

            if (!isGamesFilled && Games != null)
            {
                isGamesFilled = true;
                //FinishedGames = Games.FindAll(g => g.IsFinished).ToList();
                FinishedGames = Games;

                CreateGameTable();
                CreatePercentPerGameLineChart();
                CreateGameStageBarChart();
                CreateDaysChart();
            }
        }

        private async Task CreateGameTable()
        {
            await JS.InvokeVoidAsync("profilePage.createGameTable");
        }

        private async Task CreatePercentPerGameLineChart()
        {
            List<int> rightPercents = new();
            List<int> perfectPercents = new();
            foreach (Game game in FinishedGames)
            {
                if (game.GameStat.Total < 5) continue;

                rightPercents.Add(game.GameStat.RightPercent);
                perfectPercents.Add(game.GameStat.PerfectPercent);
            }

            await JS.InvokeVoidAsync("profilePage.createPercentPerGameLineChart", rightPercents, perfectPercents);
        }

        private async Task CreateGameStageBarChart()
        {
            int rightOpenings = 0, rightMidgames = 0, rightEndgames = 0;
            int rightOpening = 0, rightMidgame = 0, rightEndgame = 0;

            int perfectOpenings = 0, perfectMidgames = 0, perfectEndgames = 0;
            int perfectOpening = 0, perfectMidgame = 0, perfectEndgame = 0;

            for (int i = FinishedGames.Count - 1; i >= 0; i--)
            {
                Game game = FinishedGames[i];
                if (rightOpenings < 5 && game.OpeningStat != null && game.OpeningStat.Right >= 5)
                {
                    rightOpenings++;
                    rightOpening += game.OpeningStat.RightPercent;
                }
                if (rightMidgames < 5 && game.MidgameStat != null && game.MidgameStat.Right >= 5)
                {
                    rightMidgames++;
                    rightMidgame += game.MidgameStat.RightPercent;
                }
                if (rightEndgames < 5 && game.EndgameStat != null && game.EndgameStat.Right >= 5)
                {
                    rightEndgames++;
                    rightEndgame += game.EndgameStat.RightPercent;
                }

                if (perfectOpenings < 5 && game.OpeningStat != null && game.OpeningStat.Perfect >= 5)
                {
                    perfectOpenings++;
                    perfectOpening += game.OpeningStat.PerfectPercent;
                }
                if (perfectMidgames < 5 && game.MidgameStat != null && game.MidgameStat.Perfect >= 5)
                {
                    perfectMidgames++;
                    perfectMidgame += game.MidgameStat.PerfectPercent;
                }
                if (perfectEndgames < 5 && game.EndgameStat != null && game.EndgameStat.Perfect >= 5)
                {
                    perfectEndgames++;
                    perfectEndgame += game.EndgameStat.PerfectPercent;
                }
            }

            rightOpening = rightOpening > 0 ? (int)Math.Round((double)rightOpening / rightOpenings) : 0;
            rightMidgame = rightMidgame > 0 ? (int)Math.Round((double)rightMidgame / rightMidgames) : 0;
            rightEndgame = rightEndgame > 0 ? (int)Math.Round((double)rightEndgame / rightEndgames) : 0;

            perfectOpening = perfectOpening > 0 ? (int)Math.Round((double)perfectOpening / perfectOpenings) : 0;
            perfectMidgame = perfectMidgame > 0 ? (int)Math.Round((double)perfectMidgame / perfectMidgames) : 0;
            perfectEndgame = perfectEndgame > 0 ? (int)Math.Round((double)perfectEndgame / perfectEndgames) : 0;

            await JS.InvokeVoidAsync("profilePage.createGameStageBarChart",
                new int[] { rightOpening, rightMidgame, rightEndgame },
                new int[] { perfectOpening, perfectMidgame, perfectEndgame });
        }

        private async Task CreateDaysChart()
        {
            Dictionary<string, DaysChartDayTypes> days = new();

            if (FinishedGames.Count == 0)
            {
                await JS.InvokeVoidAsync("profilePage.createDaysChart", days);
                return;
            }

            DateTime firstGameDate = FinishedGames[0].CreateDate.DateTime;
            for (DateTime day = firstGameDate.Date; day.Date <= DateTime.Now.Date; day = day.AddDays(1))
            {
                days[day.ToString("dd-MM-yy")] = DaysChartDayTypes.NONE;
            }

            int canCatchUpCount = 0;
            foreach (Game game in FinishedGames)
            {
                string date = game.CreateDate.ToString("dd-MM-yy");
                if (days[date] == DaysChartDayTypes.NONE)
                {
                    days[date] = DaysChartDayTypes.PLAYED;
                }
                else
                {
                    canCatchUpCount++;
                }
            }

            DateTime lastGameDate = FinishedGames.Last().CreateDate.DateTime;
            for (DateTime day = firstGameDate.Date; day.Date <= lastGameDate.Date; day = day.AddDays(1))
            {
                if (canCatchUpCount == 0) break;

                string date = day.ToString("dd-MM-yy");
                if (days[date] != DaysChartDayTypes.NONE) continue;
                days[date] = DaysChartDayTypes.CAUGHT_UP;

                canCatchUpCount--;
            }

            await JS.InvokeVoidAsync("profilePage.createDaysChart", days);
        }
    }
}
