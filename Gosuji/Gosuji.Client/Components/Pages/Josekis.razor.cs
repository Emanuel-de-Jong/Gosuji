using Gosuji.Client.Data;
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
        private JosekisService josekisService { get; set; }
        [Inject]
        private DataService dataService { get; set; }

        private DotNetObjectReference<Josekis>? josekisRef;
        private int sessionId;
        private IJSObjectReference jsRef;

        private SettingConfig? settingConfig;
        private bool isJSInitialized = false;

        public string[]? Comment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            josekisRef = DotNetObjectReference.Create(this);
            sessionId = random.Next(100_000_000, 999_999_999);

            settingConfig = await dataService.GetSettingConfig();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/josekis/bundle.js");

            if (firstRender)
            {
                await josekisService.AddSession(sessionId);
            }

            if (settingConfig != null && !isJSInitialized)
            {
                isJSInitialized = true;

                await jsRef.InvokeVoidAsync("josekisPage.init", josekisRef, settingConfig.CalcStoneVolume());
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

            await jsRef.InvokeVoidAsync($"{EDITOR}.setTool", node.IsBlack ? "playB" : "playW");
            await jsRef.InvokeVoidAsync($"{EDITOR}.click", node.X + 1, node.Y + 1, false, false);
            await jsRef.InvokeVoidAsync($"{BOARD}.playPlaceStoneAudio");

            await AddMarkups(node);

            await jsRef.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
        }

        private async Task AddMarkups()
        {
            await AddMarkups(await josekisService.Current(sessionId));
        }

        private async Task AddMarkups(JosekisNode node)
        {
            await RemoveMarkups();

            if (node.Marks != null)
            {
                foreach (JosekisMark mark in node.Marks)
                {
                    await AddGhostStone(mark);
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

            await jsRef.InvokeVoidAsync($"{BOARD}.redraw");
        }

        private async Task AddGhostStone(JosekisMark mark)
        {
            int color = mark.MarkType == JosekisMarkType.Mark ? -1 : 1;

            await jsRef.InvokeVoidAsync($"{BOARD}.addGhostStone", mark.X + 1, mark.Y + 1, color);
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

            await jsRef.InvokeVoidAsync($"{BOARD}.addMarkup", mark.X + 1, mark.Y + 1, markId);
        }

        private async Task AddLabel(JosekisLabel textLabel)
        {
            await jsRef.InvokeVoidAsync($"{BOARD}.addMarkup", textLabel.X + 1, textLabel.Y + 1, textLabel.Text);
        }

        private async Task RemoveMarkups()
        {
            await jsRef.InvokeVoidAsync($"{BOARD}.removeGhostStones");
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
            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task LastBranch()
        {
            int returnCount = (await josekisService.ToLastBranch(sessionId)).Value;
            await jsRef.InvokeVoidAsync($"{EDITOR}.prevNode", returnCount);

            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task First()
        {
            await josekisService.ToFirst(sessionId);
            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task Next(int x, int y)
        {
            if (!(await josekisService.ToChild(sessionId, new JosekisNode(x, y))).Value)
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
