using GosujiServer.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GosujiServer.Areas.Identity.Data
{
    public class UserRole : IdentityUserRole<string>, IDbModel
    {
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ModifyDate { get; set; }
    }
}
