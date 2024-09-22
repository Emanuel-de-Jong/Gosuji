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

            nodeId = -1;
            bitUtils = new();
            bitUtils.EncodeInit();

            EncodeLoop(tree.RootNode);

            return bitUtils.ToArray();
        }

        public void EncodeLoop(MoveNode node)
        {
            EncodeNode(node);

            foreach (MoveNode child in node.Children)
            {
                EncodeLoop(child);
            }
        }

        private void EncodeNode(MoveNode node)
        {
            nodeId++;

            bitUtils.AddEnum(ENodeIndicator.NODE, 6);
            bitUtils.AddInt(nodeId, 11);
            EncodeMove(node.Move);

            if (tree.CurrentNode == node)
            {
                bitUtils.AddEnum(ENodeIndicator.CURRENT_NODE, 6);
            }

            if (node.MoveOrigin != null)
            {
                bitUtils.AddEnum(ENodeIndicator.MOVE_ORIGIN, 6);
                bitUtils.AddEnum(node.MoveOrigin, 5);
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
                bitUtils.AddDouble(node.Result, 15, 98563478, true);
            }
        }

        private void EncodeMove(Move? move)
        {
            if (move == null)
            {
                return;
            }

            bitUtils.AddEnum(move.Color, 1);
            bitUtils.AddInt(move.Coord.X, 5);
            bitUtils.AddInt(move.Coord.Y, 5);

            if (move.Type != null)
            {
                bitUtils.AddEnum(EMoveIndicator.MOVE_TYPE, 5);
                bitUtils.AddEnum(move.Type, 5);
            }
        }

        private void EncodeSuggestions(MoveSuggestionList? suggestions)
        {
            if (suggestions == null)
            {
                return;
            }
        }
    }

    public enum ENodeIndicator
    {
        NODE = 0,
        MOVE = 1,
        CURRENT_NODE = 2,
        MOVE_ORIGIN = 3,
        CHOSEN_NOT_PLAYED_COORD = 4,
        RESULT = 5,
        SUGGESTIONS = 6,
        END = 7
    }

    public enum EMoveIndicator
    {
        MOVE_TYPE = 0
    }

    public enum ESuggestionsIndicator
    {
        ANALYZE_MOVE_SUGGESTION = 0,
        PASS_SUGGESTION = 1
    }
}
