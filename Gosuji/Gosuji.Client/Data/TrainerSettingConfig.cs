using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Gosuji.Client.Data
{
    public class TrainerSettingConfig : DbModel
    {
        [Key] public long Id { get; set; }

        [MaxLength(150)]
        public string? Hash { get; set; }

        [Required]
        [RegularExpression("^(9|13|19)$")]
        public int Boardsize { get; set; }
        [Required]
        [Range(0, 9)]
        public int Handicap { get; set; }
        [Required]
        public bool PreMovesSwitch { get; set; }
        [Required]
        public int PreMoves { get; set; }
        [Required]
        public EHideOptions HideOptions { get; set; }
        [Required]
        public int ColorType { get; set; }
        [Required]
        public bool WrongMoveCorrection { get; set; }

        [Required]
        [MaxLength(100)]
        public string KomiChangeStyle { get; set; }
        [Required]
        [Range(-150, 150)]
        public double Komi { get; set; }
        [Required]
        [MaxLength(100)]
        public string Ruleset { get; set; }

        [Required]
        [MaxLength(100)]
        public string ForceOpponentCorners { get; set; }
        [Required]
        public bool CornerSwitch44 { get; set; }
        [Required]
        public bool CornerSwitch34 { get; set; }
        [Required]
        public bool CornerSwitch33 { get; set; }
        [Required]
        public bool CornerSwitch45 { get; set; }
        [Required]
        public bool CornerSwitch35 { get; set; }
        [Required]
        public int CornerChance44 { get; set; }
        [Required]
        public int CornerChance34 { get; set; }
        [Required]
        public int CornerChance33 { get; set; }
        [Required]
        public int CornerChance45 { get; set; }
        [Required]
        public int CornerChance35 { get; set; }
        [Required]
        public int PreOptions { get; set; }
        [Required]
        public bool PreOptionPercSwitch { get; set; }
        [Required]
        public double PreOptionPerc { get; set; }

        [Required]
        public int SuggestionOptions { get; set; }
        [Required]
        public bool HideWeakerOptions { get; set; }
        [Required]
        public bool MinVisitsPercSwitch { get; set; }
        [Required]
        public double MinVisitsPerc { get; set; }
        [Required]
        public bool MaxVisitDiffPercSwitch { get; set; }
        [Required]
        public double MaxVisitDiffPerc { get; set; }

        [Required]
        public int OpponentOptions { get; set; }
        [Required]
        public EHideOpponentOptions HideOpponentOptions { get; set; }
        [Required]
        public bool OpponentOptionPercSwitch { get; set; }
        [Required]
        public double OpponentOptionPerc { get; set; }

        [Required]
        [Range(1.5, 4)]
        public double SelfplayPlaySpeed { get; set; }

        public TrainerSettingConfig SetHash()
        {
            Hash = GenerateHash(this);
            return this;
        }

        public static string GenerateHash(TrainerSettingConfig config)
        {
            StringBuilder builder = new();

            builder.Append(config.Boardsize);
            builder.Append(config.Handicap);
            builder.Append(config.ColorType);
            builder.Append(config.PreMovesSwitch);
            builder.Append(config.PreMoves);
            builder.Append(config.WrongMoveCorrection);

            builder.Append(config.Ruleset);
            builder.Append(config.KomiChangeStyle);
            builder.Append(config.Komi);

            builder.Append(config.PreOptions);
            builder.Append(config.PreOptionPercSwitch);
            builder.Append(config.PreOptionPerc);
            builder.Append(config.ForceOpponentCorners);
            builder.Append(config.CornerSwitch44);
            builder.Append(config.CornerSwitch34);
            builder.Append(config.CornerSwitch33);
            builder.Append(config.CornerSwitch45);
            builder.Append(config.CornerSwitch35);
            builder.Append(config.CornerChance44);
            builder.Append(config.CornerChance34);
            builder.Append(config.CornerChance33);
            builder.Append(config.CornerChance45);
            builder.Append(config.CornerChance35);

            builder.Append(config.SuggestionOptions);
            builder.Append(config.HideOptions);
            builder.Append(config.HideWeakerOptions);
            builder.Append(config.MinVisitsPercSwitch);
            builder.Append(config.MinVisitsPerc);
            builder.Append(config.MaxVisitDiffPercSwitch);
            builder.Append(config.MaxVisitDiffPerc);

            builder.Append(config.OpponentOptionPercSwitch);
            builder.Append(config.OpponentOptions);
            builder.Append(config.OpponentOptionPerc);
            builder.Append(config.HideOpponentOptions);

            builder.Append(config.SelfplayPlaySpeed);

            byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(builder.ToString());
            byte[] hashBytes = MD5.HashData(inputBytes);
            return string.Join("", hashBytes.Select(x => x.ToString("X2")));
        }
    }

    public enum EHideOptions
    {
        NEVER = 0,
        PERFECT = 1,
        RIGHT = 2,
        ALWAYS = 3
    }

    public enum EHideOpponentOptions
    {
        NEVER = 0,
        PERFECT = 1,
        ALWAYS = 3
    }
}
