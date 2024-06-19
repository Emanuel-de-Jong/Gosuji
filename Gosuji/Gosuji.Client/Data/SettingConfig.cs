using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class SettingConfig : DbModel
    {
        [Key] public long Id { get; set; }
        public long LanguageId { get; set; }
        public Language? Language { get; set; }
        public bool IsDarkMode { get; set; }
        [Range(0, 100)]
        public int Volume { get; set; } = 100;
        public bool IsGetChangelogEmail { get; set; } = true;
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
