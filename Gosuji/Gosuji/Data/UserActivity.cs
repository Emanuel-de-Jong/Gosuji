using GosujiServer.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class UserActivity : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Ip { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
