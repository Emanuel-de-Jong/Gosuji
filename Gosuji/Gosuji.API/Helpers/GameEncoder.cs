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
            nodeId++;

            data.Add((byte)nodeId);
            data.Add((byte)EDataTypes.Node);

            data.Add((byte)node.Move.Coord.X);
            data.Add((byte)node.Move.Coord.Y);

            if (node.MoveType != null)
            {
                data.Add((byte)EDataTypes.MoveType);
                data.Add((byte)node.MoveType);
            }

            if (node.ChosenNotPlayedCoord != null)
            {
                data.Add((byte)EDataTypes.ChosenNotPlayedCoord);
                data.Add((byte)node.ChosenNotPlayedCoord.X);
                data.Add((byte)node.ChosenNotPlayedCoord.Y);
            }

            if (node.Result != null)
            {
                data.Add((byte)EDataTypes.Result);
                data.AddRange(BitConverter.GetBytes((float)node.Result.Value));
            }

            EncodeSuggestions(node.Suggestions);
        }

        private void EncodeSuggestions(MoveSuggestionList? suggestions)
        {
            if (suggestions == null)
            {
                return;
            }

            data.Add((byte)ESuggestionsField.Suggestions);

            foreach (MoveSuggestion suggestion in suggestions.Suggestions)
            {
                data.Add((byte)(suggestion.Score.Winrate / 100.0));
            }

            if (suggestions.AnalyzeMoveSuggestion != null)
            {
                data.Add((byte)ESuggestionsField.AnalyzeMoveSuggestion);
                data.Add((byte)(suggestions.AnalyzeMoveSuggestion.Score.Winrate / 100.0));
            }

            if (suggestions.PassSuggestion != null)
            {
                data.Add((byte)ESuggestionsField.PassSuggestion);
                data.Add((byte)(suggestions.PassSuggestion.Score.Winrate / 100.0));
            }

            data.Add((byte)ESuggestionsField.Visits);
            data.AddRange(BitConverter.GetBytes(suggestions.Visits));

            if (suggestions.PlayIndex != null)
            {
                data.Add((byte)ESuggestionsField.PlayIndex);
                data.Add((byte)suggestions.PlayIndex.Value);
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
