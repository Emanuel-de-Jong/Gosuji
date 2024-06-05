using Gosuji.Client;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Pages.CMS
{
    public partial class CMS : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }

        private IJSObjectReference jsRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("cmsPage.init");
            }
        }
    }
}
