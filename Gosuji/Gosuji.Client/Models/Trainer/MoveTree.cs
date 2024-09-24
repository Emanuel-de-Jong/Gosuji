using Gosuji.Client.Models;
using System.Text.Json.Serialization;

namespace Gosuji.Client.Services.TrainerService
{
    public class MoveTree
    {
        public MoveNode RootNode { get; set; } = new MoveNode(Move.ROOT_MOVE);
        [JsonIgnore]
        public List<MoveNode> AllNodes { get; set; } = [];
        [JsonIgnore]
        public MoveNode CurrentNode { get; set; }
        [JsonIgnore]
        public MoveNode? MainBranch { get; set; }

        public MoveTree()
        {
            AllNodes.Add(RootNode);
            CurrentNode = RootNode;
        }

        public MoveNode Add(Move move)
        {
            if (CurrentNode.Children.Count > 0)
            {
                MoveNode? childNode = CurrentNode.Children.Find(c => c.Move.Equals(move));
                if (childNode != null)
                {
                    CurrentNode = childNode;
                    return childNode;
                }
            }

            MoveNode newNode = CurrentNode.Add(move);

            CurrentNode = newNode;
            AllNodes.Add(newNode);

            return newNode;
        }

        public void Remove(MoveNode node)
        {
            node.IterateChildren(true).ToList().ForEach(c => AllNodes.Remove(c));
            node.Parent.Children.Remove(node);

            if (CurrentNode?.Parent == null)
            {
                CurrentNode = RootNode;
            }
            else if (CurrentNode.Equals(node))
            {
                CurrentNode = node.Parent;
            }
        }

        public void PrepareForJSON()
        {
            CurrentNode.IsCurrent = true;
            if (MainBranch != null)
            {
                MainBranch.IsMainBranch = true;
            }
        }
    }
}
