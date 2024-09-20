using Gosuji.Client.Models.Trainer;

namespace Gosuji.Client.Services.Trainer
{
    public class AnalyzeResponse
    {
        public MoveSuggestionList MoveSuggestionList { get; set; }
        public string? Result { get; set; }
    }
}
