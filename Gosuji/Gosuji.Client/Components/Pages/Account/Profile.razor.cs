using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Services;
using Gosuji.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text;

namespace Gosuji.Client.Components.Pages.Account
{
    public enum DaysChartDayTypes
    {
        NONE = 0,
        CAUGHT_UP = 1,
        PLAYED = 2,
    }

    public partial class Profile : CustomPage
    {
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private DataService dataService { get; set; }

        public string? name;
        public List<VMGame>? Games { get; set; }
        public List<VMGame>? FilteredGames { get; set; }

        private IJSObjectReference jsRef;
        private bool isChartsLoaded = false;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo($"login?{G.ReturnUriName}={Uri.EscapeDataString(navigationManager.Uri)}");
                return;
            }

            name = claimsPrincipal.Identity.Name;

            Games = [];

            int rangeStart = 1;
            int rangeStep = 500;
            List<VMGame>? tempGames;
            do
            {
                APIResponse<List<VMGame>> response = await dataService.GetUserGames(rangeStart, rangeStart + rangeStep - 1);
                if (!response.IsSuccess)
                {
                    G.StatusMessage.HandleAPIResponse(response);
                    break;
                }

                tempGames = response.Data;

                if (tempGames.Count != 0)
                {
                    Games.AddRange(tempGames);
                    rangeStart += rangeStep;
                }
            } while (tempGames.Count != 0);

            Games = Games.OrderByDescending(g => g.ModifyDate).ToList();

            //FinishedGames = Games.FindAll(g => g.IsFinished).ToList();
            FilteredGames = Games.ToList();
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

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/profile/bundle.js");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("profilePage.init", GameStat.RIGHT_COLOR.ToCSS(), GameStat.PERFECT_COLOR.ToCSS());
            }

            if (!isChartsLoaded && FilteredGames != null)
            {
                isChartsLoaded = true;

                CreateGameTable();
                CreatePercentPerGameLineChart();
                CreateGameStageBarChart();
                CreateDaysChart();
            }
        }

        public async Task DownloadSGF(string gameId)
        {
            APIResponse<Game> response = await dataService.GetGame(gameId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            Game? fullGame = response.Data;

            await js.InvokeVoidAsync("utils.downloadFile",
                fullGame.Name,
                "sgf",
                Encoding.UTF8.GetBytes(fullGame.SGF),
                "text/plain;charset=UTF-8");
        }

        private async Task CreateGameTable()
        {
            await jsRef.InvokeVoidAsync("profilePage.createGameTable");
        }

        private async Task CreatePercentPerGameLineChart()
        {
            List<int> rightPercents = [];
            List<int> perfectPercents = [];
            foreach (VMGame game in FilteredGames)
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

            for (int i = FilteredGames.Count - 1; i >= 0; i--)
            {
                VMGame game = FilteredGames[i];
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

            if (FilteredGames.Count == 0)
            {
                await jsRef.InvokeVoidAsync("profilePage.createDaysChart", days);
                return;
            }

            DateTime firstGameDate = FilteredGames[0].CreateDate.DateTime;
            for (DateTime day = firstGameDate.Date; day.Date <= DateTime.Now.Date; day = day.AddDays(1))
            {
                days[day.ToString("dd-MM-yy")] = DaysChartDayTypes.NONE;
            }

            int canCatchUpCount = 0;
            foreach (VMGame game in FilteredGames)
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

            DateTime lastGameDate = FilteredGames.Last().CreateDate.DateTime;
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
