using Gosuji.Client.Components.Pages;
using Gosuji.Client;
using Gosuji.Controllers;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace Gosuji.Components.Pages.CMS
{
    public partial class Stats : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", "css/cms.css");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }
        }
    }
}
