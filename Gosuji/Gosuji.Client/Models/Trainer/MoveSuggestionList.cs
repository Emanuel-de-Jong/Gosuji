namespace Gosuji.Client.Models.Trainer
{
    public class MoveSuggestionList
    {
        public MoveSuggestion[] Suggestions { get; set; }
        public MoveSuggestion? AnalyzeMoveSuggestion { get; set; }
        public MoveSuggestion? PassSuggestion { get; set; }
        public bool IsPass { get; set; }
    }
}
