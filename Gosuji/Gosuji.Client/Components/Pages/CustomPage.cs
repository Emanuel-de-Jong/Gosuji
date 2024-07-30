using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Pages
{
    public class CustomPage : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("CustomPage.OnInitializedAsync");
        }
    }
}
