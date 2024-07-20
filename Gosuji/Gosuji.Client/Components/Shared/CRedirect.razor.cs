using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Shared
{
    public partial class CRedirect : ComponentBase
    {
        [Parameter]
        public string? Uri { get; set; }
        [Parameter]
        public bool ShouldReturn { get; set; } = false;

        [Inject]
        private NavigationManager navigationManager { get; set; }

        protected override void OnInitialized()
        {
            string uri = Uri;
            if (ShouldReturn)
            {
                uri += $"?{G.ReturnUriName}={System.Uri.EscapeDataString(navigationManager.Uri)}";
            }

            navigationManager.NavigateTo(uri);
        }
    }
}
