using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Shared
{
    public partial class CRedirect : ComponentBase
    {
        [Parameter]
        public string? URI { get; set; }
        [Parameter]
        public bool ForceLoad { get; set; } = true;
        [Parameter]
        public bool Return { get; set; } = false;

        [Inject]
        private NavigationManager navigationManager { get; set; }

        protected override void OnInitialized()
        {
            string uri = "/" + URI;
            if (Return)
            {
                uri += $"?returnUrl={Uri.EscapeDataString(navigationManager.Uri)}";
            }

            navigationManager.NavigateTo(uri, ForceLoad);
        }
    }
}
