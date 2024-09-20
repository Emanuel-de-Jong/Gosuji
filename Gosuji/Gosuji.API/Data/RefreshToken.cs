using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    public class RefreshToken : DbModel
    {
        [Key]
        public string Token { get; set; }

        public string UserId { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
    }
}
