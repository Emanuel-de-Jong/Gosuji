using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class SettingConfig : DbModel
    {
        [Key] public long Id { get; set; }
        [Required]
        public long LanguageId { get; set; }
        public Language? Language { get; set; }
        [CustomPersonalData]
        public bool IsDarkMode { get; set; } = true;
        [Range(0, 100)]
        public int MasterVolume { get; set; } = 100;
        [Range(0, 100)]
        public int StoneVolume { get; set; } = 100;
        public bool IsPreMoveStoneSound { get; set; } = true;
        public bool IsSelfplayStoneSound { get; set; } = true;
        [CustomPersonalData]
        public bool IsGetChangelogEmail { get; set; }

        public double CalcMasterVolume()
        {
            return MasterVolume / 100d;
        }

        public double CalcStoneVolume()
        {
            return (StoneVolume / 100d) * CalcMasterVolume();
        }
    }
}
