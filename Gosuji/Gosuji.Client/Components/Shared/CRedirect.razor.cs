using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Shared
{
    public partial class CRedirect : ComponentBase
    {
        [Parameter]
        public string? URI { get; set; }
        [Parameter]
        public bool ForceLoad { get; set; }

        [Inject]
        private NavigationManager? navigationManager { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            navigationManager.NavigateTo("/" + URI, ForceLoad);
        }
    }
}
