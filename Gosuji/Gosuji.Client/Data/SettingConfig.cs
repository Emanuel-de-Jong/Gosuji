using Gosuji.Client.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    // A SettingConfig always has 1 User. A User always has 1 SettingConfig.
    public class SettingConfig : DbModel
    {
        [Key]
        public string Id { get; set; } // Same as User.Id

        public string? LanguageId { get; set; } = ELanguage.en.ToString();
        public Language? Language { get; set; }
        [CustomPersonalData]
        public EThemeType Theme { get; set; } = EThemeType.DARK;
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
            return StoneVolume / 100d * CalcMasterVolume();
        }
    }

    public enum EThemeType
    {
        DARK = 0,
        LIGHT = 1
    }
}
