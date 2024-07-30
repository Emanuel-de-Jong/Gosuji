using Gosuji.Client.Helpers.HttpResponseHandler;

namespace Gosuji.Client.Services.User
{
    public class UserService
    {
        private static string MAP_GROUP = "/api/User";

        public HttpClient? HTTP { get; set; }
        public JwtAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        public async Task<APIResponse<string>> Login(VMLogin model)
        {
            APIResponse<string> response = await HttpResponseHandler.Post<string>(HTTP,
                $"{MAP_GROUP}/Login", model);

            if (response.IsSuccess)
            {
                await AuthenticationStateProvider.NotifyLogin(response.Data);
                response.Data = null;
            }

            return response;
        }

        public async Task<APIResponse<string>> Register(VMRegister model)
        {
            return await HttpResponseHandler.Post<string>(HTTP,
                $"{MAP_GROUP}/Register", model);
        }

        public async Task<APIResponse> Logout()
        {
            APIResponse response = await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/Logout", new object());

            await AuthenticationStateProvider.NotifyLogout();

            return response;
        }

        public async Task<APIResponse> CheckAuthorized()
        {
            return await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/CheckAuthorized");
        }

        public async Task<APIResponse<string>> GetToken()
        {
            return await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetToken");
        }

        public async Task<APIResponse<string>> GetNewTokens()
        {
            APIResponse<string> response = await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetNewTokens");

            if (response.IsSuccess)
            {
                await AuthenticationStateProvider.NotifyLogin(response.Data);
                response.Data = null;
            }
            else
            {
                await AuthenticationStateProvider.NotifyLogout();
            }

            return response;
        }

        public async Task<APIResponse> UpdatePrivacy(VMUpdatePrivacy model)
        {
            return await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/UpdatePrivacy", model);
        }
    }
}
