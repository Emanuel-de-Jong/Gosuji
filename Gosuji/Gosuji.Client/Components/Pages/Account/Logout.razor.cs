﻿using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class Logout : CustomPage
    {
        [Inject]
        private UserAPI userAPI { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await userAPI.Logout();
            navigationManager.NavigateTo("/");
        }
    }
}
