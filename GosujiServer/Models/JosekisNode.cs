using IGOEnchi.GoGameLogic;

namespace GosujiServer.Models
{
    public class JosekisNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsBlack { get; set; }
        public string? Comment { get; set; }
        public List<TextLabel>? Labels { get; set; }
        public List<Mark>? Marks { get; set; }

        public JosekisNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public JosekisNode(GoMoveNode moveNode)
        {
            X = moveNode.Stone.X;
            Y = moveNode.Stone.Y;
            IsBlack = moveNode.Stone.IsBlack;
            Comment = moveNode.Comment;
            Labels = moveNode.Markup.Labels;
            Marks = moveNode.Markup.Marks;
        }

        public bool Compare(GoMoveNode moveNode)
        {
            return X == moveNode.Stone.X && Y == moveNode.Stone.Y;
        }
    }
}
