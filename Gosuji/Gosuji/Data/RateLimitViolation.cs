using System.ComponentModel.DataAnnotations;
using Gosuji.Client.Data;

namespace Gosuji.Data
{
    public class RateLimitViolation : DbModel
    {
        [Key] public long Id { get; set; }
        public string Ip { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
    }
}
