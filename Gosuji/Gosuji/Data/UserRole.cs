using Gosuji.Client.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class UserRole : IdentityUserRole<string>, IDbModel
    {
        [Required]
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ModifyDate { get; set; }
    }
}
