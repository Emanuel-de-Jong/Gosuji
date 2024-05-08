using GosujiServer.Enums;

namespace GosujiServer.Models
{
    public class RatioTree
    {
        public short Y { get; set; }
        public short X { get; set; }

        public ERatio Value { get; set; }

        public RatioTree Parent { get; set; }
        public List<RatioTree> Children { get; set; }
        public Dictionary<short, Dictionary<short, RatioTree>> Nodes { get; set; }


        public RatioTree() : this(null, ERatio.NONE, 0, 0) { }

        public RatioTree(RatioTree parent, ERatio value, short x, short y)
        {
            Y = y;
            X = x;

            Value = value;

            Parent = parent;
            Children = new();
            Nodes = parent != null ? parent.Nodes : new();

            if (!Nodes.ContainsKey(Y)) Nodes.Add(Y, new());
            Nodes[Y][X] = this;
        }


        public RatioTree Add(ERatio value, short x, short y)
        {
            RatioTree newNode = new(this, value, x, y);
            Children.Add(newNode);

            return newNode;
        }

        public Dictionary<short, Dictionary<short, ERatio>> ToDict()
        {
            Dictionary<short, Dictionary<short, ERatio>> result = new();
            foreach (KeyValuePair<short, Dictionary<short, RatioTree>> yPair in Nodes)
            {
                result[yPair.Key] = new();
                foreach (KeyValuePair<short, RatioTree> xPair in yPair.Value)
                {
                    result[yPair.Key][xPair.Key] = xPair.Value.Value;
                }
            }

            return result;
        }
    }
}
