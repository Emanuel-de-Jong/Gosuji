using Gosuji.Client.Helpers;
using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Gosuji.Client.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private UserService userService;
        private AuthenticationState? state;
        private readonly AuthenticationState anonymousState;
        public string? Token { get; set; }

        public JwtAuthenticationStateProvider(UserService userService)
        {
            this.userService = userService;
            anonymousState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private AuthenticationState CreateAuthenticationState()
        {
            IEnumerable<Claim> claims = JwtHelper.ParseClaimsFromJwt(Token);
            ClaimsPrincipal user = new(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (state != null)
            {
                return state;
            }

            Token = await userService.GetToken();
            if (Token == null)
            {
                state = anonymousState;
                return state;
            }

            state = CreateAuthenticationState();
            await userService.CheckAuthorized();

            return state;
        }

        public async Task NotifyLogin(string token)
        {
            bool shouldNotify = Token == null;

            Token = token;
            AuthenticationState tempState = CreateAuthenticationState();
            if (!shouldNotify)
            {
                shouldNotify = !JwtHelper.ClaimsEquals(state.User.Claims, tempState.User.Claims);
            }

            state = tempState;

            if (shouldNotify)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }

        public async Task NotifyLogout()
        {
            bool shouldNotify = Token != null;

            Token = null;
            state = anonymousState;

            if (shouldNotify)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }
    }
}
