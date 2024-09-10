using Gosuji.Client.Models;

namespace Gosuji.API.Services.TrainerService
{
    public class MoveTree
    {
        public MoveNode? RootNode { get; set; }
        public List<MoveNode> AllNodes { get; set; } = [];
        public MoveNode? CurrentNode { get; set; }

        public MoveNode Add(Move move)
        {
            MoveNode newNode;
            if (RootNode == null)
            {
                newNode = new MoveNode(move);
                RootNode = newNode;
            }
            else
            {
                CurrentNode ??= RootNode;
                newNode = CurrentNode.Add(move);
            }

            CurrentNode = newNode;
            AllNodes.Add(newNode);

            return newNode;
        }

        public void Remove(MoveNode node)
        {
            if (node.Parent == null)
            {
                RootNode = null;
                CurrentNode = null;
                AllNodes.Clear();
                return;
            }

            node.IterateChildren(true).ToList().ForEach(c => AllNodes.Remove(c));
            node.Parent.Children.Remove(node);

            if (CurrentNode == null || CurrentNode.Equals(node))
            {
                CurrentNode = node.Parent;
            }
        }
    }
}
