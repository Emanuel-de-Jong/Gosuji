using Gosuji.Client.Models.Trainer;

namespace Gosuji.Client.Services.Trainer
{
    public class AnalyzeResponse
    {
        public MoveSuggestionList SuggestionList { get; set; }
        public int? PlayIndex { get; set; }
        public double? Result { get; set; }

        public AnalyzeResponse() { }

        public AnalyzeResponse(MoveSuggestionList suggestionList, int? playIndex, double? result)
        {
            SuggestionList = suggestionList;
            PlayIndex = playIndex;
            Result = result;
        }
    }
}
