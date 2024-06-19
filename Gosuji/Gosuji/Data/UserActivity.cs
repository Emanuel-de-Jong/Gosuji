using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class UserActivity : DbModel
    {
        [Key] public long Id { get; set; }
        [Required]
        [StringLength(36)]
        public string UserId { get; set; }
        public User? User { get; set; }
        [Required]
        [MaxLength(120)]
        public string Ip { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
