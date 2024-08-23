using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models.Josekis;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages
{
    public partial class Josekis : CustomPage, IAsyncDisposable
    {
        private static Random random = new();

        private const string BOARD = "josekisPage.board";
        private const string EDITOR = $"{BOARD}.editor";

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private JosekisService josekisService { get; set; }
        [Inject]
        private DataService dataService { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }

        private DotNetObjectReference<Josekis>? josekisRef;
        private int sessionId;
        private IJSObjectReference jsRef;

        private bool isJSInitialized = false;

        public string[]? Comment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                await settingConfigService.InitSettingConfig();
            }
            else
            {
                await settingConfigService.SettingConfigFromDb();
            }

            sessionId = random.Next(100_000_000, 999_999_999);
            josekisRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/josekis/bundle.js");

            if (josekisRef != null && !isJSInitialized)
            {
                isJSInitialized = true;

                await jsRef.InvokeVoidAsync("josekisPage.init", josekisRef, settingConfigService.SettingConfig.CalcStoneVolume());
            }
        }

        private async Task<bool> Start()
        {
            if (josekisService.IsConnected)
            {
                return true;
            }

            APIResponse startResponse = await josekisService.Start();
            if (G.StatusMessage.HandleAPIResponse(startResponse)) return false;

            APIResponse response = await josekisService.AddSession(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return false;

            return true;
        }

        private async Task Play()
        {
            APIResponse<JosekisNode> response = await josekisService.Current(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            JosekisNode node = response.Data;

            await jsRef.InvokeVoidAsync($"{EDITOR}.setTool", node.IsBlack ? "playB" : "playW");
            await jsRef.InvokeVoidAsync($"{EDITOR}.click", node.X + 1, node.Y + 1, false, false);
            await jsRef.InvokeVoidAsync($"{BOARD}.playPlaceStoneAudio");

            await AddMarkups(node);

            await jsRef.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
        }

        private async Task AddMarkups()
        {
            APIResponse<JosekisNode> response = await josekisService.Current(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;

            await AddMarkups(response.Data);
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
            int color = mark.MarkType == JosekisMarkType.MARK ? -1 : 1;

            await jsRef.InvokeVoidAsync($"{BOARD}.addGhostStone", mark.X + 1, mark.Y + 1, color);
        }

        private async Task AddMark(JosekisMark mark)
        {
            int markId = 0;
            switch (mark.MarkType)
            {
                case JosekisMarkType.CIRCLE:
                    markId = 1;
                    break;
                case JosekisMarkType.SQUARE:
                    markId = 2;
                    break;
                case JosekisMarkType.TRIANGLE:
                    markId = 3;
                    break;
                case JosekisMarkType.MARK:
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
            if (!await Start()) return;

            APIResponse response = await josekisService.ToChild(sessionId, new JosekisNode(20, 20));
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            
            await Play();
        }

        [JSInvokable]
        public async Task Prev()
        {
            if (!await Start()) return;

            APIResponse response = await josekisService.ToParent(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            
            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task LastBranch()
        {
            if (!await Start()) return;

            APIResponse<int> response = await josekisService.ToLastBranch(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            int returnCount = response.Data;

            await jsRef.InvokeVoidAsync($"{EDITOR}.prevNode", returnCount);

            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task First()
        {
            if (!await Start()) return;

            APIResponse response = await josekisService.ToFirst(sessionId);
            if (G.StatusMessage.HandleAPIResponse(response)) return;

            await jsRef.InvokeVoidAsync($"{BOARD}.clearFuture");
            await AddMarkups();
        }

        [JSInvokable]
        public async Task Next(int x, int y)
        {
            if (!await Start()) return;

            APIResponse<bool> response = await josekisService.ToChild(sessionId, new JosekisNode(x, y));
            if (G.StatusMessage.HandleAPIResponse(response)) return;

            if (!response.Data)
            {
                return;
            }

            await Play();
        }

        public async ValueTask DisposeAsync()
        {
            josekisRef?.Dispose();

            if (josekisService.IsConnected)
            {
                await josekisService.RemoveSession(sessionId);
                await josekisService.Stop();
            }
        }
    }
}
