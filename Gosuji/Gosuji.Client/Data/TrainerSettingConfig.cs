﻿using Gosuji.Client.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Gosuji.Client.Data
{
    // This class is saved in an unusual way.
    // The Hash property is generated from all other mapped properties meaning configs with the same settings get the same hash.
    // We only ever want to save a single TrainerSettingConfig for each unique set of settings.
    // So when the settings of a game change, a new config is created and assigned to it without deleting the old one.
    public class TrainerSettingConfig : DbModel
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(150)]
        public string? Hash { get; set; }

        [Range(2, 25_000)]
        public int? SuggestionVisits { get; set; }
        [Range(2, 25_000)]
        public int? OpponentVisits { get; set; }
        [Range(2, 25_000)]
        public int? PreVisits { get; set; }
        [Range(2, 25_000)]
        public int? SelfplayVisits { get; set; }

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
        public EMoveColor ColorType { get; set; }
        [Required]
        public bool WrongMoveCorrection { get; set; }

        [Range(-150, 150)]
        public double? Komi { get; set; }
        [MaxLength(100)]
        public string? Ruleset { get; set; }

        [Required]
        public EForceOpponentCorners ForceOpponentCorners { get; set; }
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

        [NotMapped]
        public ELanguage Language { get; set; } = ELanguage.en;
        [NotMapped]
        public ESubscriptionType? SubscriptionType { get; set; }
        [NotMapped]
        public string? SGFRuleset { get; set; }
        [NotMapped]
        public string GetRuleset
        {
            get
            {
                if (SGFRuleset != null)
                {
                    return SGFRuleset;
                }
                if (Ruleset != null)
                {
                    return Ruleset;
                }
                if (Language is ELanguage.zh or ELanguage.ko)
                {
                    return "Chinese";
                }
                return "Japanese";
            }
        }
        [NotMapped]
        public double? SGFKomi { get; set; }
        [NotMapped]
        public double GetKomi
        {
            get
            {
                if (SGFKomi != null)
                {
                    return SGFKomi.Value;
                }
                if (Komi != null)
                {
                    return Komi.Value;
                }
                return GetDefaultKomi(GetRuleset);
            }
        }

        [NotMapped]
        public int GetSuggestionVisits => GetVisits(SuggestionVisits, 2000, new() {
            { ESubscriptionType.TIER_1, 2000 }, { ESubscriptionType.TIER_2, 2000 }, { ESubscriptionType.TIER_3, 2000 }});
        [NotMapped]
        public int GetOpponentVisits => GetVisits(OpponentVisits, 2000, new() {
            { ESubscriptionType.TIER_1, 2000 }, { ESubscriptionType.TIER_2, 2000 }, { ESubscriptionType.TIER_3, 2000 }});
        [NotMapped]
        public int GetPreVisits => GetVisits(PreVisits, 500, new() {
            { ESubscriptionType.TIER_1, 500 }, { ESubscriptionType.TIER_2, 500 }, { ESubscriptionType.TIER_3, 500 }});
        [NotMapped]
        public int GetSelfplayVisits => GetVisits(SelfplayVisits, 1500, new() {
            { ESubscriptionType.TIER_1, 1500 }, { ESubscriptionType.TIER_2, 1500 }, { ESubscriptionType.TIER_3, 1500 }});

        public double GetDefaultKomi(string ruleset)
        {
            if (Handicap != 0)
            {
                return 0.5;
            }

            if (ruleset.ToLower().Contains("chin"))
            {
                return 7.5;
            }

            return 6.5;
        }

        private int GetVisits(int? visits, int noSubVisits, Dictionary<ESubscriptionType, int> visitsBySub)
        {
            if (visits != null)
            {
                return visits.Value;
            }

            int resultVisits = noSubVisits;
            if (SubscriptionType != null)
            {
                resultVisits = visitsBySub[SubscriptionType.Value];
            }

            return G.IsLowComputeHost == false ? resultVisits : resultVisits / 10;
        }

        public TrainerSettingConfig SetHash()
        {
            Hash = GenerateHash(this);
            return this;
        }

        public static string GenerateHash(TrainerSettingConfig config)
        {
            StringBuilder builder = new();

            builder.Append(config.SuggestionVisits);
            builder.Append(config.OpponentVisits);
            builder.Append(config.PreVisits);
            builder.Append(config.SelfplayVisits);

            builder.Append(config.Boardsize);
            builder.Append(config.Handicap);
            builder.Append(config.PreMovesSwitch);
            builder.Append(config.PreMoves);
            builder.Append(config.HideOptions);
            builder.Append(config.ColorType);
            builder.Append(config.WrongMoveCorrection);

            builder.Append(config.Komi);
            builder.Append(config.Ruleset);

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
            builder.Append(config.PreOptions);
            builder.Append(config.PreOptionPercSwitch);
            builder.Append(config.PreOptionPerc);

            builder.Append(config.SuggestionOptions);
            builder.Append(config.HideWeakerOptions);
            builder.Append(config.MinVisitsPercSwitch);
            builder.Append(config.MinVisitsPerc);
            builder.Append(config.MaxVisitDiffPercSwitch);
            builder.Append(config.MaxVisitDiffPerc);

            builder.Append(config.OpponentOptions);
            builder.Append(config.HideOpponentOptions);
            builder.Append(config.OpponentOptionPercSwitch);
            builder.Append(config.OpponentOptionPerc);

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

    public enum EForceOpponentCorners
    {
        NONE = 0,
        FIRST = 1,
        SECOND = 2,
        BOTH = 3
    }
}
