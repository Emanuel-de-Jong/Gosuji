using System.ComponentModel.DataAnnotations;

namespace Gosuji.Client.Data
{
    public class Game : DbModel
    {
        [Key] public long Id { get; set; }

        [StringLength(36)]
        public string? UserId { get; set; }
        [Required]
        public long TrainerSettingConfigId { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        [Required]
        public long KataGoVersionId { get; set; }
        public KataGoVersion? kataGoVersion { get; set; }
        public long? GameStatId { get; set; }
        public GameStat? GameStat { get; set; }
        public long? OpeningStatId { get; set; }
        public GameStat? OpeningStat { get; set; }
        public long? MidgameStatId { get; set; }
        public GameStat? MidgameStat { get; set; }
        public long? EndgameStatId { get; set; }
        public GameStat? EndgameStat { get; set; }

        [Required]
        [MaxLength(250)]
        [MinLength(1)]
        public string Name { get; set; }
        public int? Result { get; set; }
        [Required]
        [Range(0, 500)]
        public int PrevNodeX { get; set; }
        [Required]
        [Range(0, 500)]
        public int PrevNodeY { get; set; }

        [Required]
        public int RightStreak { get; set; }
        [Required]
        public int PerfectStreak { get; set; }
        [Required]
        public int RightTopStreak { get; set; }
        [Required]
        public int PerfectTopStreak { get; set; }

        [Required]
        [RegularExpression("^(9|13|19)$")]
        public int Boardsize { get; set; }
        [Required]
        [Range(0, 9)]
        public int Handicap { get; set; }
        [Required]
        [Range(-1, 1)]
        public int Color { get; set; }
        [Required]
        [MaxLength(100)]
        public string Ruleset { get; set; }
        [Required]
        [Range(-150, 150)]
        public double Komi { get; set; }

        [MaxLength(100_000)]
        public string SGF { get; set; }
        public byte[] PlayerResults { get; set; }
        public byte[] Suggestions { get; set; }
        public byte[] MoveTypes { get; set; }
        public byte[] ChosenNotPlayedCoords { get; set; }

        public bool IsFinished { get; set; }
        public bool IsThirdPartySGF { get; set; }
        public bool IsDeleted { get; set; }
    }
}
