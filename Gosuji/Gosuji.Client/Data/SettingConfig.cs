using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class SettingConfig : DbModel
    {
        [Key] public long Id { get; set; }
        public long LanguageId { get; set; }
        public Language? Language { get; set; }
        public bool IsDarkMode { get; set; }
        public int Volume { get; set; } = 100;
        public bool IsGetChangelogEmail { get; set; } = true;
        public float? KomiJP19 { get; set; }
        public float? KomiJP13 { get; set; }
        public float? KomiJP9 { get; set; }
        public float? KomiCN19 { get; set; }
        public float? KomiCN13 { get; set; }
        public float? KomiCN9 { get; set; }
    }
}
