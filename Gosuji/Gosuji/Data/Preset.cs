using Gosuji.Client.Data;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Data
{
    public class Preset : DbModel
    {
        [Key] public long Id { get; set; }
        [MaxLength(100)]
        public string UserId { get; set; }
        public User User { get; set; }
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
