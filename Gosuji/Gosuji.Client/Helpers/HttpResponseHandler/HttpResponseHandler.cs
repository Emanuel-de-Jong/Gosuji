using System.Net;
using System.Net.Http.Json;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{

    public class HttpResponseHandler
    {
        public static async Task<APIResponse> Get(HttpClient http, string uri)
        {
            return await TryCatch(http.GetAsync(uri), uri);
        }

        public static async Task<APIResponse<T>> Get<T>(HttpClient http, string uri)
        {
            return await TryCatch<T>(http.GetAsync(uri), uri);
        }

        public static async Task<APIResponse> Post(HttpClient http, string uri, object obj)
        {
            return await TryCatch(http.PostAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse<T>> Post<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PostAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse> Put(HttpClient http, string uri, object obj)
        {
            return await TryCatch(http.PutAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse<T>> Put<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PutAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse> Delete(HttpClient http, string uri)
        {
            return await TryCatch(http.DeleteAsync(uri), uri);
        }

        public static async Task<APIResponse<T>> Delete<T>(HttpClient http, string uri)
        {
            return await TryCatch<T>(http.DeleteAsync(uri), uri);
        }

        private static async Task<APIResponse> TryCatch(Task<HttpResponseMessage> responseTask, string uri)
        {
            APIResponse<object> templateResponse = await TryCatch<object>(responseTask, uri);
            APIResponse response = new()
            {
                StatusCode = templateResponse.StatusCode,
                Message = templateResponse.Message
            };
            return response;
        }

        private static async Task<APIResponse<T>> TryCatch<T>(Task<HttpResponseMessage> responseTask, string uri)
        {
            var response = new APIResponse<T>();

            try
            {
                HttpResponseMessage httpResponse = await responseTask;
                response.StatusCode = httpResponse.StatusCode;

                string content = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    response.Message = content;
                    if (string.IsNullOrEmpty(response.Message))
                    {
                        response.Message = httpResponse.ReasonPhrase;
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                    {
                        Console.WriteLine($"{uri}: {(int)httpResponse.StatusCode} {response.Message}");
                    }
                }
                else if (!string.IsNullOrEmpty(content))
                {
                    response.Data = await Convert<T>(httpResponse, uri);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");

                response.StatusCode = HttpStatusCode.NotImplemented;
                response.Message = e.Message;
            }

            return response;
        }

        private static async Task<T?> Convert<T>(HttpResponseMessage httpResponse, string uri)
        {
            return typeof(T) == typeof(string) ? (T)(object)await httpResponse.Content.ReadAsStringAsync()
                : typeof(T) == typeof(long) ? (T)(object)long.Parse(await httpResponse.Content.ReadAsStringAsync())
                : typeof(T) == typeof(int) ? (T)(object)int.Parse(await httpResponse.Content.ReadAsStringAsync())
                : typeof(T) == typeof(bool) ? (T)(object)bool.Parse(await httpResponse.Content.ReadAsStringAsync())
                : await httpResponse.Content.ReadFromJsonAsync<T>();
        }
    }
}
