using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Xml.Linq;

namespace GosujiServer.Pages
{
    public partial class Josekis
    {
        protected override async Task OnInitializedAsync()
        {
            josekiService.AddSession();
        }
    }
}
