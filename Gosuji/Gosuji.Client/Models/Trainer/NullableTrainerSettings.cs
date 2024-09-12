using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components;

namespace Gosuji.Client.Models.Trainer
{
    public class NullableTrainerSettings
    {
        public TrainerSettingConfig TrainerSettingConfig { get; set; }

        public string Ruleset { get; set; }
        public double Komi { get; set; }

        public int SuggestionVisits { get; set; }
        public int OpponentVisits { get; set; }
        public int PreVisits { get; set; }
        public int SelfplayVisits { get; set; }

        public NullableTrainerSettings() { }

        public NullableTrainerSettings(TrainerSettingConfig trainerSettingConfig, string langId)
        {
            Init(trainerSettingConfig, langId);
        }

        public void Init(TrainerSettingConfig trainerSettingConfig, string langId)
        {
            string? ruleset = trainerSettingConfig.Ruleset;
            if (ruleset == null)
            {
                if (langId == ELanguage.zh.ToString() ||
                    langId == ELanguage.ko.ToString())
                {
                    ruleset = "Chinese";
                }
                else
                {
                    ruleset = "Japanese";
                }
            }

            double komi = trainerSettingConfig.GetKomi(ruleset);

            int suggestionVisits = trainerSettingConfig.SuggestionVisits != null ? trainerSettingConfig.SuggestionVisits.Value : 200;
            int opponentVisits = trainerSettingConfig.OpponentVisits != null ? trainerSettingConfig.OpponentVisits.Value : 200;
            int preVisits = trainerSettingConfig.PreVisits != null ? trainerSettingConfig.PreVisits.Value : 200;
            int selfplayVisits = trainerSettingConfig.SelfplayVisits != null ? trainerSettingConfig.SelfplayVisits.Value : 200;

            TrainerSettingConfig = trainerSettingConfig;
            Ruleset = ruleset;
            Komi = komi;
            SuggestionVisits = suggestionVisits;
            OpponentVisits = opponentVisits;
            PreVisits = preVisits;
            SelfplayVisits = selfplayVisits;
        }

        public async Task SetKomi(ChangeEventArgs e)
        {
            if (!double.TryParse(e.Value?.ToString(), out double newKomi))
            {
                return;
            }
            Komi = newKomi;
            TrainerSettingConfig.Komi = newKomi;
        }

        public async Task SetRuleset(ChangeEventArgs e)
        {
            string newRuleset = e.Value?.ToString();
            Ruleset = newRuleset;
            TrainerSettingConfig.Ruleset = newRuleset;
        }

        public async Task SetSuggestionVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            SuggestionVisits = visits;
            TrainerSettingConfig.SuggestionVisits = visits;
        }

        public async Task SetOpponentVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            OpponentVisits = visits;
            TrainerSettingConfig.OpponentVisits = visits;
        }

        public async Task SetPreVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            PreVisits = visits;
            TrainerSettingConfig.PreVisits = visits;
        }

        public async Task SetSelfplayVisits(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int visits))
            {
                return;
            }
            SelfplayVisits = visits;
            TrainerSettingConfig.SelfplayVisits = visits;
        }
    }
}
