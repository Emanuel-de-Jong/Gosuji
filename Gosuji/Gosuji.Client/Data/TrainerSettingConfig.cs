using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Gosuji.Client.Data
{
    public class TrainerSettingConfig : DbModel
    {
        [Key] public long Id { get; set; }

        [MaxLength(150)]
        public string Hash { get; set; }

        public int Boardsize { get; set; } = 19;
        public int Handicap { get; set; } = 0;
        public int ColorType { get; set; } = 0;
        public bool PreMovesSwitch { get; set; } = false;
        public int PreMoves { get; set; } = 0;
        public int PreVisits { get; set; } = 1000;
        public int SelfplayVisits { get; set; } = 2500;
        public int SuggestionVisits { get; set; } = 1500;
        public int OpponentVisits { get; set; } = 1500;
        public bool DisableAICorrection { get; set; } = false;

        [MaxLength(100)]
        public string? Ruleset { get; set; } = "Japanese";
        [MaxLength(100)]
        public string? KomiChangeStyle { get; set; } = "Automatic";
        public float Komi { get; set; } = 6.5f;

        public int PreOptions { get; set; } = 2;
        public float PreOptionPerc { get; set; } = 20;
        [MaxLength(100)]
        public string? ForceOpponentCorners { get; set; } = "Both";
        public bool CornerSwitch44 { get; set; } = true;
        public bool CornerSwitch34 { get; set; } = true;
        public bool CornerSwitch33 { get; set; } = false;
        public bool CornerSwitch45 { get; set; } = false;
        public bool CornerSwitch35 { get; set; } = false;
        public int CornerChance44 { get; set; } = 8;
        public int CornerChance34 { get; set; } = 12;
        public int CornerChance33 { get; set; } = 2;
        public int CornerChance45 { get; set; } = 2;
        public int CornerChance35 { get; set; } = 2;

        public int SuggestionOptions { get; set; } = 4;
        public bool ShowOptions { get; set; } = true;
        public bool ShowWeakerOptions { get; set; } = false;
        public bool MinVisitsPercSwitch { get; set; } = true;
        public float MinVisitsPerc { get; set; } = 10;
        public bool MaxVisitDiffPercSwitch { get; set; } = false;
        public float MaxVisitDiffPerc { get; set; } = 40;

        public bool OpponentOptionsSwitch { get; set; } = false;
        public int OpponentOptions { get; set; } = 5;
        public float OpponentOptionPerc { get; set; } = 10;
        public bool ShowOpponentOptions { get; set; } = false;

        public void Update(TrainerSettingConfig newModel)
        {
            Hash = newModel.Hash;

            Boardsize = newModel.Boardsize;
            Handicap = newModel.Handicap;
            ColorType = newModel.ColorType;
            PreMovesSwitch = newModel.PreMovesSwitch;
            PreMoves = newModel.PreMoves;
            PreVisits = newModel.PreVisits;
            SelfplayVisits = newModel.SelfplayVisits;
            SuggestionVisits = newModel.SuggestionVisits;
            OpponentVisits = newModel.OpponentVisits;
            DisableAICorrection = newModel.DisableAICorrection;

            Ruleset = newModel.Ruleset;
            KomiChangeStyle = newModel.KomiChangeStyle;
            Komi = newModel.Komi;

            PreOptions = newModel.PreOptions;
            PreOptionPerc = newModel.PreOptionPerc;
            ForceOpponentCorners = newModel.ForceOpponentCorners;
            CornerSwitch44 = newModel.CornerSwitch44;
            CornerSwitch34 = newModel.CornerSwitch34;
            CornerSwitch33 = newModel.CornerSwitch33;
            CornerSwitch45 = newModel.CornerSwitch45;
            CornerSwitch35 = newModel.CornerSwitch35;
            CornerChance44 = newModel.CornerChance44;
            CornerChance34 = newModel.CornerChance34;
            CornerChance33 = newModel.CornerChance33;
            CornerChance45 = newModel.CornerChance45;
            CornerChance35 = newModel.CornerChance35;

            SuggestionOptions = newModel.SuggestionOptions;
            ShowOptions = newModel.ShowOptions;
            ShowWeakerOptions = newModel.ShowWeakerOptions;
            MinVisitsPercSwitch = newModel.MinVisitsPercSwitch;
            MinVisitsPerc = newModel.MinVisitsPerc;
            MaxVisitDiffPercSwitch = newModel.MaxVisitDiffPercSwitch;
            MaxVisitDiffPerc = newModel.MaxVisitDiffPerc;

            OpponentOptionsSwitch = newModel.OpponentOptionsSwitch;
            OpponentOptions = newModel.OpponentOptions;
            OpponentOptionPerc = newModel.OpponentOptionPerc;
            ShowOpponentOptions = newModel.ShowOpponentOptions;

            base.Update(newModel);
        }

        public void SetHash()
        {
            Hash = GenerateHash(this);
        }

        public static string GenerateHash(TrainerSettingConfig config)
        {
            StringBuilder builder = new();

            builder.Append(config.Boardsize);
            builder.Append(config.Handicap);
            builder.Append(config.ColorType);
            builder.Append(config.PreMovesSwitch);
            builder.Append(config.PreMoves);
            builder.Append(config.PreVisits);
            builder.Append(config.SelfplayVisits);
            builder.Append(config.SuggestionVisits);
            builder.Append(config.OpponentVisits);
            builder.Append(config.DisableAICorrection);

            builder.Append(config.Ruleset);
            builder.Append(config.KomiChangeStyle);
            builder.Append(config.Komi);

            builder.Append(config.PreOptions);
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
            builder.Append(config.ShowOptions);
            builder.Append(config.ShowWeakerOptions);
            builder.Append(config.MinVisitsPercSwitch);
            builder.Append(config.MinVisitsPerc);
            builder.Append(config.MaxVisitDiffPercSwitch);
            builder.Append(config.MaxVisitDiffPerc);

            builder.Append(config.OpponentOptionsSwitch);
            builder.Append(config.OpponentOptions);
            builder.Append(config.OpponentOptionPerc);
            builder.Append(config.ShowOpponentOptions);

            byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(builder.ToString());
            byte[] hashBytes = MD5.Create().ComputeHash(inputBytes);
            return string.Join("", hashBytes.Select(x => x.ToString("X2")));
        }
    }
}
