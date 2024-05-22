using GosujiServer.Controllers;
using GosujiServer.Models.KataGo;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Shared
{
    public partial class RCKataGoWrapper : ComponentBase, IDisposable
    {
        [Inject]
        IJSRuntime JS { get; set; }
        [Inject]
        KataGoService kataGoService { get; set; }

        private KataGo? kataGo;

        [JSInvokable]
        public void ClearBoard()
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.ClearBoard");

            kataGo.ClearBoard();
        }

        [JSInvokable]
        public void Restart()
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.Restart");

            if (kataGo == null)
            {
                kataGo = kataGoService.Get("a");
            }

            kataGo.Restart();
        }

        [JSInvokable]
        public void SetBoardsize([RegularExpression(@"(9|13|19)")] string boardsize)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.SetBoardsize " + boardsize);

            kataGo.SetBoardsize(int.Parse(boardsize));
        }

        [JSInvokable]
        public void SetRuleset([RegularExpression(@"(Japanese|Chinese)")] string ruleset)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.SetRuleset " + ruleset);

            kataGo.SetRuleset(ruleset);
        }

        [JSInvokable]
        public void SetKomi([Range(-150, 150)] float komi)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.SetKomi " + komi);

            kataGo.SetKomi(komi);
        }

        [JSInvokable]
        public void SetHandicap([Range(2, 9)] int handicap)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.SetHandicap " + handicap);

            kataGo.SetHandicap(handicap);
        }

        [JSInvokable]
        public MoveSuggestion AnalyzeMove([RegularExpression(@"(B|W)")] string color,
            [RegularExpression(@"([A-H]|[J-T])(1[0-9]|[1-9])")] string coord)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.AnalyzeMove " + color + " " + coord);

            MoveSuggestion output = kataGo.AnalyzeMove(color, coord);
            //MoveSuggestion output = new(color, coord, 200, 0.8f, 1.5f);
            return output;
        }

        [JSInvokable]
        public List<MoveSuggestion> Analyze([RegularExpression(@"(B|W)")] string color,
            [Range(2, 25000)] int maxVisits,
            [Range(0, 100)] float minVisitsPerc,
            [Range(0, 100)] float maxVisitDiffPerc)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.Analyze " + color + " " + maxVisits + " " + minVisitsPerc + " " + maxVisitDiffPerc);

            List<MoveSuggestion> output = kataGo.Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
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
            if (G.Log) Console.WriteLine("RCKataGoWrapper.Play " + color + " " + coord);

            kataGo.Play(color, coord);
        }

        [JSInvokable]
        public void PlayRange(Moves moves)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.PlayRange " + moves);

            kataGo.PlayRange(moves);
        }

        [JSInvokable]
        public string SGF(bool shouldWriteFile)
        {
            if (G.Log) Console.WriteLine("RCKataGoWrapper.SGF " + shouldWriteFile);

            return kataGo.SGF(shouldWriteFile);
        }

        public void Dispose()
        {
            kataGoService.Return("a");
        }
    }
}
