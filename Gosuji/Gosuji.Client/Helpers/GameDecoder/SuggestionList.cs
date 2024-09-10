using Gosuji.Client.Models.Trainer;

namespace Gosuji.Client.Helpers.GameDecoder
{
    public class SuggestionList
    {
        public List<MoveSuggestion> Suggestions { get; set; } = [];

        public MoveSuggestion? AnalyzeMoveSuggestion { get; set; }

        public void Add(MoveSuggestion suggestion)
        {
            Suggestions.Add(suggestion);
        }
    }
}
