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
        private const string BOARD = "josekisPage.board";
        private const string EDITOR = $"{BOARD}.editor";

        private DotNetObjectReference<Josekis>? josekisRef;

        protected override async Task OnInitializedAsync()
        {
            josekiService.AddSession();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                josekisRef = DotNetObjectReference.Create(this);

                await JS.InvokeVoidAsync("josekisPage.init", josekisRef);

                for (int i = 0; i < 4; i++)
                {
                    josekiService.ToChild(josekiService.Children()[0]);
                    await Play();
                }
            }
        }

        private async Task Play()
        {
            JosekisNode? node = josekiService.Current();
            if (node == null)
            {
                return;
            }

            await JS.InvokeVoidAsync($"{EDITOR}.setTool", node.IsBlack ? "playB" : "playW");
            await JS.InvokeVoidAsync($"{EDITOR}.click", node.X + 1, node.Y + 1, false, false);
            await JS.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
        }

        [JSInvokable]
        public void PrevNodeClickListener()
        {
            josekiService.ToParent();
        }

        [JSInvokable]
        public async Task CrossPlacedListener(int x, int y)
        {
            if (!josekiService.ToChild(new JosekisNode(x, y)))
            {
                return;
            }

            await Play();
        }

        public void Dispose()
        {
            josekisRef?.Dispose();
            josekiService.RemoveSession();
        }
    }
}
