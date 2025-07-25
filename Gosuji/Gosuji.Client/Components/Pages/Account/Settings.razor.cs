﻿using Gosuji.Client.Attributes;
using Gosuji.Client.Components.Shared;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Resources.Translations;
using Gosuji.Client.Services;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gosuji.Client.Components.Pages.Account
{
    public partial class Settings : CustomPage
    {
        [SupplyParameterFromForm]
        private PrivacyInputModel? privacyInput { get; set; }
        [SupplyParameterFromForm]
        private DeletePersonalDataInputModel? deletePersonalDataInput { get; set; } = new();

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        private NavigationManager navigationManager { get; set; }
        [Inject]
        private SettingConfigService settingConfigService { get; set; }
        [Inject]
        private UserAPI userAPI { get; set; }
        [Inject]
        private DataAPI dataAPI { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IStringLocalizer<APIResponses> tlAPI { get; set; }

        private CStatusMessage privacyStatusMessage;
        private CStatusMessage deletePersonalDataStatusMessage;
        private CStatusMessage issuesStatusMessage;

        private string? currentUserName;
        private string? currentEmail;
        private Subscription? subscription;

        protected override async Task OnInitializedAsync()
        {
            ClaimsPrincipal claimsPrincipal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                navigationManager.NavigateTo($"login?{G.ReturnUriName}={Uri.EscapeDataString(navigationManager.Uri)}");
                return;
            }

            currentUserName = claimsPrincipal.Identity.Name;
            currentEmail = claimsPrincipal.FindFirst(ClaimTypes.Email).Value;

            privacyInput = new PrivacyInputModel
            {
                UserName = currentUserName,
                Email = currentEmail
            };

            APIResponse<Subscription?> response = await dataAPI.GetSubscription(true);
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            subscription = response.Data;
        }

        public async Task UpdatePrivacy()
        {
            VMUpdatePrivacy vmUpdatePrivacy = new()
            {
                CurrentPassword = privacyInput.CurrentPassword
            };

            if (privacyInput.UserName != currentUserName)
            {
                vmUpdatePrivacy.UserName = privacyInput.UserName;
            }
            if (privacyInput.Email != currentEmail)
            {
                vmUpdatePrivacy.Email = privacyInput.Email;
            }
            if (!string.IsNullOrEmpty(privacyInput.NewPassword))
            {
                vmUpdatePrivacy.NewPassword = privacyInput.NewPassword;
            }

            if (vmUpdatePrivacy.UserName == null && vmUpdatePrivacy.Email == null && vmUpdatePrivacy.NewPassword == null)
            {
                privacyStatusMessage.SetMessage(tlAPI[APIResponses.User_UpdatePrivacy_NoChanges]);
                return;
            }

            APIResponse response = await userAPI.UpdatePrivacy(vmUpdatePrivacy);

            if (response.IsSuccess)
            {
                privacyStatusMessage.SetMessage("Privacy changes pending. A confirmation email has been sent to your current email address. Please use the link in the email to confirm your changes.");
                privacyInput.NewPassword = string.Empty;
                privacyInput.ConfirmNewPassword = string.Empty;
                privacyInput.CurrentPassword = string.Empty;
            }
            else if (response.Message == APIResponses.User_UpdatePrivacy_NoChanges)
            {
                privacyStatusMessage.HandleAPIResponse(response, true);
            }
            else
            {
                privacyStatusMessage.HandleAPIResponse(response);
            }
        }

        private sealed class PrivacyInputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MinLength(2, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(30, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?!\s*$).+$", ErrorMessageResourceName = "JustSpacesError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string UserName { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^[^@\s]+@[^@\s.]+(\.[^@\s.]+)*\.[a-zA-Z]{2,}$",
                ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Email { get; set; }

            [MinLength(6, ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [NotEqual(nameof(CurrentPassword), ErrorMessageResourceName = "NewPasswordEqualError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [RegularExpression(@"^(?=.*\d)(?=.*[@#$%^&+=!]).*$",
                ErrorMessageResourceName = "PasswordCharError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string NewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [Compare(nameof(NewPassword), ErrorMessageResourceName = "CompareError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string ConfirmNewPassword { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string CurrentPassword { get; set; }
        }

        private async Task DownloadPersonalData()
        {
            APIResponse<byte[]> response = await userAPI.DownloadPersonalData();
            if (G.StatusMessage.HandleAPIResponse(response)) return;
            byte[] fileBytes = response.Data;

            await js.InvokeVoidAsync("utils.downloadByteFile",
                "PersonalData",
                "json",
                fileBytes,
                "application/json");
        }

        private async Task DeletePersonalData()
        {
            VMDeletePersonalData vmDeletePersonalData = new()
            {
                Password = deletePersonalDataInput.Password
            };

            APIResponse response = await userAPI.DeletePersonalData(vmDeletePersonalData);
            if (response.IsSuccess)
            {
                deletePersonalDataStatusMessage.SetMessage("A confirmation email has been sent. Please use the link in the email to confirm the deletion.");
                deletePersonalDataInput = new();
            }
            else
            {
                deletePersonalDataStatusMessage.HandleAPIResponse(response);
            }
        }

        private async Task TryForgotPassword()
        {
            if (currentEmail == null)
            {
                navigationManager.NavigateTo("forgot-password");
            }

            VMForgotPassword vmForgotPassword = new()
            {
                Email = currentEmail
            };

            APIResponse response = await userAPI.ForgotPassword(vmForgotPassword);
            if (response.IsSuccess)
            {
                issuesStatusMessage.SetMessage("An email has been sent. Please use the link in the email to set a new password.");
            }
            else
            {
                issuesStatusMessage.HandleAPIResponse(response);
            }
        }

        private sealed class DeletePersonalDataInputModel
        {
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(ValidateMessages))]
            [MaxLength(50, ErrorMessageResourceName = "MaxLengthError", ErrorMessageResourceType = typeof(ValidateMessages))]
            public string Password { get; set; }
        }
    }
}
