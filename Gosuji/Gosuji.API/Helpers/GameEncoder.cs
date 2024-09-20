using Gosuji.API.Services.TrainerService;

namespace Gosuji.API.Helpers
{
    public class GameEncoder
    {
        private List<byte> data = [];
        private int nodeId = 0;

        public byte[] Encode(MoveTree tree)
        {
            EncodeLoop(tree.RootNode);

            return data.ToArray();
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

            if (node.Suggestions != null)
            {
                data.AddRange(BitConverter.GetBytes((int)EDataTypes.Suggestions));
                //data.AddRange(BitConverter.GetBytes(node.Suggestions.Count));
                //foreach (MoveSuggestion suggestion in node.Suggestions)
                //{
                //    data.AddRange(BitConverter.GetBytes(suggestion.Move.X));
                //    data.AddRange(BitConverter.GetBytes(suggestion.Move.Y));
                //    data.AddRange(BitConverter.GetBytes(suggestion.Score));
                //}
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
                data.AddRange(BitConverter.GetBytes((double)node.Result));
            }
        }
    }

    public enum EDataTypes
    {
        Node = -1,
        Move = 0,
        MoveType = 1,
        Suggestions = 2,
        ChosenNotPlayedCoord = 3,
        Result = 4,
    }
}
