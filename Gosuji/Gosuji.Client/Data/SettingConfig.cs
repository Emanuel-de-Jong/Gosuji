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
        [CustomPersonalData]
        public int Volume { get; set; } = 100;
        [CustomPersonalData]
        public bool IsGetChangelogEmail { get; set; }
        [Range(-150, 150)]
        public float? KomiJP19 { get; set; }
        [Range(-150, 150)]
        public float? KomiJP13 { get; set; }
        [Range(-150, 150)]
        public float? KomiJP9 { get; set; }
        [Range(-150, 150)]
        public float? KomiCN19 { get; set; }
        [Range(-150, 150)]
        public float? KomiCN13 { get; set; }
        [Range(-150, 150)]
        public float? KomiCN9 { get; set; }
    }
}
