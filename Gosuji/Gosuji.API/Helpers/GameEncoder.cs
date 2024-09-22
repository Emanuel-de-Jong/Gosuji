using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Helpers
{
    public class GameEncoder
    {
        private MoveTree tree;
        private int nodeId;
        private BitUtils bitUtils;

        public byte[] Encode(MoveTree tree)
        {
            this.tree = tree;

            nodeId = 0;
            bitUtils = new();
            bitUtils.EncodeInit();

            EncodeLoop(tree.RootNode, 0);

            bitUtils.AddEnum(ENodeIndicator.END, 6);

            return bitUtils.ToArray();
        }

        public void EncodeLoop(MoveNode node, int parentNodeId)
        {
            int currentNodeId = ++nodeId;

            EncodeNode(node, parentNodeId);

            foreach (MoveNode child in node.Children)
            {
                EncodeLoop(child, currentNodeId);
            }
        }

        private void EncodeNode(MoveNode node, int parentNodeId)
        {
            bitUtils.AddEnum(ENodeIndicator.NODE, 6);
            bitUtils.AddInt(parentNodeId, 11);
            bitUtils.AddInt(nodeId, 11);

            EncodeMove(node.Move);

            if (tree.CurrentNode == node)
            {
                bitUtils.AddEnum(ENodeIndicator.CURRENT_NODE, 6);
            }

            if (tree.MainBranch == node)
            {
                bitUtils.AddEnum(ENodeIndicator.MAIN_BRANCH, 6);
            }

            if (node.MoveOrigin != null)
            {
                bitUtils.AddEnum(ENodeIndicator.MOVE_ORIGIN, 6);
                bitUtils.AddEnum(node.MoveOrigin, 5);
            }

            if (node.PlayerResult != null)
            {
                bitUtils.AddEnum(ENodeIndicator.PLAYER_RESULT, 6);
                bitUtils.AddEnum(node.PlayerResult, 5);
            }

            if (node.ChosenNotPlayedCoord != null)
            {
                bitUtils.AddEnum(ENodeIndicator.CHOSEN_NOT_PLAYED_COORD, 6);
                bitUtils.AddInt(node.ChosenNotPlayedCoord.X, 5);
                bitUtils.AddInt(node.ChosenNotPlayedCoord.Y, 5);
            }

            if (node.Result != null)
            {
                bitUtils.AddEnum(ENodeIndicator.RESULT, 6);
                bitUtils.AddDouble(node.Result, 15, 1, true);
            }

            if (node.Suggestions != null)
            {
                bitUtils.AddEnum(ENodeIndicator.SUGGESTIONS, 6);
                EncodeSuggestions(node.Suggestions);
            }
        }

        private void EncodeMove(Move move)
        {
            int color = move.Color == EMoveColor.BLACK ? 0 : 1;
            bitUtils.AddInt(color, 1);
            bitUtils.AddInt(move.Coord.X, 5);
            bitUtils.AddInt(move.Coord.Y, 5);

            if (move.Type != null)
            {
                bitUtils.AddEnum(EMoveIndicator.MOVE_TYPE, 5);
                bitUtils.AddEnum(move.Type, 5);
            }

            bitUtils.AddEnum(EMoveIndicator.END, 5);
        }

        private void EncodeSuggestions(MoveSuggestionList suggestions)
        {
            bitUtils.AddInt(suggestions.Visits, 20);

            bitUtils.AddInt(suggestions.Suggestions.Count, 6);
            foreach (MoveSuggestion suggestion in suggestions.Suggestions)
            {
                EncodeSuggestion(suggestion);
            }

            if (suggestions.AnalyzeMoveSuggestion != null)
            {
                bitUtils.AddEnum(ESuggestionsIndicator.ANALYZE_MOVE_SUGGESTION, 5);
                EncodeSuggestion(suggestions.AnalyzeMoveSuggestion);
            }

            if (suggestions.PassSuggestion != null)
            {
                bitUtils.AddEnum(ESuggestionsIndicator.PASS_SUGGESTION, 5);
                EncodeSuggestion(suggestions.PassSuggestion);
            }

            bitUtils.AddEnum(ESuggestionsIndicator.END, 5);
        }

        private void EncodeSuggestion(MoveSuggestion suggestion)
        {
            bitUtils.AddInt(suggestion.Coord.X, 5);
            bitUtils.AddInt(suggestion.Coord.Y, 5);
            bitUtils.AddInt(suggestion.Visits, 20);
            bitUtils.AddDouble(suggestion.Score.Winrate, 17, 3);
            bitUtils.AddDouble(suggestion.Score.ScoreLead, 21, 3, true);

            bitUtils.AddInt(suggestion.Continuation.Count, 9);
            foreach (Coord coord in suggestion.Continuation)
            {
                bitUtils.AddInt(coord.X, 5);
                bitUtils.AddInt(coord.Y, 5);
            }

            bitUtils.AddEnum(ESuggestionIndicator.END, 5);
        }
    }

    public enum ENodeIndicator
    {
        NODE = 0,
        MOVE = 1,
        CURRENT_NODE = 2,
        MAIN_BRANCH = 3,
        MOVE_ORIGIN = 4,
        PLAYER_RESULT = 5,
        CHOSEN_NOT_PLAYED_COORD = 6,
        RESULT = 7,
        SUGGESTIONS = 8,
        END = 63
    }

    public enum EMoveIndicator
    {
        MOVE_TYPE = 0,
        END = 31
    }

    public enum ESuggestionsIndicator
    {
        ANALYZE_MOVE_SUGGESTION = 0,
        PASS_SUGGESTION = 1,
        END = 31
    }

    public enum ESuggestionIndicator
    {
        END = 31
    }
}
