using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Services;
using IGOEnchi.GoGameLogic;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Xml.Linq;
using GosujiServer.Models.GoGameWraps;
using GosujiServer.Models;

namespace GosujiServer.Pages
{
    public partial class Josekis : ComponentBase, IDisposable
    {
        private static Random random = new Random();

        private const string BOARD = "josekisPage.board";
        private const string EDITOR = $"{BOARD}.editor";

        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private JosekisService josekisService { get; set; }

        private int sessionId;
        private DotNetObjectReference<Josekis>? josekisRef;

        protected override async Task OnInitializedAsync()
        {
            sessionId = random.Next(100_000_000, 999_999_999);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                josekisService.AddSession(sessionId);

                josekisRef = DotNetObjectReference.Create(this);

                await JS.InvokeVoidAsync("josekisPage.init", josekisRef);

                await AddMarkups();
            }
        }

        private async Task Play()
        {
            JosekisNode node = josekisService.Current(sessionId);
            if (node == null)
            {
                return;
            }

            await JS.InvokeVoidAsync($"{EDITOR}.setTool", node.IsBlack ? "playB" : "playW");
            await JS.InvokeVoidAsync($"{EDITOR}.click", node.X + 1, node.Y + 1, false, false);

            await AddMarkups();

            await JS.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
        }

        private async Task AddMarkups()
        {
            JosekisNode node = josekisService.Current(sessionId);

            foreach (Mark mark in node.Marks)
            {
                await AddMark(mark);
            }

            foreach (TextLabel textLabel in node.Labels)
            {
                await AddLabel(textLabel);
            }

            await JS.InvokeVoidAsync($"{EDITOR}.setComment", node.Comment);

            await JS.InvokeVoidAsync($"{BOARD}.redraw");
        }

        private async Task AddMark(Mark mark)
        {
            int markId = 0;
            switch (mark.MarkType)
            {
                case MarkType.Circle:
                    markId = 1;
                    break;
                case MarkType.Square:
                    markId = 2;
                    break;
                case MarkType.Triangle:
                    markId = 3;
                    break;
                case MarkType.Mark:
                    //markId = 4;
                    markId = 2; // Cross is used for selecting
                    break;
            }

            await JS.InvokeVoidAsync($"{BOARD}.addMarkup", mark.X + 1, mark.Y + 1, markId);
        }

        private async Task AddLabel(TextLabel textLabel)
        {
            await JS.InvokeVoidAsync($"{BOARD}.addMarkup", textLabel.X + 1, textLabel.Y + 1, textLabel.Text);
        }

        [JSInvokable]
        public async Task Pass()
        {
            josekisService.ToChild(sessionId, new JosekisNode(20, 20));
            await Play();
        }

        [JSInvokable]
        public async Task Prev()
        {
            josekisService.ToParent(sessionId);
            await JS.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task LastBranch()
        {
            int returnCount = josekisService.ToLastBranch(sessionId);
            await JS.InvokeVoidAsync($"{EDITOR}.prevNode", returnCount);

            await JS.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task First()
        {
            josekisService.ToFirst(sessionId);
            await JS.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task Next(int x, int y)
        {
            if (!josekisService.ToChild(sessionId, new JosekisNode(x, y)))
            {
                return;
            }

            await Play();
        }

        public void Dispose()
        {
            josekisRef?.Dispose();
            josekisService.RemoveSession(sessionId);
        }
    }
}
