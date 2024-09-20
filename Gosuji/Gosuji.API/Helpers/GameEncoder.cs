using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Helpers
{
    public class GameEncoder
    {
        private List<byte> data = [];
        private int nodeId = 0;

        public List<byte> Encode(MoveTree tree)
        {
            EncodeLoop(tree.RootNode);

            return data;
        }

        public void EncodeLoop(MoveNode node)
        {
            foreach (MoveNode child in node.Children)
            {
                EncodeNode(child);
            }
        }

        private void EncodeNode(MoveNode node)
        {
            data.AddRange(BitConverter.GetBytes(nodeId));
            data.AddRange(BitConverter.GetBytes((int)EDataTypes.Node));

            data.AddRange(BitConverter.GetBytes(node.Move.X.Value));
            data.AddRange(BitConverter.GetBytes(node.Move.Y.Value));

            if (node.MoveType != null)
            {
                data.AddRange(BitConverter.GetBytes((int)EDataTypes.MoveType));
                data.AddRange(BitConverter.GetBytes((int)node.MoveType));
            }

            if (node.ChosenNotPlayedCoord != null)
            {
                data.AddRange(BitConverter.GetBytes((int)EDataTypes.ChosenNotPlayedCoord));
                data.AddRange(BitConverter.GetBytes(node.ChosenNotPlayedCoord.X));
                data.AddRange(BitConverter.GetBytes(node.ChosenNotPlayedCoord.Y));
            }

            if (node.Result != null)
            {
                data.AddRange(BitConverter.GetBytes((int)EDataTypes.Result));
                data.AddRange(BitConverter.GetBytes(node.Result.Value));
            }

            EncodeSuggestions(node.Suggestions);
        }

        private void EncodeSuggestions(MoveSuggestionList? suggestions)
        {
            if (suggestions == null)
            {
                return;
            }

            data.AddRange(BitConverter.GetBytes((int)ESuggestionsField.Suggestions));
            foreach (MoveSuggestion suggestion in suggestions.Suggestions)
            {
                data.AddRange(BitConverter.GetBytes(suggestion.Score.Winrate));
                data.AddRange(BitConverter.GetBytes(suggestion.Score.ScoreLead));
            }

            if (suggestions.AnalyzeMoveSuggestion != null)
            {
                data.AddRange(BitConverter.GetBytes((int)ESuggestionsField.AnalyzeMoveSuggestion));
                data.AddRange(BitConverter.GetBytes(suggestions.AnalyzeMoveSuggestion.Score.Winrate));
                data.AddRange(BitConverter.GetBytes(suggestions.AnalyzeMoveSuggestion.Score.ScoreLead));
            }

            if (suggestions.PassSuggestion != null)
            {
                data.AddRange(BitConverter.GetBytes((int)ESuggestionsField.PassSuggestion));
                data.AddRange(BitConverter.GetBytes(suggestions.PassSuggestion.Score.Winrate));
                data.AddRange(BitConverter.GetBytes(suggestions.PassSuggestion.Score.ScoreLead));
            }

            data.AddRange(BitConverter.GetBytes((int)ESuggestionsField.Visits));
            data.AddRange(BitConverter.GetBytes(suggestions.Visits));

            if (suggestions.PlayIndex != null)
            {
                data.AddRange(BitConverter.GetBytes((int)ESuggestionsField.PlayIndex));
                data.AddRange(BitConverter.GetBytes(suggestions.PlayIndex.Value));
            }
        }
    }

    public enum EDataTypes
    {
        Node,
        Move,
        MoveType,
        ChosenNotPlayedCoord,
        Result,
        Suggestions,
    }

    public enum ESuggestionsField
    {
        Suggestions,
        AnalyzeMoveSuggestion,
        PassSuggestion,
        Visits,
        PlayIndex,
    }
}
