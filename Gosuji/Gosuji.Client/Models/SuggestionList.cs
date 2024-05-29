namespace Gosuji.Client.Models
{
    public class SuggestionList
    {
        public List<Suggestion> Suggestions { get; set; } = new();

        public Suggestion? AnalyzeMoveSuggestion { get; set; }

        public void Add(Suggestion suggestion)
        {
            Suggestions.Add(suggestion);
        }
    }
}
