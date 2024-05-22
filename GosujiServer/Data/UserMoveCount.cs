using GosujiServer.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class UserMoveCount : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int Moves { get; set; }
        public int KataGoVisits { get; set; }
    }
}
