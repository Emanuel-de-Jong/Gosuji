using Gosuji.Client.Helpers;

namespace Gosuji.Client.Services.User
{
    public class UserService
    {
        private static string MAP_GROUP = "/api/User";

        public HttpClient? HTTP { get; set; }
        public JwtAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        public async Task<bool> Login(VMLogin model)
        {
            string? token = await HttpResponseHandler.Post<string>(HTTP,
                $"{MAP_GROUP}/Login", model);
            if (token == null)
            {
                return false;
            }

            await AuthenticationStateProvider.NotifyLogin(token);
            return true;
        }

        public async Task<string?> Register(VMRegister model)
        {
            return await HttpResponseHandler.Post<string>(HTTP,
                $"{MAP_GROUP}/Register", model);
        }

        public async Task<bool> Logout()
        {
            bool result = await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/Logout", new object());

            await AuthenticationStateProvider.NotifyLogout();

            return result;
        }

        public async Task<bool> CheckAuthorized()
        {
            return await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/CheckAuthorized");
        }

        public async Task<string?> GetToken()
        {
            return await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetToken");
        }

        public async Task<bool> GetNewTokens()
        {
            string? token = await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetNewTokens");
            if (token == null)
            {
                await AuthenticationStateProvider.NotifyLogout();
                return false;
            }

            await AuthenticationStateProvider.NotifyLogin(token);
            return true;
        }

        public async Task<bool> UpdatePrivacy(VMUpdatePrivacy model)
        {
            return await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/UpdatePrivacy", model);
        }
    }
}
