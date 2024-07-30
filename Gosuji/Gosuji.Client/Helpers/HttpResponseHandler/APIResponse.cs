using System.Net;

namespace Gosuji.Client.Helpers.HttpResponseHandler
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }

        public bool IsSuccess => StatusCode == HttpStatusCode.OK;
        public bool IsMessage => StatusCode == HttpStatusCode.Accepted;
        public bool IsLimit => StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.RequestTimeout;
        public bool IsNotAuthenticated => StatusCode == HttpStatusCode.Unauthorized;
        public bool IsNotAuthorized => StatusCode == HttpStatusCode.Forbidden;
        public bool IsClientProblem => StatusCode == HttpStatusCode.NotImplemented;
    }

    public class APIResponse<T> : APIResponse
    {
        public T? Data { get; set; }
    }
}
