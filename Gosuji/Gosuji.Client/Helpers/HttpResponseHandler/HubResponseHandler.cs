using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class HubResponseHandler
    {
        public static async Task<APIResponse> TryCatch(string uri, Task<HubResponse> responseTask)
        {
            return await TryCatch<object?>(uri, responseTask);
        }

        public static async Task<APIResponse<T>> TryCatch<T>(string uri, Task<HubResponse> responseTask)
        {
            APIResponse<T> response = new();

            try
            {
                HubResponse hubResponse = await responseTask;
                response.StatusCode = hubResponse.StatusCode;

                if (hubResponse.StatusCode != HttpStatusCode.OK)
                {
                    response.Message = hubResponse.Data != null ? hubResponse.Data.ToString() : "";

                    if (hubResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        Console.WriteLine($"{uri}: {(int)hubResponse.StatusCode} {response.Message}");
                    }
                }
                else if (hubResponse.Data != null)
                {
                    response.Data = (T)hubResponse.Data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");

                response.StatusCode = HttpStatusCode.NotImplemented;
                response.Message = e.Message;

                if (e is HubException hubException)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                }
            }

            return response;
        }
    }
}
