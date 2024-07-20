using Gosuji.Client.Helpers;

namespace Gosuji.Client.Services
{
    public class TestService
    {
        private static string MAP_GROUP = "/api/Test";

        public HttpClient? HTTP { get; set; }

        public TestService(IHttpClientFactory httpClientFactory)
        {
            HTTP = httpClientFactory.CreateClient("Auth");
        }

        public async Task<bool> Test()
        {
            return await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/Test");
        }

        public async Task<bool> Test2()
        {
            return await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/Test2");
        }

        public async Task<bool> Test3()
        {
            return await HttpResponseHandler.Get(HTTP,
                $"{MAP_GROUP}/Test3");
        }
    }
}
