using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace GosujiServer.Shared.CMS
{
    public partial class AbuseDetect : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private DbService dbService { get; set; }

        private List<User> users;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            users = await dbContext.Users.ToListAsync();
            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("abuseDetect.init");
            }
        }
    }
}
