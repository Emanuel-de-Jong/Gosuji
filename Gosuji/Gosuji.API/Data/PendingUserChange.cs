using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    // A PendingUserChange always has 1 User. A User always has 0 or 1 PendingUserChange.
    public class PendingUserChange : DbModel
    {
        [Key]
        public string Id { get; set; } // Same as User.Id
        public User? User { get; set; }

        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
