using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    public class UserMoveCount : DbModel
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(36)]
        public string UserId { get; set; }
        public User? User { get; set; }
        [CustomPersonalData]
        public int Moves { get; set; }
        [Required]
        [CustomPersonalData]
        public int KataGoVisits { get; set; }
    }
}
