using System.Net.Http.Json;

namespace Gosuji.Client.Helpers
{

    public class HttpResponseHandler
    {
        private class NoValue() { }

        public static async Task<bool> Get(HttpClient http, string uri)
        {
            return (await Get<NoValue>(http, uri)) != null ? true : false;
        }

        public static async Task<T?> Get<T>(HttpClient http, string uri)
        {
            return await TryCatch<T>(http.GetAsync(uri), uri);
        }

        public static async Task<bool> Post(HttpClient http, string uri, object obj)
        {
            return (await Post<NoValue>(http, uri, obj)) != null ? true : false;
        }

        public static async Task<T?> Post<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PostAsJsonAsync(uri, obj), uri);
        }

        public static async Task<bool> Put(HttpClient http, string uri, object obj)
        {
            return (await Put<NoValue>(http, uri, obj)) != null ? true : false;
        }

        public static async Task<T?> Put<T>(HttpClient http, string uri, object obj)
        {
            return await TryCatch<T>(http.PostAsJsonAsync(uri, obj), uri);
        }

        private static async Task<T?> TryCatch<T>(Task<HttpResponseMessage> responseTask, string uri)
        {
            try
            {
                HttpResponseMessage response = await responseTask;
                return await Convert<T>(response, uri);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{uri}: {e.GetType().Name} {e.Message} ({e.StackTrace})");
                return default;
            }
        }

        private static async Task<T?> Convert<T>(HttpResponseMessage response, string uri)
        {
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"{uri}: {(int)response.StatusCode} {response.ReasonPhrase}");
                return default;
            }

            if (typeof(T) == typeof(NoValue))
            {
                return (T)(object)new NoValue();
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await response.Content.ReadAsStringAsync();
            }
            if (typeof(T) == typeof(long))
            {
                return (T)(object)long.Parse(await response.Content.ReadAsStringAsync());
            }
            if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(await response.Content.ReadAsStringAsync());
            }
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)bool.Parse(await response.Content.ReadAsStringAsync());
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
