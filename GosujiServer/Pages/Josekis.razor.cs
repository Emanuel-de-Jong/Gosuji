using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Services;
using IGOEnchi.GoGameLogic;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Xml.Linq;
using GosujiServer.Models.GoGameWraps;

namespace GosujiServer.Pages
{
    public partial class Josekis : ComponentBase, IDisposable
    {
        protected override async Task OnInitializedAsync()
        {
            josekiService.AddSession();

            josekiService.ToChild(josekiService.ChildStones()[0]);
        }

        public void Dispose()
        {
            josekiService.RemoveSession();
        }
    }
}
