using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Pages
{
    public class CustomPage : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            if (G.StatusMessage != null)
            {
                G.StatusMessage.Show = false;
            }
        }
    }
}
