namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestionList
    {
        public List<MoveSuggestion> Suggestions { get; set; } = [];
        public MoveSuggestion? AnalyzeMoveSuggestion { get; set; }
        public MoveSuggestion? PassSuggestion { get; set; }
        public int Visits { get; set; }
        public int? PlayIndex { get; set; }

        public MoveSuggestionList() { }

        public MoveSuggestionList(int visits)
        {
            Visits = visits;
        }

        public void Add(MoveSuggestion suggestion)
        {
            Suggestions.Add(suggestion);
        }

        public void Filter(int moveOptions)
        {
            if (Suggestions.Count <= moveOptions)
            {
                return;
            }

            int moveOptionCount = 1;
            int index;
            for (index = 0; index < Suggestions.Count; index++)
            {
                if (index != 0 && Suggestions[index].Grade != Suggestions[index - 1].Grade)
                {
                    moveOptionCount++;
                    if (moveOptionCount > moveOptions)
                    {
                        break;
                    }
                }
            }

            Suggestions = Suggestions[..index];
        }

        public void CheckPass()
        {
            if (Suggestions.Count == 0)
            {
                return;
            }

            double highestScoreLead = Math.Round(Suggestions[0].Score.ScoreLead, 2);
            for (int i = 1; i < Suggestions.Count; i++)
            {
                MoveSuggestion suggestion = Suggestions[i];
                if (suggestion.IsPass && Math.Round(suggestion.Score.ScoreLead, 2) == highestScoreLead)
                {
                    PassSuggestion = suggestion;
                    Suggestions.Remove(PassSuggestion);
                    break;
                }
            }
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
