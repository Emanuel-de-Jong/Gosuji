using Gosuji.Client.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class User : IdentityUser, IDbModel
    {
        [Required]
        public long SettingConfigId { get; set; }
        public SettingConfig? SettingConfig { get; set; }
        public long? CurrentSubscriptionId { get; set; }
        public UserSubscription? CurrentSubscription { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? EmailConfirmedDate { get; set; }
        [Required]
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ModifyDate { get; set; }
    }
}
