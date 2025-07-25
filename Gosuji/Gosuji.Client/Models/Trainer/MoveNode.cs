﻿using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using System.Text.Json.Serialization;

namespace Gosuji.Client.Services.TrainerService
{
    public class MoveNode : IEquatable<MoveNode>
    {
        [JsonIgnore]
        public MoveNode? Parent { get; set; }
        public List<MoveNode> Children { get; set; } = [];

        public Move Move { get; set; }
        public EMoveOrigin? MoveOrigin { get; set; }
        public EPlayerResult? PlayerResult { get; set; }
        public MoveSuggestionList? Suggestions { get; set; }
        public Coord? ChosenNotPlayedCoord { get; set; }
        public double? Result { get; set; }

        public bool? IsCurrent;
        public bool? IsMainBranch;

        public MoveNode() { }

        public MoveNode(Move move, MoveNode? parent = null)
        {
            Move = move;
            Parent = parent;
        }

        public MoveNode Add(MoveNode newNode)
        {
            newNode.Parent = this;
            Children.Add(newNode);
            return newNode;
        }

        public MoveNode Add(Move move)
        {
            return Add(new MoveNode(move, this));
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
                MoveOrigin == other.MoveOrigin;
        }
    }
}
