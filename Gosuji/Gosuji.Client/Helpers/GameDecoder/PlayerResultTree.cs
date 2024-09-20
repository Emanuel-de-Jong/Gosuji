using Gosuji.Client.Models.Trainer;

namespace Gosuji.Client.Helpers.GameDecoder
{
    public class PlayerResultTree
    {
        public short Y { get; set; }
        public short X { get; set; }

        public EPlayerResult? Value { get; set; }

        public PlayerResultTree? Parent { get; set; }
        public List<PlayerResultTree> Children { get; set; }
        public Dictionary<short, Dictionary<short, PlayerResultTree>> Nodes { get; set; }


        public PlayerResultTree() : this(null, null, 0, 0) { }

        public PlayerResultTree(PlayerResultTree? parent, EPlayerResult? value, short x, short y)
        {
            Y = y;
            X = x;

            Value = value;

            Parent = parent;
            Children = [];
            Nodes = parent != null ? parent.Nodes : [];

            if (!Nodes.ContainsKey(Y))
            {
                Nodes.Add(Y, []);
            }

            Nodes[Y][X] = this;
        }


        public PlayerResultTree Add(EPlayerResult value, short x, short y)
        {
            PlayerResultTree newNode = new(this, value, x, y);
            Children.Add(newNode);

            return newNode;
        }

        public Dictionary<short, Dictionary<short, EPlayerResult>> ToDict()
        {
            Dictionary<short, Dictionary<short, EPlayerResult>> result = [];
            foreach (KeyValuePair<short, Dictionary<short, PlayerResultTree>> yPair in Nodes)
            {
                result[yPair.Key] = [];
                foreach (KeyValuePair<short, PlayerResultTree> xPair in yPair.Value)
                {
                    result[yPair.Key][xPair.Key] = xPair.Value.Value.Value;
                }
            }

            return result;
        }
    }
}
