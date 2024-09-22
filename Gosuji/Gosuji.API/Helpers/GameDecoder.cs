using AngleSharp.Dom;
using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Helpers
{
    public class GameDecoder
    {
        private byte[] data;
        private MoveTree tree;
        private Dictionary<int, MoveNode> nodes;
        private BitUtils bitUtils;

        public MoveTree Decode(byte[] data)
        {
            this.data = data;

            tree = new();
            nodes = new();
            bitUtils = new();

            bitUtils.DecodeInit(data);

            ENodeIndicator indicator = bitUtils.ExtractEnum<ENodeIndicator>(6);
            if (indicator == ENodeIndicator.NODE)
            {
                DecodeLoop();

                MoveNode defaultRootNode = tree.RootNode;
                tree.RootNode = tree.RootNode.Children[0];
                tree.AllNodes.Remove(defaultRootNode);
            }

            return tree;
        }

        private void DecodeLoop()
        {
            MoveNode moveNode = new();

            int parentNodeId = bitUtils.ExtractInt(11);
            int nodeId = bitUtils.ExtractInt(11);

            moveNode.Move = DecodeMove();

            ENodeIndicator indicator;
            do
            {
                indicator = bitUtils.ExtractEnum<ENodeIndicator>(6);
                if (indicator == ENodeIndicator.CURRENT_NODE)
                {
                    tree.CurrentNode = moveNode;
                }
                else if (indicator == ENodeIndicator.MAIN_BRANCH)
                {
                    tree.MainBranch = moveNode;
                }
                else if (indicator == ENodeIndicator.MOVE_ORIGIN)
                {
                    moveNode.MoveOrigin = bitUtils.ExtractEnum<EMoveOrigin>(5);
                }
                else if (indicator == ENodeIndicator.CHOSEN_NOT_PLAYED_COORD)
                {
                    moveNode.ChosenNotPlayedCoord = new Coord(bitUtils.ExtractInt(5), bitUtils.ExtractInt(5));
                }
                else if (indicator == ENodeIndicator.RESULT)
                {
                    moveNode.Result = bitUtils.ExtractDouble(15, 1, true);
                }
                else if (indicator == ENodeIndicator.SUGGESTIONS)
                {
                    moveNode.Suggestions = DecodeSuggestions();
                }
            } while (indicator is not ENodeIndicator.NODE and not ENodeIndicator.END);

            if (indicator == ENodeIndicator.NODE)
            {
                tree.AllNodes.Add(moveNode);
                nodes.Add(nodeId, moveNode);

                if (parentNodeId != 0)
                {
                    nodes[parentNodeId].Add(moveNode);
                }

                DecodeLoop();
            }
        }

        private Move DecodeMove()
        {
            Move move = new();

            move.Color = bitUtils.ExtractEnum<EMoveColor>(1);
            move.Coord = new Coord(bitUtils.ExtractInt(5), bitUtils.ExtractInt(5));

            EMoveIndicator indicator;
            do
            {
                indicator = bitUtils.ExtractEnum<EMoveIndicator>(5);
                if (indicator == EMoveIndicator.MOVE_TYPE)
                {
                    move.Type = bitUtils.ExtractEnum<EMoveType>(5);
                }
            } while (indicator != EMoveIndicator.END);

            return move;
        }

        private MoveSuggestionList DecodeSuggestions()
        {
            MoveSuggestionList suggestions = new();

            int suggestionCount = bitUtils.ExtractInt(6);
            for (int i = 0; i < suggestionCount; i++)
            {
                suggestions.Add(DecodeSuggestion());
            }

            ESuggestionsIndicator indicator;
            do
            {
                indicator = bitUtils.ExtractEnum<ESuggestionsIndicator>(5);
                if (indicator == ESuggestionsIndicator.ANALYZE_MOVE_SUGGESTION)
                {
                    suggestions.AnalyzeMoveSuggestion = DecodeSuggestion();
                }
                else if (indicator == ESuggestionsIndicator.PASS_SUGGESTION)
                {
                    suggestions.PassSuggestion = DecodeSuggestion();
                }
            } while (indicator != ESuggestionsIndicator.END);

            return suggestions;
        }

        private MoveSuggestion DecodeSuggestion()
        {
            MoveSuggestion suggestion = new();

            suggestion.Coord = new Coord(bitUtils.ExtractInt(5), bitUtils.ExtractInt(5));
            suggestion.Visits = bitUtils.ExtractInt(20);
            suggestion.Score = new Score(bitUtils.ExtractDouble(17, 3), bitUtils.ExtractDouble(21, 3, true));

            int continuationCount = bitUtils.ExtractInt(9);
            for (int i = 0; i < continuationCount; i++)
            {
                suggestion.Continuation.Add(new Coord(bitUtils.ExtractInt(5), bitUtils.ExtractInt(5)));
            }

            ESuggestionIndicator indicator;
            do
            {
                indicator = bitUtils.ExtractEnum<ESuggestionIndicator>(5);
            } while (indicator != ESuggestionIndicator.END);

            return suggestion;
        }
    }
}
