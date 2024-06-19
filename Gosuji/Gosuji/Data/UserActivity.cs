using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class UserActivity : DbModel
    {
        [Key] public long Id { get; set; }
        [MaxLength(100)]
        public string UserId { get; set; }
        public User User { get; set; }
        [MaxLength(120)]
        public string Ip { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
