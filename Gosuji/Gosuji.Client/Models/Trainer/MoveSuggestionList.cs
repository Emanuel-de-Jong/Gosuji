namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestionList
    {
        public const int MAX_OPTIONS = 15;

        public List<MoveSuggestion> Suggestions { get; set; } = [];
        public MoveSuggestion? AnalyzeMoveSuggestion { get; set; }
        public MoveSuggestion? PassSuggestion { get; set; }
        public int Visits { get; set; }

        public MoveSuggestionList() { }

        public bool Add(MoveSuggestion suggestion)
        {
            if (Suggestions.Count >= MAX_OPTIONS)
            {
                return false;
            }

            Suggestions.Add(suggestion);
            return true;
        }

        public void Filter(double minVisitsPerc, double maxVisitDiffPerc, int moveOptions)
        {
            FilterPass();
            FilterByVisits(minVisitsPerc, maxVisitDiffPerc);
            AddGrades();
            FilterByOptions(moveOptions);
        }

        public void FilterPass()
        {
            if (Suggestions.Count == 0)
            {
                return;
            }

            double mainScoreLead = Math.Round(Suggestions[0].Score.ScoreLead, 2);
            foreach (MoveSuggestion suggestion in Suggestions)
            {
                if (!suggestion.IsPass)
                {
                    continue;
                }

                if (Math.Round(suggestion.Score.ScoreLead, 2) >= mainScoreLead)
                {
                    PassSuggestion = suggestion;
                }

                Suggestions.Remove(suggestion);
                break;
            }
        }

        public void FilterByVisits(double minVisitsPerc, double maxVisitDiffPerc)
        {
            int highestVisits = 0;
            foreach (MoveSuggestion suggestion in Suggestions)
            {
                if (highestVisits < suggestion.Visits)
                {
                    highestVisits = suggestion.Visits;
                }
            }

            int minVisits = (int)Math.Round(minVisitsPerc / 100.0 * Visits);
            int maxVisitDiff = (int)Math.Round(maxVisitDiffPerc / 100.0 * Math.Max(Visits, highestVisits));

            int index;
            int lastSuggestionVisits = Suggestions[0].Visits;
            for (index = 1; index < Suggestions.Count; index++)
            {
                MoveSuggestion suggestion = Suggestions[index];
                if (suggestion.Visits < minVisits || lastSuggestionVisits - suggestion.Visits > maxVisitDiff)
                {
                    break;
                }

                if (lastSuggestionVisits > suggestion.Visits)
                {
                    lastSuggestionVisits = suggestion.Visits;
                }
            }

            // If loop broke, we want to go back to the last suggestion
            // If loop didn't break, we want to revert the last increment before the loop check
            index--;

            Suggestions = Suggestions[..(index + 1)];
        }

        public void AddGrades()
        {
            int gradeIndex = 0;
            for (int i = 0; i < Suggestions.Count; i++)
            {
                MoveSuggestion suggestion = Suggestions[i];
                if (i != 0 && suggestion.Visits != Suggestions[i - 1].Visits)
                {
                    gradeIndex++;
                }

                suggestion.Grade = ((char)(gradeIndex + 65)).ToString();
            }
        }

        public void FilterByOptions(int moveOptions)
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

            // If loop broke, we want to go back to the last suggestion
            // If loop didn't break, we want to revert the last increment before the loop check
            index--;

            Suggestions = Suggestions[..(index + 1)];
        }
    }
}
