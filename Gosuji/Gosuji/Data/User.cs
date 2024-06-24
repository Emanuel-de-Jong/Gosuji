using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
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
        [CustomPersonalData]
        public bool IsBanned { get; set; }
        [CustomPersonalData]
        public DateTimeOffset? EmailConfirmedDate { get; set; }
        [Required]
        [CustomPersonalData]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        [CustomPersonalData]
        public DateTimeOffset ModifyDate { get; set; }

        public User()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            CreateDate = now;
            ModifyDate = now;
        }
    }
}
