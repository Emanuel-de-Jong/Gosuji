using GosujiServer.Data;
using GosujiServer.Interfaces;
using GosujiServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Transactions;

namespace GosujiServer.Areas.Identity.Data
{
    public class User : IdentityUser, IDbModel
    {
        public long? SettingConfigId { get; set; }
        public SettingConfig? SettingConfig { get; set; }
        public long? CurrentSubscriptionId { get; set; }
        public UserSubscription? CurrentSubscription { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? EmailConfirmedDate { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? ModifyDate { get; set; }

        public User() : base()
        {
            ApplicationDbContext context = new DbService().GetContext();

            if (!context.Presets.Where(p => p.UserId == Id).Any())
            {
                TrainerSettingConfig trainerSettingConfig = new TrainerSettingConfig();
            }
        }
    }
}
