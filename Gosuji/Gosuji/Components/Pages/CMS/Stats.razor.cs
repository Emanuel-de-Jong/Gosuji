using Gosuji.Client.Components.Pages;
using Gosuji.Client;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Pages.CMS
{
    public partial class Stats : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["ChartJS"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("cmsPage.init");
                await jsRef.InvokeVoidAsync("stats.init");
            }
        }
    }
}
