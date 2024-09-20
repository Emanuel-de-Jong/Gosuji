using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;

namespace Gosuji.API.Services.TrainerService
{
    public class MoveNode : IEquatable<MoveNode>
    {
        public MoveNode? Parent { get; set; }
        public List<MoveNode> Children { get; set; } = [];
        public int Depth { get; set; }

        public Move Move { get; set; }
        public EMoveType? MoveType { get; set; }
        public EPlayerResult? PlayerResult { get; set; }
        public MoveSuggestionList? Suggestions { get; set; }
        public Coord? ChosenNotPlayedCoord { get; set; }
        public double? Result { get; set; }

        public MoveNode(Move move, MoveNode? parent = null)
        {
            Move = move;
            Parent = parent;
            Depth = parent?.Depth + 1 ?? 0;
        }

        public MoveNode Add(Move move)
        {
            MoveNode newNode = new(move, this);
            Children.Add(newNode);
            return newNode;
        }

        public IEnumerable<MoveNode> IterateChildren(bool includeSelf = false)
        {
            if (includeSelf)
            {
                yield return this;
            }

            foreach (MoveNode child in Children)
            {
                yield return child;
                foreach (MoveNode grandChild in child.IterateChildren())
                {
                    yield return grandChild;
                }
            }
        }

        public bool Equals(MoveNode other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Move.Equals(other.Move) &&
                (Parent == null || Parent.Move.Equals(other.Parent?.Move)) &&
                MoveType == other.MoveType;
        }
    }
}
