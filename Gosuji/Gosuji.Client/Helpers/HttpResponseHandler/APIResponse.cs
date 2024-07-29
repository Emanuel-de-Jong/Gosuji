using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class APIResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public bool IsSuccess => IsOk && Message == null;
        public bool IsOk => StatusCode == HttpStatusCode.OK;
        public bool IsLimit => StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.RequestTimeout;
        public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;
        public bool IsClientProblem => StatusCode == HttpStatusCode.NotImplemented;
    }
}
