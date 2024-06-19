using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Gosuji.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
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
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDataService dataService { get; set; }

        public VMGame[] Games { get; set; }
        public VMGame[] FinishedGames { get; set; }

        private IJSObjectReference jsRef;
        private bool isGamesFilled = false;

        public async Task DownloadSGF(long gameId)
        {
            Game? fullGame = await dataService.GetGame(gameId);
            if (fullGame == null)
            {
                return;
            }

            await jsRef.InvokeVoidAsync("profilePage.downloadSGF", fullGame.Name, fullGame.SGF);
        }

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo("Account/Login");
                return;
            }

            if (claimsPrincipal.Identity.Name != Name)
            {
                navigationManager.NavigateTo("user/" + claimsPrincipal.Identity.Name);
                return;
            }

            Games = await dataService.GetUserGames(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", G.CSSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["JQuery"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["ChartJS"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/profile/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("profilePage.init", GameStat.RIGHT_COLOR.ToCSS(), GameStat.PERFECT_COLOR.ToCSS());
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
            await jsRef.InvokeVoidAsync("profilePage.createGameTable");
        }

        private async Task CreatePercentPerGameLineChart()
        {
            List<int> rightPercents = [];
            List<int> perfectPercents = [];
            foreach (VMGame game in FinishedGames)
            {
                if (game.GameStat == null || game.GameStat.Total < 5)
                {
                    continue;
                }

                rightPercents.Add(game.GameStat.RightPercent);
                perfectPercents.Add(game.GameStat.PerfectPercent);
            }

            await jsRef.InvokeVoidAsync("profilePage.createPercentPerGameLineChart", rightPercents, perfectPercents);
        }

        private async Task CreateGameStageBarChart()
        {
            int rightOpenings = 0, rightMidgames = 0, rightEndgames = 0;
            int rightOpening = 0, rightMidgame = 0, rightEndgame = 0;

            int perfectOpenings = 0, perfectMidgames = 0, perfectEndgames = 0;
            int perfectOpening = 0, perfectMidgame = 0, perfectEndgame = 0;

            for (int i = FinishedGames.Length - 1; i >= 0; i--)
            {
                VMGame game = FinishedGames[i];
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

            await jsRef.InvokeVoidAsync("profilePage.createGameStageBarChart",
                new int[] { rightOpening, rightMidgame, rightEndgame },
                new int[] { perfectOpening, perfectMidgame, perfectEndgame });
        }

        private async Task CreateDaysChart()
        {
            Dictionary<string, DaysChartDayTypes> days = [];

            if (FinishedGames.Length == 0)
            {
                await jsRef.InvokeVoidAsync("profilePage.createDaysChart", days);
                return;
            }

            DateTime firstGameDate = FinishedGames[0].CreateDate.DateTime;
            for (DateTime day = firstGameDate.Date; day.Date <= DateTime.Now.Date; day = day.AddDays(1))
            {
                days[day.ToString("dd-MM-yy")] = DaysChartDayTypes.NONE;
            }

            int canCatchUpCount = 0;
            foreach (VMGame game in FinishedGames)
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
                if (canCatchUpCount == 0)
                {
                    break;
                }

                string date = day.ToString("dd-MM-yy");
                if (days[date] != DaysChartDayTypes.NONE)
                {
                    continue;
                }

                days[date] = DaysChartDayTypes.CAUGHT_UP;

                canCatchUpCount--;
            }

            await jsRef.InvokeVoidAsync("profilePage.createDaysChart", days);
        }
    }
}
