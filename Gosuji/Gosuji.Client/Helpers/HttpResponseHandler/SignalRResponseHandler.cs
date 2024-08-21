using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class SignalRResponseHandler
    {
        public static async Task<APIResponse> TryCatch(string uri, Task task)
        {
            Task<object?> newTask = task.ContinueWith<object?>(t => {
                t.GetAwaiter().GetResult();
                return null;
            });
            return await TryCatch(uri, newTask);
        }

        public static async Task<APIResponse<T>> TryCatch<T>(string uri, Task<T> task)
        {
            APIResponse<T> response = new();

            try
            {
                T result = await task;
                response.StatusCode = HttpStatusCode.OK;
                response.Data = result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");

                response.StatusCode = e is HubException ? HttpStatusCode.BadRequest : HttpStatusCode.NotImplemented;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
