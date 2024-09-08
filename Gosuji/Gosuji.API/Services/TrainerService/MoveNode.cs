using Gosuji.Client.Models;

namespace Gosuji.API.Services.TrainerService
{
    public class MoveNode
    {
        public MoveNode? Parent { get; set; }
        public List<MoveNode> Children { get; set; } = [];
        public Move Move { get; set; }
        public int Depth => Parent?.Depth + 1 ?? 0;

        public MoveNode(Move move, MoveNode? parent)
        {
            Move = move;
            Parent = parent;
        }

        public void Add(Move move)
        {
            Children.Add(new MoveNode(move, this));
        }
    }
}
