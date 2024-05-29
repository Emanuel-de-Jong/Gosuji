using GosujiServer.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GosujiServer.Data
{
    public class Preset : DbModel
    {
        [Key] public long Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
