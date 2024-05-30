using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gosuji.Client.Components.Pages
{
    public partial class Josekis : ComponentBase, IDisposable
    {
        private static Random random = new();

        private const string BOARD = "josekisPage.board";
        private const string EDITOR = $"{BOARD}.editor";

        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IJosekisService josekisService { get; set; }

        private int sessionId;
        private DotNetObjectReference<Josekis>? josekisRef;

        public string[]? Comment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            sessionId = random.Next(100_000_000, 999_999_999);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await josekisService.AddSession(sessionId);

                josekisRef = DotNetObjectReference.Create(this);

                await js.InvokeVoidAsync("josekisPage.init", josekisRef);

                await AddMarkups();
            }
        }

        private async Task Play()
        {
            JosekisNode node = await josekisService.Current(sessionId);
            if (node == null)
            {
                return;
            }

            await js.InvokeVoidAsync($"{EDITOR}.setTool", node.IsBlack ? "playB" : "playW");
            await js.InvokeVoidAsync($"{EDITOR}.click", node.X + 1, node.Y + 1, false, false);

            await AddMarkups(node);

            await js.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
        }

        private async Task AddMarkups()
        {
            await AddMarkups(await josekisService.Current(sessionId));
        }

        private async Task AddMarkups(JosekisNode node)
        {
            if (node.Marks != null)
            {
                foreach (JosekisMark mark in node.Marks)
                {
                    await AddMark(mark);
                }
            }

            if (node.Labels != null)
            {
                foreach (JosekisLabel label in node.Labels)
                {
                    await AddLabel(label);
                }
            }

            if (node.Comment != null)
            {
                Comment = node.Comment.Split("\r\n");
                StateHasChanged();
            }

            await js.InvokeVoidAsync($"{BOARD}.redraw");
        }

        private async Task AddMark(JosekisMark mark)
        {
            int markId = 0;
            switch (mark.MarkType)
            {
                case JosekisMarkType.Circle:
                    markId = 1;
                    break;
                case JosekisMarkType.Square:
                    markId = 2;
                    break;
                case JosekisMarkType.Triangle:
                    markId = 3;
                    break;
                case JosekisMarkType.Mark:
                    //markId = 4;
                    markId = 2; // Cross is used for selecting
                    break;
            }

            await js.InvokeVoidAsync($"{BOARD}.addMarkup", mark.X + 1, mark.Y + 1, markId);
        }

        private async Task AddLabel(JosekisLabel textLabel)
        {
            await js.InvokeVoidAsync($"{BOARD}.addMarkup", textLabel.X + 1, textLabel.Y + 1, textLabel.Text);
        }

        [JSInvokable]
        public async Task Pass()
        {
            await josekisService.ToChild(sessionId, new JosekisNode(20, 20));
            await Play();
        }

        [JSInvokable]
        public async Task Prev()
        {
            await josekisService.ToParent(sessionId);
            await js.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task LastBranch()
        {
            int returnCount = await josekisService.ToLastBranch(sessionId);
            await js.InvokeVoidAsync($"{EDITOR}.prevNode", returnCount);

            await js.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task First()
        {
            await josekisService.ToFirst(sessionId);
            await js.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task Next(int x, int y)
        {
            if (!await josekisService.ToChild(sessionId, new JosekisNode(x, y)))
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
