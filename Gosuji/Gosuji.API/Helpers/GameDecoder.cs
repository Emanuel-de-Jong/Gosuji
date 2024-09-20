using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Helpers
{
    public class GameDecoder
    {
        private MoveTree tree = new();
        private int currentIndex = 0;
        private byte[] data;

        public MoveTree Decode(List<byte> data)
        {
            this.data = data.ToArray();
            DecodeLoop(tree.RootNode);
            return tree;
        }

        private void DecodeLoop(MoveNode node)
        {
            while (currentIndex < data.Length)
            {
                MoveNode decodedNode = DecodeNode();
                node.Children.Add(decodedNode);
                decodedNode.Parent = node;

                if (decodedNode.Children.Count > 0)
                {
                    DecodeLoop(decodedNode);
                }
            }
        }

        private MoveNode DecodeNode()
        {
            int nodeId = BitConverter.ToInt32(data, currentIndex);
            currentIndex += 4;

            int dataType = BitConverter.ToInt32(data, currentIndex);
            currentIndex += 4;

            int x = data[currentIndex++];
            int y = data[currentIndex++];

            Move move = new(x, y);
            MoveNode node = new(move);

            while (currentIndex < data.Length)
            {
                EDataTypes fieldType = (EDataTypes)BitConverter.ToInt32(data, currentIndex);
                currentIndex += 4;

                switch (fieldType)
                {
                    case EDataTypes.MoveType:
                        node.MoveType = (EMoveType)BitConverter.ToInt32(data, currentIndex);
                        currentIndex += 4;
                        break;

                    case EDataTypes.ChosenNotPlayedCoord:
                        int chosenX = data[currentIndex++];
                        int chosenY = data[currentIndex++];
                        node.ChosenNotPlayedCoord = new Coord(chosenX, chosenY);
                        break;

                    case EDataTypes.Result:
                        node.Result = BitConverter.ToDouble(data, currentIndex);
                        currentIndex += 8;
                        break;

                    case EDataTypes.Suggestions:
                        node.Suggestions = DecodeSuggestions();
                        break;

                    default:
                        return node;
                }
            }

            return node;
        }

        private MoveSuggestionList? DecodeSuggestions()
        {
            MoveSuggestionList suggestions = new();

            while (currentIndex < data.Length)
            {
                ESuggestionsField suggestionsField = (ESuggestionsField)BitConverter.ToInt32(data, currentIndex);
                currentIndex += 4;

                switch (suggestionsField)
                {
                    case ESuggestionsField.Suggestions:
                        suggestions.Suggestions = DecodeMoveSuggestions();
                        break;
                    case ESuggestionsField.AnalyzeMoveSuggestion:
                        suggestions.AnalyzeMoveSuggestion = DecodeMoveSuggestion();
                        break;
                    case ESuggestionsField.PassSuggestion:
                        suggestions.PassSuggestion = DecodeMoveSuggestion();
                        break;
                    case ESuggestionsField.Visits:
                        suggestions.Visits = BitConverter.ToInt32(data, currentIndex);
                        currentIndex += 4;
                        break;
                    case ESuggestionsField.PlayIndex:
                        suggestions.PlayIndex = BitConverter.ToInt32(data, currentIndex);
                        currentIndex += 4;
                        break;
                    default:
                        return suggestions;
                }
            }

            return suggestions;
        }

        private List<MoveSuggestion> DecodeMoveSuggestions()
        {
            List<MoveSuggestion> moveSuggestions = [];

            for (int i = 0; i < MoveSuggestionList.MAX_OPTIONS; i++)
            {
                if (currentIndex >= data.Length)
                {
                    break;
                }

                MoveSuggestion suggestion = DecodeMoveSuggestion();
                moveSuggestions.Add(suggestion);
            }

            return moveSuggestions;
        }

        private MoveSuggestion DecodeMoveSuggestion()
        {
            double winrate = BitConverter.ToDouble(data, currentIndex);
            currentIndex += 8;

            double scoreLead = BitConverter.ToDouble(data, currentIndex);
            currentIndex += 8;

            return new MoveSuggestion { Score = new Score(winrate, scoreLead) };
        }
    }
}
