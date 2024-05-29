using Gosuji.Client.Data;
using Microsoft.AspNetCore.Identity;

namespace Gosuji.Data
{
    public class UserRole : IdentityUserRole<string>, IDbModel
    {
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ModifyDate { get; set; }
    }
}
