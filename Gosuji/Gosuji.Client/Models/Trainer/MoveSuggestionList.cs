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

        public void Filter(EMoveColor color, double minVisitsPerc, double maxVisitDiffPerc, int moveOptions)
        {
            FilterPass();
            FilterByVisits(minVisitsPerc, maxVisitDiffPerc);
            FilterByScoreLead(color);
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
            if (Suggestions.Count == 0 || (Suggestions.Count == 1 && PassSuggestion == null))
            {
                return;
            }

            MoveSuggestion failsaveSuggestion = Suggestions[0];

            int highestVisits = Suggestions.Max(s => s.Visits);

            int minVisitThreshold = (int)Math.Min(150.0, Visits/3.5);
            int minVisits = (int)Math.Round(minVisitsPerc / 100.0 * highestVisits);
            minVisits = Math.Max(minVisits, minVisitThreshold);

            int maxVisitDiff = (int)Math.Round(maxVisitDiffPerc / 100.0 * highestVisits);

            int lastSuggestionVisits = Suggestions[0].Visits;
            List<MoveSuggestion> suggestionsToRem = [];
            foreach (MoveSuggestion suggestion in Suggestions)
            {
                if (suggestion.Visits < minVisits || lastSuggestionVisits - suggestion.Visits > maxVisitDiff)
                {
                    suggestionsToRem.Add(suggestion);
                }

                if (lastSuggestionVisits > suggestion.Visits)
                {
                    lastSuggestionVisits = suggestion.Visits;
                }
            }

            foreach (MoveSuggestion suggestion in suggestionsToRem)
            {
                Suggestions.Remove(suggestion);
            }

            if (Suggestions.Count == 0)
            {
                Suggestions.Add(failsaveSuggestion);
            }
        }

        public void FilterByScoreLead(EMoveColor color)
        {
            if (Suggestions.Count == 0 || (Suggestions.Count == 1 && PassSuggestion == null))
            {
                return;
            }

            double bestScoreLead = color == EMoveColor.BLACK
                ? Suggestions.Max(s => s.Score.ScoreLead)
                : Suggestions.Min(s => s.Score.ScoreLead);

            double threshold = 3;
            Suggestions.RemoveAll(s => Math.Abs(s.Score.ScoreLead - bestScoreLead) > threshold);
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
