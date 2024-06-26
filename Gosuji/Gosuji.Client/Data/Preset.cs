using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gosuji.Client.Data
{
    public class Preset : DbModel
    {
        [Key] public long Id { get; set; }

        [StringLength(36)]
        public string? UserId { get; set; }
        [Required]
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }

        [Required]
        [MaxLength(250)]
        [MinLength(1)]
        public string Name { get; set; }
        public int? Order { get; set; }

        [NotMapped]
        public bool IsGeneral => Order != null;
    }
}
