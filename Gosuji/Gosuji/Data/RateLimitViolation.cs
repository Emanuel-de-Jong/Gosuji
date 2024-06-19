using System.ComponentModel.DataAnnotations;
using Gosuji.Client.Data;

namespace Gosuji.Data
{
    public enum HTTPMethod
    {
        GET,
        PUT,
        POST,
        DELETE,
        HEAD,
        OPTIONS,
        TRACE,
        PATCH,
        CONNECT,
    }

    public class RateLimitViolation : DbModel
    {
        [Key] public long Id { get; set; }
        [MaxLength(120)]
        public string Ip { get; set; }
        [MaxLength(250)]
        public string Endpoint { get; set; }
        public HTTPMethod Method { get; set; }
    }
}
