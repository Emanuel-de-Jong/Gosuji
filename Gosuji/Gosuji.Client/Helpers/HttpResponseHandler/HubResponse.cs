using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class HubResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Data { get; set; }

        public HubResponse() { }

        public HubResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HubResponse(HttpStatusCode statusCode, string data)
            : this(statusCode)
        {
            Data = data;
        }
    }
}
