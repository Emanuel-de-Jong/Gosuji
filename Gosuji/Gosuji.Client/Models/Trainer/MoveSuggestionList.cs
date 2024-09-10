using System.Collections;

namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestionList
    {
        public List<MoveSuggestion> Suggestions { get; set; } = [];
        public MoveSuggestion? AnalyzeMoveSuggestion { get; set; }
        public int? PlayIndex { get; set; }
        public bool IsPass { get; set; } = false;

        public void Add(MoveSuggestion suggestion)
        {
            Suggestions.Add(suggestion);
        }

        public bool CheckPass()
        {
            if (Suggestions.Count == 0)
            {
                return IsPass;
            }

            double highestScoreLead = Math.Round(Suggestions[0].Score.ScoreLead, 2);
            for (int i = 1; i < Suggestions.Count; i++)
            {
                if (Suggestions[i].IsPass && Math.Round(Suggestions[i].Score.ScoreLead, 2) == highestScoreLead)
                {
                    IsPass = true;
                    PlayIndex = i;
                    break;
                }
            }

            return IsPass;
        }

        public void AddGrades()
        {
            int gradeIndex = 0;
            for (int i = 0; i < Suggestions.Count; i++)
            {
                MoveSuggestion suggestion = Suggestions[i];
                if (suggestion.IsPass)
                {
                    continue;
                }

                if (i != 0 && suggestion.Visits != Suggestions[i - 1].Visits)
                {
                    gradeIndex++;
                }

                suggestion.Grade = ((char)(gradeIndex + 65)).ToString();
            }
        }
    }
}
