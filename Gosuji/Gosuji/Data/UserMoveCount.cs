using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class UserMoveCount : DbModel
    {
        [Key] public long Id { get; set; }
        [StringLength(36)]
        public string UserId { get; set; }
        public User? User { get; set; }
        public int Moves { get; set; }
        public int KataGoVisits { get; set; }

        public UserMoveCount(string userId)
        {
            UserId = userId;
        }
    }
}
