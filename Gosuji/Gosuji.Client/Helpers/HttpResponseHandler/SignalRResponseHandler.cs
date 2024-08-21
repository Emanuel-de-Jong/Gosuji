using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class SignalRResponseHandler
    {
        public static async Task<APIResponse> Invoke(HubConnection hubConnection, string uri, params object[] args)
        {
            return await TryCatch(hubConnection.InvokeAsync(uri, args), uri);
        }

        public static async Task<APIResponse<T>> Invoke<T>(HubConnection hubConnection, string uri, params object[] args)
        {
            return await TryCatch(hubConnection.InvokeAsync<T>(uri, args), uri);
        }

        private static async Task<APIResponse> TryCatch(Task task, string uri)
        {
            Task<object?> newTask = task.ContinueWith<object?>(t => {
                t.GetAwaiter().GetResult();
                return null;
            });
            return await TryCatch(newTask, uri);
        }

        private static async Task<APIResponse<T>> TryCatch<T>(Task<T> task, string uri)
        {
            APIResponse<T> response = new();

            try
            {
                T result = await task;
                response.StatusCode = HttpStatusCode.OK;
                response.Data = result;
            }
            catch (HubException hubEx)
            {
                Console.WriteLine($"{uri}: HubException {hubEx.Message} ({hubEx.StackTrace})");

                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = hubEx.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");

                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
