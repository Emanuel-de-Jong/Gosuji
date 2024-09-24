using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    public class UserActivity : DbModel
    {
        [Key]
        public long Id { get; set; }

        [StringLength(36)]
        public string? UserId { get; set; }
        public User? User { get; set; }
        [Required]
        [MaxLength(120)]
        [CustomPersonalData]
        public string Ip { get; set; }
        [CustomPersonalData]
        public DateTimeOffset? EndDate { get; set; }
    }
}
