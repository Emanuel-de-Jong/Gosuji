namespace Gosuji.Client.Helpers.GameDecoder
{
    public class SuggestionList
    {
        public List<Suggestion> Suggestions { get; set; } = [];

        public Suggestion? AnalyzeMoveSuggestion { get; set; }

        public void Add(Suggestion suggestion)
        {
            Suggestions.Add(suggestion);
        }
    }
}
