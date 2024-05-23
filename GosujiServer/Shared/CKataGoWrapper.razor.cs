using GosujiServer.Areas.Identity.Data;
using GosujiServer.Controllers;
using GosujiServer.Data;
using GosujiServer.Models.KataGo;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GosujiServer.Shared
{
    public partial class CKataGoWrapper : ComponentBase, IDisposable
    {
        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        KataGoService kataGoService { get; set; }

        private string? userId;
        private KataGo? kataGo;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }

        [JSInvokable]
        public void ClearBoard()
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.ClearBoard");

            kataGo?.ClearBoard();
        }

        [JSInvokable]
        public bool Restart()
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.Restart");

            if (kataGo == null)
            {
                kataGo = kataGoService.Get(userId);

                if (kataGo == null)
                {
                    return false;
                }
            }

            kataGo?.Restart();

            return true;
        }

        [JSInvokable]
        public void SetBoardsize([RegularExpression(@"(9|13|19)")] string boardsize)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.SetBoardsize " + boardsize);

            kataGo?.SetBoardsize(int.Parse(boardsize));
        }

        [JSInvokable]
        public void SetRuleset([RegularExpression(@"(Japanese|Chinese)")] string ruleset)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.SetRuleset " + ruleset);

            kataGo?.SetRuleset(ruleset);
        }

        [JSInvokable]
        public void SetKomi([Range(-150, 150)] float komi)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.SetKomi " + komi);

            kataGo?.SetKomi(komi);
        }

        [JSInvokable]
        public void SetHandicap([Range(2, 9)] int handicap)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.SetHandicap " + handicap);

            kataGo?.SetHandicap(handicap);
        }

        [JSInvokable]
        public MoveSuggestion? AnalyzeMove([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.AnalyzeMove " + color + " " + coord);

            MoveSuggestion? output = kataGo?.AnalyzeMove(color, coord);
            //MoveSuggestion output = new(color, coord, 200, 0.8f, 1.5f);
            return output;
        }

        [JSInvokable]
        public List<MoveSuggestion>? Analyze([RegularExpression(@"(B|W)")] string color,
            [Range(2, 25000)] int maxVisits,
            [Range(0, 100)] float minVisitsPerc,
            [Range(0, 100)] float maxVisitDiffPerc)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.Analyze " + color + " " + maxVisits + " " + minVisitsPerc + " " + maxVisitDiffPerc);

            List<MoveSuggestion>? output = kataGo?.Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
            //List<MoveSuggestion> output = new()
            //{
            //    new MoveSuggestion(color, "A1", 200, 80_000_000, 15_000_000),
            //    new MoveSuggestion(color, "B2", 200, 80_000_000, 15_000_000)
            //};
            return output;
        }

        [JSInvokable]
        public void Play([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.Play " + color + " " + coord);

            kataGo?.Play(color, coord);
        }

        [JSInvokable]
        public void PlayRange(Moves moves)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.PlayRange " + moves);

            kataGo?.PlayRange(moves);
        }

        [JSInvokable]
        public string? SGF(bool shouldWriteFile)
        {
            if (G.Log) Console.WriteLine("CKataGoWrapper.SGF " + shouldWriteFile);

            return kataGo?.SGF(shouldWriteFile);
        }

        public void Dispose()
        {
            kataGoService.Return(userId).Wait();
        }
    }
}
