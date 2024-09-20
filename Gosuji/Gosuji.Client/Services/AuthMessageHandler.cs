using Gosuji.Client.Services.User;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;

namespace Gosuji.Client.Services
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private IJSRuntime js;
        private UserAPI userAPI;
        private JwtAuthenticationStateProvider authenticationStateProvider;

        public AuthMessageHandler(IJSRuntime js, UserAPI userAPI, AuthenticationStateProvider authenticationStateProvider)
        {
            this.js = js;
            this.userAPI = userAPI;
            this.authenticationStateProvider = authenticationStateProvider as JwtAuthenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

            if (authenticationStateProvider.Token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", authenticationStateProvider.Token);
            }
            else
            {
                request.Headers.Authorization = null;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized && authenticationStateProvider.Token != null)
            {
                if (await userAPI.GetNewTokens())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("bearer", authenticationStateProvider.Token);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }
}
