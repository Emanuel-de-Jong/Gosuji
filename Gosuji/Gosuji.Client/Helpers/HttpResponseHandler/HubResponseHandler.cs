using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

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
                    response.Message = hubResponse.Data != null ? await Convert<string>(hubResponse.Data) : "";

                    if (hubResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        Console.WriteLine($"{uri}: {(int)hubResponse.StatusCode} {response.Message}");
                    }
                }
                else if (hubResponse.Data != null)
                {
                    response.Data = await Convert<T>(hubResponse.Data);
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

        private static async Task<T?> Convert<T>(object data)
        {
            if (data is JsonElement jsonElement)
            {
                data = typeof(T) == typeof(string) ? jsonElement.GetString()
                    : typeof(T) == typeof(long) ? jsonElement.GetInt64()
                    : typeof(T) == typeof(int) ? jsonElement.GetInt32()
                    : typeof(T) == typeof(bool) ? jsonElement.GetBoolean()
                    : JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }

            return (T)data;
        }
    }
}
