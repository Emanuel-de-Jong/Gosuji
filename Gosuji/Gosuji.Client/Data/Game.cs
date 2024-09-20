using Gosuji.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Game : DbModel
    {
        public const string DEFAULT_NAME = "Game";

        [StringLength(12)]
        [Key]
        public string Id { get; set; }

        [StringLength(36)]
        public string? UserId { get; set; }
        [Required]
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public long KataGoVersionId { get; set; }
        public KataGoVersion? KataGoVersion { get; set; }
        public long GameStatId { get; set; }
        public GameStat? GameStat { get; set; }
        public long? OpeningStatId { get; set; }
        public GameStat? OpeningStat { get; set; }
        public long? MidgameStatId { get; set; }
        public GameStat? MidgameStat { get; set; }
        public long? EndgameStatId { get; set; }
        public GameStat? EndgameStat { get; set; }

        public string Name { get; set; }
        public EMoveColor Color { get; set; } = EMoveColor.RANDOM;
        public double? Result { get; set; }
        public string ProductVersion { get; set; }
        public bool IsDeleted { get; set; }
        public bool ShouldIgnoreStats { get; set; }
        public bool IsThirdPartySGF { get; set; }

        public string Ruleset { get; set; }
        public double Komi { get; set; }

        public int LastNodeX { get; set; }
        public int LastNodeY { get; set; }
        public int RightStreak { get; set; }
        public int PerfectStreak { get; set; }
        public int RightTopStreak { get; set; }
        public int PerfectTopStreak { get; set; }

        public byte[] EncodedData { get; set; }

        public void GenerateId()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "")[..12];
        }
    }
}
