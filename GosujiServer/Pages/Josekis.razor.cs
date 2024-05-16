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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                josekiService.AddSession();

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

            foreach (Mark mark in node.Marks)
            {
                await AddMark(mark);
            }

            foreach (TextLabel textLabel in node.Labels)
            {
                await AddLabel(textLabel);
            }

            await JS.InvokeVoidAsync($"{BOARD}.redraw");

            await JS.InvokeVoidAsync($"{EDITOR}.setTool", "cross");
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
