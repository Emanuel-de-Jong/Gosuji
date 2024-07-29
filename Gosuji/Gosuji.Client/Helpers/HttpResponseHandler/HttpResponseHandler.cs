using System.Net;
using System.Net.Http.Json;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{

    public class HttpResponseHandler
    {
        public static async Task<APIResponse<bool?>> Get(HttpClient http, string uri)
        {
            return await Get<bool?>(http, uri);
        }

        public static async Task<APIResponse<T>> Get<T>(HttpClient http, string uri)
        {
            return await TryCatch<T>(http.GetAsync(uri), uri);
        }

        public static async Task<APIResponse<bool?>> Post(HttpClient http, string uri, object obj)
        {
            return await Post<bool?>(http, uri, obj);
        }

        public static async Task<APIResponse<T>> Post<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PostAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse<bool?>> Put(HttpClient http, string uri, object obj)
        {
            return await Put<bool?>(http, uri, obj);
        }

        public static async Task<APIResponse<T>> Put<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PutAsJsonAsync(uri, obj), uri);
        }

        public static async Task<APIResponse<bool?>> Delete(HttpClient http, string uri)
        {
            return await Delete<bool?>(http, uri);
        }

        public static async Task<APIResponse<T>> Delete<T>(HttpClient http, string uri)
        {
            return await TryCatch<T>(http.DeleteAsync(uri), uri);
        }

        private static async Task<APIResponse<T>> TryCatch<T>(Task<HttpResponseMessage> responseTask, string uri)
        {
            var response = new APIResponse<T>();

            try
            {
                HttpResponseMessage httpResponse = await responseTask;
                response.StatusCode = httpResponse.StatusCode;
                response.Message = httpResponse.ReasonPhrase;

                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{uri}: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                }
                else if (!string.IsNullOrEmpty(await httpResponse.Content.ReadAsStringAsync()))
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
