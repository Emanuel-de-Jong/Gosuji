using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Pages
{
    public partial class Logout : ComponentBase
    {
        [Inject]
        private UserService userService { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await userService.Logout();
            navigationManager.NavigateTo("/");
        }
    }
}
