using Gosuji.Client.Helpers.HttpResponseHandler;
using System.Globalization;

namespace Gosuji.Client.Services.User
{
    public class UserService
    {
        private static string MAP_GROUP = "/api/User";

        public HttpClient? HTTP { get; set; }
        public JwtAuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        //public UserService()
        //{
        //    G.Log();
        //}

        public async Task<bool> CheckAuthorized()
        {
            return (await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/CheckAuthorized")).IsSuccess;
        }

        public async Task<APIResponse<string>> Register(VMRegister model)
        {
            model.Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            return await HttpResponseHandler.Post<string>(HTTP,
                $"{MAP_GROUP}/Register", model);
        }

        public async Task<APIResponse> Login(VMLogin model)
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

        public async Task<APIResponse> Logout()
        {
            APIResponse response = await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/Logout", new object());

            await AuthenticationStateProvider.NotifyLogout();

            return response;
        }

        public async Task<APIResponse> UpdatePrivacy(VMUpdatePrivacy model)
        {
            return await HttpResponseHandler.Post(HTTP,
                $"{MAP_GROUP}/UpdatePrivacy", model);
        }

        public async Task<string?> GetToken()
        {
            return (await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetToken")).Data;
        }

        public async Task<bool> GetNewTokens()
        {
            APIResponse<string> response = await HttpResponseHandler.Get<string>(HTTP,
                $"{MAP_GROUP}/GetNewTokens");

            if (response.IsSuccess)
            {
                await AuthenticationStateProvider.NotifyLogin(response.Data);
            }
            else
            {
                await AuthenticationStateProvider.NotifyLogout();
            }

            return response.IsSuccess;
        }
    }
}
