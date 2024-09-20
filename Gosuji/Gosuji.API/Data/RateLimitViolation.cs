using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
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
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Ip { get; set; }
        [Required]
        [MaxLength(250)]
        public string Endpoint { get; set; }
        [Required]
        public HTTPMethod Method { get; set; }
    }
}
