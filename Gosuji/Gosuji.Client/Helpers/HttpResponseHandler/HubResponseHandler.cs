using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class HubResponseHandler
    {
        public static async Task<APIResponse> TryCatch(string uri, Task<HubResponse> responseTask)
        {
            Task<HubResponse<object>> newResponse = responseTask.ContinueWith(t =>
                new HubResponse<object>(t.GetAwaiter().GetResult().StatusCode));
            return await TryCatch<object>(uri, newResponse);
        }

        public static async Task<APIResponse<T>> TryCatch<T>(string uri, Task<HubResponse<T>> responseTask)
        {
            APIResponse<T> response = new();

            try
            {
                HubResponse<T> hubResponse = await responseTask;
                response.StatusCode = hubResponse.StatusCode;

                if (hubResponse.StatusCode != HttpStatusCode.OK)
                {
                    response.Message = hubResponse.Data != null ? hubResponse.Data.ToString() : "";

                    if (hubResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        Console.WriteLine($"{uri}: {(int)hubResponse.StatusCode} {response.Message}");
                    }
                }
                else
                {
                    response.Data = hubResponse.Data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");

                response.StatusCode = HttpStatusCode.NotImplemented;
                response.Message = e.Message;

                if (e is HubException hubException)
                {
                    if (hubException.Message == "Hub_RateLimit")
                    {
                        response.StatusCode = HttpStatusCode.TooManyRequests;
                    } else
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                    }
                }
            }

            return response;
        }
    }
}
