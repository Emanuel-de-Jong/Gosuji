using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class HubResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }

        public HubResponse() { }

        public HubResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HubResponse(HttpStatusCode statusCode, T? data)
            : this(statusCode)
        {
            Data = data;
        }
    }

    public class HubResponse : HubResponse<object?>
    {
        public HubResponse() : base() { }
        public HubResponse(HttpStatusCode statusCode) : base(statusCode) { }
        public HubResponse(HttpStatusCode statusCode, object? data) : base(statusCode, data) { }
    }
}
