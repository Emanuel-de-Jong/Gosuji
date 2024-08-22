using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class HubResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object? Data { get; set; }

        public HubResponse() { }

        public HubResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HubResponse(HttpStatusCode statusCode, object data)
            : this(statusCode)
        {
            Data = data;
        }
    }
}
